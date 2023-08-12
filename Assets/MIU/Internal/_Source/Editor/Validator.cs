using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MIU;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public delegate void FixNote();

public static class TransformDeepChildExtension
{
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }

}

class MarbleValidator : EditorWindow
{

    enum Category
    {
        Import,
        General,
        Hierarchy,
        Static,
        Dynamic,
        Gameplay,
        Render,
        Playmode
    }

    class ResultGroup
    {
        public ResultGroup(string name)
        {
            this.name = name;
        }

        public string name;

        public ResultGroup parent = null;
        public int containedErrors = 0;
        public int containedUniqueErrors = 0;

        public List<ResultNote> notes = new List<ResultNote>();

        public bool show = false;
        public bool showDefault = false;
        public bool showChanged = false;

        public List<ResultGroup> subgroups = new List<ResultGroup>();
        public SortedDictionary<string, ResultGroup> subgroupsByError = new SortedDictionary<string, ResultGroup>();

        public void Add(ResultNote note, bool groupByError = true)
        {
            showDefault = true;

            if (groupByError)
            {
                ResultGroup errorGroup;
                if (!subgroupsByError.TryGetValue(note.error, out errorGroup))
                {
                    errorGroup = new ResultGroup(note.error);
                    Add(errorGroup);
                    errorGroup.containedUniqueErrors = 1;
                    AdjustContainedUniqueErrors(1);
                    subgroupsByError.Add(note.error, errorGroup);
                }
                errorGroup.Add(note, false);
            }
            else
            {
                notes.Add(note);
                AdjustContainedErrors(1);
                SortNoteList();
            }
        }

        public void Add(ResultGroup group)
        {
            showDefault = true;

            subgroups.Add(group);
            group.parent = this;
        }

        public void Clear()
        {
            notes.Clear();
            containedErrors = 0;
            foreach (var group in subgroups)
            {
                group.Clear();
            }
        }

        void SortNoteList()
        {
            notes.Sort(delegate (ResultNote a, ResultNote b)
            {
                return b.priority - a.priority;
            });
        }

        public void AdjustContainedErrors(int shift)
        {
            var group = this;
            while (group != null)
            {
                group.containedErrors += shift;
                group = group.parent;
            }
            if (subgroups.Count == 0)
            {
                showDefault = false;
            }
        }

        public void AdjustContainedUniqueErrors(int shift)
        {
            var group = this;
            while (group != null)
            {
                group.containedUniqueErrors += shift;
                group = group.parent;
            }
        }
    }

    class SceneResult : ResultGroup
    {
        public Dictionary<Category, ResultGroup> subgroupsByCategory = new Dictionary<Category, ResultGroup>();

        public SceneResult(string name) : base(name)
        {

            foreach (var cat in Enum.GetValues(typeof(Category)))
            {
                var subgroup = new ResultGroup(Enum.GetName(typeof(Category), cat));
                subgroupsByCategory.Add((Category)cat, subgroup);
                Add(subgroup);
            }
        }

        public ResultGroup GetCategoryGroup(Category cat)
        {
            ResultGroup group;
            if (subgroupsByCategory.TryGetValue(cat, out group))
            {
                return group;
            }
            return null;
        }
    }

    class ResultNote
    {
        public ResultNote(UnityEngine.Object obj, string e, FixNote f = null, int p = 0)
        {
            this.obj = obj;

            comp = obj as Component;
            if (comp)
            {
                go = comp.gameObject;
            }

            if (!go) go = obj as GameObject;
            if (go)
            {
                path = AnimationUtility.CalculateTransformPath(go.transform, go.transform.root);
            }

            if (obj != null)
            {
                if (go && obj != go)
                {
                    name = go.name + " / " + obj.name;
                }
                else
                {
                    name = obj.name;
                }
            }
            else
            {
                name = "";
                path = "";
            }

            priority = p;
            error = e;
            fix = f;
            wasFixed = false;
        }

        // Higher comes first.
        public int priority;

        public UnityEngine.Object obj;
        public GameObject go;
        public Component comp;
        public string name;
        public string path;
        public string error;
        public FixNote fix;

        public bool wasFixed;
    }

    //List<ResultGroup> sceneResults = new List<ResultGroup>();

    Dictionary<string, SceneResult> pathToSceneResult = new Dictionary<string, SceneResult>();

    Scene _currentScene;
    SceneResult _currentSceneResult = new SceneResult("Current");
    SceneResult getCurrentSceneResult()
    {
        var s = EditorSceneManager.GetActiveScene();
        if (_currentSceneResult == null || s != _currentScene)
        {
            _currentScene = s;
            var got = pathToSceneResult.TryGetValue(_currentScene.path, out _currentSceneResult);
            if (!got)
            {
                _currentSceneResult = null;
            }
        }

        if(_currentSceneResult == null)
            _currentSceneResult = new SceneResult("Current");

        return _currentSceneResult;
    }

    SceneResult getNoteSceneResult()
    {
        var result = getCurrentSceneResult();
        return result;
    }

    SceneResult lastValidated;
    int frameFixedErrorCount = 0;

    private Vector3 scrollPos = Vector3.zero;

    string PrevalidationScene;
    //ConfigValues Config;

    int ChapterIndex = -1;
    int LevelIndex;

    bool ValidatingPlaymode = false;
    bool ValidatingPlaymodeProgressShown = false;
    double ValidatingPlaymodeStartTime = 0;
    double ValidatingPlaymodeDuration = 0;

    MarbleValidator()
    {
        titleContent = new GUIContent("MIU Validator");
    }

    void ClearResults()
    {
        _currentSceneResult = null;
        lastValidated = null;
        pathToSceneResult.Clear();
    }

    ResultNote AddNote(Category category, UnityEngine.Object obj, string e, FixNote fix = null, int priority = 0)
    {
        var note = new ResultNote(obj, e, fix, priority);
        getNoteSceneResult()
        .GetCategoryGroup(category)
        .Add(note);
        return note;
    }

    private string GetAssetPathForPrefabInstance(GameObject gameObject)
    {
        if(gameObject == null)
            return null;

        var parentObject = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
        return AssetDatabase.GetAssetPath(parentObject);
    }

    private bool CheckGoInFolder(GameObject go, GameObject folder)
    {
        while (go.transform.parent != null)
        {
            if (go == folder)
                return true;

            go = go.transform.parent.gameObject;
        }

        return false;
    }

    private void RecenterGameObjectWithoutAffectingChildren(GameObject go)
    {
        var t = go.transform;
        var tmpGo = new GameObject("TmpRoot");
        var tmpT = tmpGo.transform;

        while (t.childCount > 0)
            t.GetChild(0).parent = tmpGo.transform;

        t.localEulerAngles = Vector3.zero;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        while (tmpT.childCount > 0)
            tmpT.GetChild(0).parent = t;

        DestroyImmediate(tmpGo);
    }

    void CheckSuggestFolder(string name)
    {
        var go = GameObject.Find(name);
        if (!go)
        {
            AddNote(Category.Hierarchy, null, "No folder '" + name + "' found", delegate ()
            {
                new GameObject(name);
            }, 100);
            return;
        }

        if (go.transform.parent != null)
        {
            AddNote(Category.Hierarchy, null, "Folder '" + name + "' must be at root", delegate ()
            {
                go.transform.SetParent(null);
            }, 100);
        }
    }

    public void ValidateScene()
    {
        Validate();
    }


    public void Validate()
    {
        _currentSceneResult = new SceneResult("Current");
        var sceneResult = getCurrentSceneResult();

        string[] validRootNames = { "Static", "Dynamic", "Gameplay", "Skybox" };

        var staticFolder = GameObject.Find("Static");
        var dynamicFolder = GameObject.Find("Dynamic");
        var gameplayFolder = GameObject.Find("Gameplay");

        foreach (var n in validRootNames)
            CheckSuggestFolder(n);

        // Also check for Lighting subfolder.
        if (staticFolder)
        {
            var lightXfrm = staticFolder.transform.Find("Lighting");
            if (!lightXfrm)
            {
                AddNote(Category.Static, null, "Static folder should contain subfolder called Lighting", delegate ()
                {
                    var lightSubFolder = new GameObject("Lighting");
                    lightSubFolder.transform.SetParent(staticFolder.transform);
                }, 90);
            }
        }

        string[] allowedRootGameObjects = {
            "_CurvyGlobal_",
            "Progroups2_Groups_Container_Object"
        };

        // First, we only want groups at top level.
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        Camera rootCamera = null;
        foreach (var go in roots)
        {
            if (allowedRootGameObjects.Contains(go.name)) continue;

            var cam = CheckCamera(go);
            if (cam != null && rootCamera == null)
            {
                rootCamera = cam;
                continue;
            }

            var cameras = go.GetComponentsInChildren<Camera>(true);
            foreach (var camera in cameras)
            {
                AddNote(Category.Hierarchy, camera.gameObject, "Only the root camera should exist", delegate ()
                {
                    DestroyImmediate(camera.gameObject);
                });
            }

            var goComs = go.GetComponents<Component>();

            if (goComs.GetLength(0) != 1 || goComs[0].GetType() != typeof(Transform))
                AddNote(Category.Hierarchy, go, "Root GameObjects should only be folders/empty objects");

            if (!validRootNames.Contains(go.name))
                AddNote(Category.Hierarchy, go, "Root GameObject names must be one of " + String.Join(", ", validRootNames));
            else
            {
                // Only prompt if it's a known good folder.
                var localGo = go;
                if (goComs[0].transform.localToWorldMatrix != Matrix4x4.identity)
                    AddNote(Category.Hierarchy, goComs[0].gameObject, "Root folder should not be moved/rotated/scaled", delegate ()
                    {
                        RecenterGameObjectWithoutAffectingChildren(localGo);
                    }, 90);
            }


            // TODO: Detect duplicate folders.

            var root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
            var path = GetAssetPathForPrefabInstance(root);
            if (path != null && path.Length > 0 && path.Contains("Assets/MIU Level Assets/Gameplay Prefabs/") && !CheckGoInFolder(go, gameplayFolder))
            {
                FixNote fix = null;
                if (gameplayFolder)
                {
                    fix = delegate ()
                    {
                        root.transform.SetParent(gameplayFolder.transform);
                    };
                }

                AddNote(Category.Gameplay, root, "All gameplay prefabs must be in Gameplay folder! (" + path + ")", fix, 80);
            }


            // Further checks.
            CheckReflectionProbes(go);
            CheckMeshes(go);
            CheckLights(staticFolder, go);
            CheckGems(go);
            CheckIdentifiers(go);
        }

        if (rootCamera == null)
        {
            AddNote(Category.Hierarchy, null, "A root Camera tagged MainCamera should exist", delegate ()
            {
                var prefab = AssetDatabase.LoadAssetAtPath<Camera>("Assets/Prefabs/Internal/Level Camera.prefab");
                var cam = Instantiate<Camera>(prefab);
                cam.name = "Level Camera";
            });
        }

        // We do want level bounds.
        if (GameObject.Find("LevelBounds") == null)
            AddNote(Category.Gameplay, null, "Could not find LevelBounds object. Marble will not respawn", null, 90);

        if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
        {
            AddNote(Category.Render, null, "Lighting should be generated on demand, not automatically (Auto Generate is checked)", delegate ()
            {
                Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
            });
        }

        CheckImport(dynamicFolder, gameplayFolder);

        // Check that the static folder is setup properly.
        CheckStatics(staticFolder);
        CheckDynamics(dynamicFolder);

        CheckProBuilder(staticFolder, dynamicFolder);

        // And the gameplay folder.
        CheckGameplay(gameplayFolder);

        lastValidated = getCurrentSceneResult();

        // ValidatePlaymodeBegin();
    }

    private void CheckIdentifiers(GameObject gameObject)
    {
        var allComponents = gameObject.GetComponentsInChildren<Component>(true);
        foreach (var component in allComponents) CheckIdentifiers(component);
    }

    private void CheckIdentifiers(Component component)
    {
        if (component == null) return;
        /*if (Identificator.HasIdentifier(component.GetType()))
        {
            // TODO: check duplicate identifiers
            var identifier = Identificator.GetIdentifier(component);
            var nonSerialized = System.Reflection.CustomAttributeExtensions.GetCustomAttributes<NonSerializedAttribute>(identifier.Field);
            if (!identifier.IsSet && nonSerialized == null)
            {
                AddNote(Category.General, component.gameObject, "AutoIdentifier on " + identifier.Field.DeclaringType + " not set for object.", delegate ()
                {
                    Identificator.Identify(component);
                });
            }
        }*/
    }

    private Camera CheckCamera(GameObject go)
    {
        var cam = go.GetComponent<Camera>();
        if (cam != null)
        {
            if (cam.tag != "MainCamera")
            {
                AddNote(Category.Hierarchy, go, "Root Camera should be tagged as a MainCamera", delegate ()
                {
                    cam.tag = "MainCamera";
                });
            }
        }
        return cam;
    }

    private void CheckPrefabModifications(Category category, GameObject root, Dictionary<string, string[]> allowedPerPrefabNameChanges)
    {

        // Get all the root prefab instances.
        var prefabRoots = new List<GameObject>();
        foreach (var transform in root.GetComponentsInChildren<Transform>())
        {
            if(transform == root.transform)
                continue;
                
            var rootPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(transform.gameObject);
            if (category == Category.Gameplay && !rootPrefab)
            {
                AddNote(Category.Gameplay, transform.gameObject, "All Gameplay objects should be prefabs!", null, 70);
                continue;
            }

            if (prefabRoots.Contains(rootPrefab))
                continue;

            prefabRoots.Add(rootPrefab);
        }

        string[] allowedPrefabChanges = {
            "m_RootOrder",
            "m_Name",
            "m_LocalRotation.x",
            "m_LocalRotation.y",
            "m_LocalRotation.z",
            "m_LocalRotation.w",
            "m_LocalPosition.x",
            "m_LocalPosition.y",
            "m_LocalPosition.z",
            "m_LocalEulerAnglesHint.x",
            "m_LocalEulerAnglesHint.y",
            "m_LocalEulerAnglesHint.z",
            "Id",
            "HideTime",
            "ShowTime",
            "TeamOwnership",
            "TeamSpawn",
            "isBlueGem",
            "GemGroups"
        };

        foreach (var prefabRoot in prefabRoots)
        {
            var path = GetAssetPathForPrefabInstance(prefabRoot);

            if (category == Category.Gameplay &&
                path.Length > 0 &&
                !path.Contains("Assets/Mayhem Level Assets/Gameplay Prefabs"))
                AddNote(Category.Gameplay, prefabRoot, "All Gameplay objects should be a prefab from Assets/Mayhem Level Assets/Gameplay Prefabs! Saw asset '" + path + "'", null, 50);

            var mods = PrefabUtility.GetPropertyModifications(prefabRoot);
            if (mods == null)
                continue;

            // Limited accidental modifications of prefabs.
            foreach (var p in mods)
            {
                bool allowed = false;

                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabRoot);
                var messageObject = "";
                string[] allowedChanges;
                if (prefab != null && allowedPerPrefabNameChanges.TryGetValue(prefab.name, out allowedChanges))
                {
                    messageObject = "on " + prefab.name;
                    if (!allowed && allowedChanges.Contains(p.propertyPath)) allowed = true;
                }
                if (!allowed && allowedPrefabChanges.Contains(p.propertyPath)) allowed = true;

                // We special case gates and scale.
                if(p.propertyPath.IndexOf("Scale") >= 0 && prefab.name.IndexOf("Gate") > 0) allowed = true;
                
                if (allowed) continue;

                var localP = p;
                var localRoot = prefabRoot;

                AddNote(Category.Gameplay, prefabRoot, "Prefab property '" + p.propertyPath + "' was modified inappropriately " + messageObject, delegate ()
                {

                    // Fix is to remove this property.
                    var itemMods = PrefabUtility.GetPropertyModifications(localRoot);
                    var outList = new List<PropertyModification>();

                    foreach (var i in itemMods)
                        if (i.propertyPath != localP.propertyPath)
                            outList.Add(i);

                    PrefabUtility.SetPropertyModifications(localRoot, outList.ToArray());
                }, 60);
            }
        }
    }

    private void CheckGameplay(GameObject gameplayFolder)
    {
        if (!gameplayFolder || gameplayFolder.transform == null)
            return;

        // Ensure that items with gameplay components are in gameplay folder.
        if (gameplayFolder.transform.Find("StartPad") == null)
            AddNote(Category.Gameplay, null, "No StartPad found - please add a StartPad", null, 80);

        if (gameplayFolder.transform.Find("EndPad") == null)
            AddNote(Category.Gameplay, null, "No EndPad found - please add an EndPad", null, 80);

        if (gameplayFolder.transform.Find("LevelBounds") == null)
            AddNote(Category.Gameplay, null, "No LevelBounds found - please add a LevelBounds", null, 80);

        var allowedPerPrefabNameChanges = new Dictionary<string, string[]>
        {
            {
                "TutorialMessage",
                new string[]{
                    "message",
                    "graphic",
                    "ShowOnce",
                    // Box Collider
                    "m_Size.x",
                    "m_Size.y",
                    "m_Size.z",
                    "m_Center.x",
                    "m_Center.y",
                    "m_Center.z",
                }
            },
            {
                "LevelBounds",
                new string[]{
                    "m_RootOrder",
                    "m_Name",
                    "m_LocalRotation.w",
                    "m_LocalPosition.x",
                    "m_LocalPosition.y",
                    "m_LocalPosition.z",
                    "m_LocalScale.x",
                    "m_LocalScale.y",
                    "m_localScale.z",
                    // Box Collider
                    "m_Size.x",
                    "m_Size.y",
                    "m_Size.z",
                    "m_Center.x",
                    "m_Center.y",
                    "m_Center.z",
                }
            },
            {
                "CheckPoint",
                new string[]{
                    // Box Collider
                    "m_Size.x",
                    "m_Size.y",
                    "m_Size.z",
                    "m_Center.x",
                    "m_Center.y",
                    "m_Center.z",
                    "Order",
                }
            },
            {
                "Goal",
                new string[]{
                    // Box Collider
                    "m_Size.x",
                    "m_Size.y",
                    "m_Size.z",
                    "m_Center.x",
                    "m_Center.y",
                    "m_Center.z",
                    "Order",
                }
            },
            {
                "_TeamTiles",
                new string[]{
                    "displayType",
                    "UseColorTint",
                    "SelfTiles.Array.size",
                    "SelfTiles.Array.data[0]",
                    "SelfTiles.Array.data[1]",
                    "OpponentTiles.Array.size",
                    "OpponentTiles.Array.data[0]",
                    "OpponentTiles.Array.data[1]",
                }
            },
            {
                "MP_Gem_1",
                new string[]{
                    "GemGroups.Array.size",
                    "GemGroups.Array.data[0]",
                    "GemGroups.Array.data[1]",
                    "GemGroups.Array.data[2]",
                    "GemGroups.Array.data[3]",
                    "GemGroups.Array.data[4]",
                    "GemGroups.Array.data[5]",
                    "GemGroups.Array.data[6]",
                    "GemGroups.Array.data[7]",
                    "GemGroups.Array.data[8]",
                    "GemGroups.Array.data[9]",
                }
            },
            {
                "MP_Gem_2",
                new string[]{
                    "GemGroups.Array.size",
                    "GemGroups.Array.data[0]",
                    "GemGroups.Array.data[1]",
                    "GemGroups.Array.data[2]",
                    "GemGroups.Array.data[3]",
                    "GemGroups.Array.data[4]",
                    "GemGroups.Array.data[5]",
                    "GemGroups.Array.data[6]",
                    "GemGroups.Array.data[7]",
                    "GemGroups.Array.data[8]",
                    "GemGroups.Array.data[9]",
                }
            },
            {
                "MP_Gem_5",
                new string[]{
                    "GemGroups.Array.size",
                    "GemGroups.Array.data[0]",
                    "GemGroups.Array.data[1]",
                    "GemGroups.Array.data[2]",
                    "GemGroups.Array.data[3]",
                    "GemGroups.Array.data[4]",
                    "GemGroups.Array.data[5]",
                    "GemGroups.Array.data[6]",
                    "GemGroups.Array.data[7]",
                    "GemGroups.Array.data[8]",
                    "GemGroups.Array.data[9]",
                }
            }
        };

        CheckPrefabModifications(Category.Gameplay, gameplayFolder, allowedPerPrefabNameChanges);
    }

    private void CheckImport(GameObject dynamicFolder, GameObject gameplayFolder)
    {
        var prefabs = new Dictionary<string, GameObject>
        {
            { "StartPad", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsStartPad.prefab") },
            { "EndPad", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsEndPad.prefab") },
            { "Easter Egg", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsEaster Egg.prefab") },
            { "Gem", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsGem.prefab") },
            { "CheckPoint", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsCheckPoint.prefab") },
            { "Tutorial", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsTutorialMessage.prefab") },
            { "LevelBounds", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsLevelBounds.prefab") },

            { "Basher", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsHazards/Basher.prefab") },
            { "Bumper", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsHazards/Bumper.prefab") },
            { "DestructorPit", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsHazards/DestructorPit.prefab") },

            { "MP_Gem_1", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsMultiplayer/MP_Gem_1.prefab") },
            { "MP_Gem_2", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsMultiplayer/MP_Gem_2.prefab") },
            { "MP_Gem_5", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsMultiplayer/MP_Gem_5.prefab") },
            { "SpawnPoint", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsMultiplayer/SpawnPoint.prefab") },

            { "Boost", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsPowerups/Boost.prefab") },
            { "Featherfall", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsPowerups/Featherfall.prefab") },
            { "Jump", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsPowerups/Jump.prefab") },
            { "TimeTravel", AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mayhem Level Assets/Gameplay PrefabsPowerups/TimeTravel.prefab") }
        };

        if (dynamicFolder != null)
        {
            foreach (Transform child in dynamicFolder.transform)
            {
                CheckImportObject(child.gameObject, prefabs);
            }
        }

        if (gameplayFolder != null)
        {
            foreach (Transform child in gameplayFolder.transform)
            {
                CheckImportObject(child.gameObject, prefabs);
            }
        }

#if false
        // Design mode level serialization deals with this.
        var splineDrawers = GameObject.FindObjectsOfType<SplineDrawer>();
        foreach (var splineDrawer in splineDrawers)
        {
            var spline = splineDrawer.gameObject;
            AddNote(Category.Import, splineDrawer,
                "Spline Drawer (level kit) should be converted to a Spline hierarchy: " + spline.name,
                delegate ()
                {
                    var splineTrans = spline.transform;
                    var controlPointCount = splineTrans.childCount;

                    var points = new List<Vector3>();
                    var settings = new List<Battlehub.SplineEditor.ControlPointSetting>();
                    var modes = new List<Battlehub.SplineEditor.ControlPointMode>();
                    for (var i = 0; i < controlPointCount; i++)
                    {
                        var child = splineTrans.GetChild(i);
                        points.Add(child.localPosition);
                        Debug.Log(i + " " + child.localPosition);
                        settings.Add(new Battlehub.SplineEditor.ControlPointSetting());
                        modes.Add(Battlehub.SplineEditor.ControlPointMode.Free);
                    }

                    for (var i = controlPointCount - 1; i >= 0; i--)
                    {
                        DestroyImmediate(splineTrans.GetChild(i).gameObject);
                    }

                    var snapshot = new Battlehub.SplineEditor.SplineSnapshot(
                        points.ToArray(),
                        settings.ToArray(),
                        modes.ToArray(),
                        false
                    );

                    var comp = spline.AddComponent<Battlehub.SplineEditor.Spline>();
                    comp.Load(snapshot);

                    DestroyImmediate(splineDrawer);
                }
            );
        }
#endif
    }

    private void CheckImportObject(GameObject imported, Dictionary<string, GameObject> prefabs)
    {
        if (PrefabUtility.IsAnyPrefabInstanceRoot(imported)) return;
        Transform importedTrans = imported.transform;
        GameObject prefab;
        if (prefabs.TryGetValue(imported.name, out prefab))
        {
            AddNote(Category.Import, imported,
                "Game object not linked to a prefab: " + imported.name,
                delegate ()
                {
                    var obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    obj.name = imported.name;
                    obj.transform.localPosition = importedTrans.localPosition;
                    obj.transform.localRotation = importedTrans.localRotation;
                    obj.transform.localScale = importedTrans.localScale;
                    obj.transform.SetParent(importedTrans.parent);
                    obj.transform.SetSiblingIndex(importedTrans.GetSiblingIndex());

                    var components = imported.GetComponents<Component>();
                    foreach (var comp in components)
                    {
                        if (comp == null) continue;
                        var compType = comp.GetType();
                        var objComp = obj.GetComponent(compType);
                        if (objComp == null) continue;
                        CopyFields(comp, objComp, compType);
                    }

                    DestroyImmediate(imported);
                }
            );
        }

    }

    private void CopyFields(object source, object target, Type type)
    {

        var bindingFlags =
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance;

        var fields = type.GetFields(bindingFlags);
        foreach (var field in fields)
        {
            var value = field.GetValue(source);
            field.SetValue(target, value);
        }

        var props = type.GetProperties(bindingFlags);
        foreach (var prop in props)
        {
            if (!prop.CanWrite) continue;
            var value = prop.GetValue(source, null);
            prop.SetValue(target, value, null);
        }
    }

    private void CheckStatics(GameObject staticFolder)
    {
        // Find everything in the Static folder and ensure it is static.
        if (!staticFolder)
            return;

        Type[] allowedNonStaticFlagComponents = { };

        foreach (var staticXfrm in staticFolder.GetComponentsInChildren<Transform>())
        {
            List<Component> components = new List<Component>();

            var skipFlags = false;
            if (allowedNonStaticFlagComponents.Length > 0)
            {
                staticXfrm.gameObject.GetComponents(components);
                foreach (var component in components)
                {
                    foreach (var allowedType in allowedNonStaticFlagComponents)
                    {
                        if (component.GetType() == allowedType) skipFlags = true;
                        if (skipFlags) break;
                    }
                    if (skipFlags) break;
                }
            }

            if (!skipFlags)
            {
                var staticFlags = new Dictionary<StaticEditorFlags, string>
                {
                    { StaticEditorFlags.OccluderStatic, "Occluder Static" },
                    { StaticEditorFlags.OccludeeStatic, "Occludee Static" },
                    { StaticEditorFlags.BatchingStatic, "Batching Static" },
                    { StaticEditorFlags.NavigationStatic, "Navigation Static" },
                    { StaticEditorFlags.OffMeshLinkGeneration, "Off Mesh Link Generation" },
                    { StaticEditorFlags.ReflectionProbeStatic, "Reflection Probe Static" },
                    { StaticEditorFlags.ContributeGI, "Lightmap Static" },
                };

                foreach (var staticFlag in staticFlags)
                {
                    if (!GameObjectUtility.AreStaticEditorFlagsSet(staticXfrm.gameObject, staticFlag.Key))
                    {
                        AddNote(Category.Static, staticXfrm.gameObject, "Static object must have " + staticFlag.Value + " set", delegate ()
                        {
                            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(staticXfrm.gameObject);
                            GameObjectUtility.SetStaticEditorFlags(staticXfrm.gameObject, flags | staticFlag.Key);
                        });
                    }
                }
            }


            var renderer = staticXfrm.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (!renderer.receiveShadows)
                {
                    AddNote(Category.Static, staticXfrm.gameObject, "Static object should receive shadows", delegate ()
                    {
                        renderer.receiveShadows = true;
                    });
                }
                if (renderer.shadowCastingMode != ShadowCastingMode.On)
                {
                    AddNote(Category.Static, staticXfrm.gameObject, "Static object should have Cast Shadows set to On", delegate ()
                    {
                        renderer.shadowCastingMode = ShadowCastingMode.On;
                    });
                }
            }
        }

        // Check for a sun light.
        var sunXfrm = staticFolder.transform.FindDeepChild("Sun");
        if (sunXfrm == null)
        {
            // Try to find a directional light we can make the sun.
            FixNote fix = null;

            Light sunLight = null;
            foreach (var l in Light.GetLights(LightType.Directional, 0xFFFFFFF))
            {
                Debug.Log("Saw light " + l.gameObject.name + " type=" + l.type);
                if (l.type != LightType.Directional)
                    continue;

                sunLight = l;
                break;
            }

            if (sunLight)
                fix = delegate ()
                {
                    sunLight.gameObject.name = "Sun";
                };

            AddNote(Category.Static, null, "Could not find static light called 'Sun'!", fix, 90);
        }
        else
        {
            var sunGo = sunXfrm.gameObject;
            var sunPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MIU Internal/Game Internal/Sun.prefab");
            var sunGoPrefab = PrefabUtility.GetCorrespondingObjectFromSource(sunGo);
            if (sunGoPrefab != sunPrefab)
            {
                AddNote(Category.Static, sunGo, "Sun is not an instance of the Sun prefab", delegate ()
                {
                    var trans = sunGo.transform;
                    var pos = trans.localPosition;
                    var rot = trans.localRotation;
                    var scale = trans.localScale;
                    
                    // This loses some state which is not ideal.
                    //sunGo = PrefabUtility.ConnectGameObjectToPrefab(sunGo, sunPrefab);
                    Destroy(sunGo);
                    sunGo = Instantiate(sunPrefab);

                    trans = sunGo.transform;
                    trans.localPosition = pos;
                    trans.localRotation = rot;
                    trans.localScale = scale;
                }, 60);
            }
            else
            {
                var allowedPerPrefabNameChanges = new Dictionary<string, string[]> { };
                CheckPrefabModifications(Category.Static, sunGo, allowedPerPrefabNameChanges);
            }

            var sunLight = sunGo.GetComponent<Light>();
            if (sunLight == null)
            {
                AddNote(Category.Static, sunGo, "Sun does not contain a Light component");
            }
            else
            {
                if (RenderSettings.sun != sunLight)
                {
                    AddNote(Category.Render, RenderSettings.sun, "Sun not set as scene Sun Source", delegate ()
                    {
                        RenderSettings.sun = sunLight;
                    });
                }

                if (RenderSettings.defaultReflectionMode != DefaultReflectionMode.Custom)
                {
                    AddNote(Category.Render, null, "Environment Reflection Source should be Custom", delegate ()
                    {
                        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
                    });
                }

                if (RenderSettings.customReflection == null)
                {
                    AddNote(Category.Render, null, "Environment Reflection Cubemap should be set");
                }

                if (LightmapEditorSettings.reflectionCubemapCompression != ReflectionCubemapCompression.Auto)
                {
                    AddNote(Category.Render, null, "Environment Reflection Compression should be Auto", delegate ()
                    {
                        LightmapEditorSettings.reflectionCubemapCompression = ReflectionCubemapCompression.Auto;
                    });
                }

                if (RenderSettings.reflectionIntensity != 0.75f)
                {
                    AddNote(Category.Render, null, "Environment Reflection Intensity Multiplier should be 0.75", delegate ()
                    {
                        RenderSettings.reflectionIntensity = 0.75f;
                    });
                }

                if (RenderSettings.reflectionBounces != 1)
                {
                    AddNote(Category.Render, null, "Environment Reflection Bounces should be 1", delegate ()
                    {
                        RenderSettings.reflectionBounces = 1;
                    });
                }

                if (Lightmapping.realtimeGI != false)
                {
                    AddNote(Category.Render, null, "Realtime Global Illumination should be disabled", delegate ()
                    {
                        Lightmapping.realtimeGI = false;
                    });
                }

                if (Lightmapping.bakedGI != true)
                {
                    AddNote(Category.Render, null, "Baked Global Illumination should be enabled", delegate ()
                    {
                        Lightmapping.bakedGI = true;
                    });
                }

                if (LightmapEditorSettings.lightmapper != LightmapEditorSettings.Lightmapper.Enlighten)
                {
                    AddNote(Category.Render, null, "Lightmapper should be set to Enlighten", delegate ()
                    {
                        LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.Enlighten;
                    });
                }

                if (LightmapEditorSettings.realtimeResolution != 2)
                {
                    AddNote(Category.Render, null, "Lightmapper Indirect Resolution should be 2", delegate ()
                    {
                        LightmapEditorSettings.realtimeResolution = 2;
                    });
                }

                if (LightmapEditorSettings.bakeResolution != 4)
                {
                    AddNote(Category.Render, null, "Lightmap Resolution should be 4", delegate ()
                    {
                        LightmapEditorSettings.bakeResolution = 4;
                    });
                }

                if (LightmapEditorSettings.padding != 2)
                {
                    AddNote(Category.Render, null, "Lightmap Padding should be 2", delegate ()
                    {
                        LightmapEditorSettings.padding = 2;
                    });
                }

                if (LightmapEditorSettings.maxAtlasSize != 4096 || LightmapEditorSettings.maxAtlasHeight != 4096)
                {
                    AddNote(Category.Render, null, "Lightmap Size should be 4096", delegate ()
                    {
                        LightmapEditorSettings.maxAtlasSize = LightmapEditorSettings.maxAtlasHeight = 4096;
                    });
                }

                if (LightmapEditorSettings.textureCompression != true)
                {
                    AddNote(Category.Render, null, "Lightmapper Compress Lightmaps should be enabled", delegate ()
                    {
                        LightmapEditorSettings.textureCompression = true;
                    });
                }

                if (LightmapEditorSettings.enableAmbientOcclusion != true)
                {
                    AddNote(Category.Render, null, "Lightmapper Ambient Occlusion should be enabled", delegate ()
                    {
                        LightmapEditorSettings.enableAmbientOcclusion = true;
                    });
                }

                if (LightmapEditorSettings.aoMaxDistance != 8)
                {
                    AddNote(Category.Render, null, "Lightmapper Ambient Occlusion Max Distance should be 8", delegate ()
                    {
                        LightmapEditorSettings.aoMaxDistance = 8;
                    });
                }

                if (LightmapEditorSettings.aoExponentIndirect != 4)
                {
                    AddNote(Category.Render, null, "Lightmapper Indirect Contribution should be 4", delegate ()
                    {
                        LightmapEditorSettings.aoExponentIndirect = 4;
                    });
                }

                if (LightmapEditorSettings.aoExponentDirect != 4)
                {
                    AddNote(Category.Render, null, "Lightmapper Direct Contribution should be 4", delegate ()
                    {
                        LightmapEditorSettings.aoExponentDirect = 4;
                    });
                }

                if (LightmapEditorSettings.lightmapsMode != LightmapsMode.CombinedDirectional)
                {
                    AddNote(Category.Render, null, "Lightmapper Directional Mode should be Directional", delegate ()
                    {
                        LightmapEditorSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
                    });
                }

            }
        }
    }

    private void CheckDynamics(GameObject dynamicFolder)
    {
        if (dynamicFolder == null) return;
        var renderers = dynamicFolder.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            var go = renderer.gameObject;

            if (renderer.receiveShadows)
            {
                AddNote(Category.Dynamic, go, "Dynamic object should not receive shadows", delegate ()
                {
                    renderer.receiveShadows = false;
                });
            }
            if (renderer.shadowCastingMode != ShadowCastingMode.Off)
            {
                AddNote(Category.Dynamic, go, "Dynamic object should not cast shadows", delegate ()
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                });
            }

            var staticFlagsDisabled = new Dictionary<StaticEditorFlags, string>
            {
                { StaticEditorFlags.ContributeGI, "Lightmap Static" },
            };

            foreach (var staticFlag in staticFlagsDisabled)
            {
                if (GameObjectUtility.AreStaticEditorFlagsSet(go, staticFlag.Key))
                {
                    AddNote(Category.Dynamic, go, "Dynamic object must not have " + staticFlag.Value + " set", delegate ()
                    {
                        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(go);
                        GameObjectUtility.SetStaticEditorFlags(go, flags & ~staticFlag.Key);
                    });
                }
            }
        }
    }

    private void CheckProBuilder(GameObject staticFolder, GameObject dynamicFolder)
    {
        var meshes = new List<UnityEngine.ProBuilder.ProBuilderMesh>();
        if (staticFolder) meshes = meshes.Concat(staticFolder.GetComponentsInChildren<UnityEngine.ProBuilder.ProBuilderMesh>()).ToList();
        if (dynamicFolder) meshes = meshes.Concat(dynamicFolder.GetComponentsInChildren<UnityEngine.ProBuilder.ProBuilderMesh>()).ToList();

        foreach (var mesh in meshes)
        {
            var go = mesh.gameObject;
            var collider = go.GetComponent<MeshCollider>();
            if (collider == null && false) // This causes too many false positives, disabled.
            {
                AddNote(Category.General, go, "ProBuilder Mesh object must have a Mesh Collider component", delegate ()
                {
                    go.AddComponent<MeshCollider>();
                });
            }
        }
    }

    private void CheckLights(GameObject staticFolder, GameObject go)
    {
        // And any lights.
        if (staticFolder)
        {
            var lightXfrm = staticFolder.transform.Find("Lighting");
            if (lightXfrm)
            {
                var refLights = new List<Light>();
                go.GetComponentsInChildren(refLights);
                foreach (var l in refLights)
                {
                    var localLight = l;
                    if (!CheckGoInFolder(l.gameObject, lightXfrm.gameObject))
                        AddNote(Category.Static, l.gameObject, "Light should be in Static/Lighting folder", lightXfrm != null ? delegate ()
                        {
                            localLight.transform.SetParent(lightXfrm);
                        }
                        : (FixNote)null, 80);
                }
            }
        }
    }

    private void CheckReflectionProbes(GameObject go)
    {
        // Also, check for any reflection probes.
        var refProbes = new List<ReflectionProbe>();
        go.GetComponentsInChildren(refProbes);
        foreach (var p in refProbes)
        {
            var localP = p;
            AddNote(Category.Render, p.gameObject, "Reflection probes are not supported, please remove", delegate ()
            {
                DestroyImmediate(localP.gameObject);
            }, 40);
        }
    }

    private void CheckMeshes(GameObject go)
    {
/*        var curvyMeshes = new List<FluffyUnderware.Curvy.Generator.Modules.CreateMesh>();
        go.GetComponentsInChildren<FluffyUnderware.Curvy.Generator.Modules.CreateMesh>(curvyMeshes);
        foreach (var mesh in curvyMeshes)
        {
            if (mesh.ReflectionProbes != ReflectionProbeUsage.Off)
            {
                AddNote(Category.Render, mesh.gameObject, "Reflection probes should be disabled on Curvy mesh", delegate ()
                {
                    mesh.ReflectionProbes = ReflectionProbeUsage.Off;
                });
            }
        }*/

        var meshes = new List<MeshRenderer>();
        go.GetComponentsInChildren<MeshRenderer>(meshes);
        foreach (var mesh in meshes)
        {
            if (mesh.reflectionProbeUsage != ReflectionProbeUsage.Off)
            {
                AddNote(Category.Render, mesh.gameObject, "Reflection probes should be disabled on mesh", delegate ()
                {
                    mesh.reflectionProbeUsage = ReflectionProbeUsage.Off;
                });
            }
        }
    }

    private void CheckGems(GameObject go)
    {
        var gems = GameObject.FindGameObjectsWithTag("Gem");
        foreach (var gem in gems)
        {
            Vector3 scale = gem.gameObject.transform.localScale;

            if(gem.gameObject.name.IndexOf("Gate") > 0)
                continue;

            if (scale.x != 1.0f || scale.y != 1.0f || scale.z != 1.0f)
            {
                AddNote(Category.Gameplay, gem.gameObject, "Gems should not be rescaled", delegate ()
                {
                    gem.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                });
            }
        }
    }

    static Texture2D blackTex = null;

    private Texture2D MakeTex(int width, int height, Color col)
    {
        if (blackTex != null)
            return blackTex;

        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        blackTex = new Texture2D(width, height);
        blackTex.SetPixels(pix);
        blackTex.Apply();
        return blackTex;
    }

    void Bake()
    {
        // Disable reflection probes on everything.
        var roots = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(roots);

        // Make static folder content static.
        var staticFolder = GameObject.Find("Static");
        if (staticFolder)
        {
            var allGos = SceneModeUtility.GetObjects(new UnityEngine.Object[1] { staticFolder }, true);
            foreach (var go in allGos)
            {
                GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic
                    | StaticEditorFlags.NavigationStatic
                    | StaticEditorFlags.OccludeeStatic
                    | StaticEditorFlags.OccluderStatic
                    | StaticEditorFlags.OffMeshLinkGeneration
                    | StaticEditorFlags.ReflectionProbeStatic);
            }
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        var bigLabel = new GUIStyle(GUI.skin.label);
        bigLabel.fontSize = 16;
        bigLabel.fontStyle = FontStyle.Bold;

        var bigButton = new GUIStyle(GUI.skin.button);
        bigButton.fontSize = bigLabel.fontSize;
        bigButton.fixedWidth = 150;
        bigButton.fixedHeight = 40;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        GUILayout.Label("Validate", bigLabel);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear", bigButton)) ClearResults();
        if (GUILayout.Button("Scene", bigButton)) ValidateScene();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        frameFixedErrorCount = 0;

        var statusStyle = new GUIStyle(GUI.skin.label);
        statusStyle.fontSize = 20;
        statusStyle.fontStyle = FontStyle.Bold;
        statusStyle.stretchWidth = false;

        var totalNoteCount = 0;
        if(getCurrentSceneResult() != null) totalNoteCount = getCurrentSceneResult().containedErrors;

        if (totalNoteCount == 0)
        {
            statusStyle.normal.textColor = Color.green;
            statusStyle.normal.background = MakeTex(2, 2, new Color(0.0223529f, 0.366667f, 0.056863f));
            statusStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("OK", statusStyle);
        }
        else
        {
            statusStyle.normal.textColor = Color.red;
            statusStyle.normal.background = MakeTex(2, 2, Color.black);
            statusStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("FAIL (" + totalNoteCount + ")", statusStyle);
        }

        // Render lists in a scroll box.
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Validate scene if changed externally
        var scene = getCurrentSceneResult();
        if (scene != lastValidated)
            Validate();

        if(scene != null)
        {
            var results = scene.subgroups;
            foreach (var g in results)
                RenderResultGroup(g);
        }

        EditorGUILayout.EndScrollView();

        if (frameFixedErrorCount > 0)
        {
            Validate();
        }
    }

    GUIStyle GetInlineButtonStyle()
    {
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.margin.left = (int)EditorGUI.IndentedRect(new Rect()).x;
        buttonStyle.fontSize = 12;
        return buttonStyle;
    }

    GUIStyle GetNonIndentedInlineButtonStyle()
    {
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 12;
        return buttonStyle;
    }

    void FixResultGroup(ResultGroup group)
    {
        foreach (var note in group.notes)
        {
            FixResultNote(note);
        }

        foreach (var subgroup in group.subgroups)
        {
            FixResultGroup(subgroup);
        }

    }

    void SetShowResultGroup(ResultGroup group, bool show)
    {
        group.showDefault = show;
        SetShowResultSubgroups(group, show);
    }

    void SetShowResultSubgroups(ResultGroup group, bool show)
    {
        foreach (var subgroup in group.subgroups)
        {
            SetShowResultGroup(subgroup, show);
        }
    }

    void ShowResultGroup(ResultGroup group)
    {
        SetShowResultGroup(group, true);
    }
    void ShowResultSubgroups(ResultGroup group)
    {
        SetShowResultSubgroups(group, true);
    }

    void HideResultGroup(ResultGroup group)
    {
        SetShowResultGroup(group, false);
    }
    void HideResultSubgroups(ResultGroup group)
    {
        SetShowResultSubgroups(group, false);
    }

    void FixResultNote(ResultNote note)
    {
        if (note.fix == null || note.wasFixed) return;
        try
        {
            var undoMessage = "Fix for \"" + note.error + "\"";
            if (note.obj != null)
            {
                var allObjects = EditorUtility.CollectDeepHierarchy(new UnityEngine.Object[] { note.obj });
                allObjects = allObjects.Where(obj => obj != null).ToArray();
                Undo.RecordObjects(allObjects, undoMessage);
            }
            else
            {
                // TODO: Do we know at this point what we will touch?
                Undo.RegisterSceneUndo(undoMessage);
            }
            note.fix();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            note.wasFixed = true;
            frameFixedErrorCount++;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to apply fix for '" + note.error + "' due to: " + e.ToString());
        }
    }

    void RenderResultGroup(ResultGroup group, int level = 0)
    {
        if(group == null)
            return;

        var style = new GUIStyle(EditorStyles.foldout);
        var color = group.containedErrors == 0 ?
            new Color(0.0223529f, 0.366667f, 0.056863f) :
            level > 1 ? EditorStyles.foldout.normal.textColor : new Color(0.837255f, 0.243137f, 0.196078f);


        var scene = group as SceneResult;
        if (scene != null)
        {
            var currentScene = getCurrentSceneResult();
            var sceneOpen =
                currentScene != null &&
                (group == currentScene || group.parent == currentScene);
            if (sceneOpen)
            {
                style.fontStyle = FontStyle.Bold;
            }
        }

        style.normal.textColor = color;
        style.onNormal.textColor = color;
        style.hover.textColor = color;
        style.onHover.textColor = color;
        style.focused.textColor = color;
        style.onFocused.textColor = color;
        style.active.textColor = color;
        style.onActive.textColor = color;

        int[] fontSizeByLevel = { 16, 14, 12 };

        style.fontSize = fontSizeByLevel[Math.Min(level, fontSizeByLevel.Length - 1)];

        var foldoutName =
            group.name +
            (group.subgroups.Count > 0 ?
                " (" + group.containedUniqueErrors + " unique, " + group.containedErrors + " case" + (group.containedErrors == 1 ? "" : "s") + ")" :
                " (" + group.containedErrors + " case" + (group.containedErrors == 1 ? "" : "s") + ")"
            );

        var showState = group.showChanged ? group.show : group.showDefault;
        group.show = EditorGUILayout.Foldout(showState, foldoutName, true, style);
        if (!group.showChanged)
        {
            group.showChanged = showState != group.show;
        }

        EditorGUILayout.Space();

        if (group.show)
        {
            EditorGUI.indentLevel++;

            var openAndValidateButton = scene != null;
            var fixAllButton = group.containedErrors > 0;
            var collapseExpandButtons = group.subgroups.Count > 0;
            if (openAndValidateButton || fixAllButton || collapseExpandButtons)
            {
                EditorGUILayout.BeginHorizontal();

                if (fixAllButton && GUILayout.Button("Fix All", GetInlineButtonStyle(), GUILayout.MaxWidth(60)))
                {
                    FixResultGroup(group);
                }

                if (collapseExpandButtons)
                {
                    if (GUILayout.Button(new GUIContent("-▲-", "Collapse All"), GetInlineButtonStyle(), GUILayout.MaxWidth(30)))
                    {
                        HideResultSubgroups(group);
                    }
                    if (GUILayout.Button(new GUIContent("-▼-", "Expand all"), GetNonIndentedInlineButtonStyle(), GUILayout.MaxWidth(30)))
                    {
                        ShowResultSubgroups(group);
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel++;
            RenderResultNotes(group.notes);
            EditorGUI.indentLevel--;

            foreach(var g in group.subgroups)
                RenderResultGroup(g, level + 1);

            EditorGUI.indentLevel--;
        }

    }

    void RenderResultNotes(List<ResultNote> notes)
    {
        var toRemove = new List<ResultNote>();

        var noteWidth = 120;
        var noteHeight = 20;

        var fixWidth = 30;
        var fixSpace = 2;
        var nameSpace = 0;
        var nameWidth = noteWidth - fixWidth - fixSpace - nameSpace;

        var nameStyle = new GUIStyle(EditorStyles.miniButton);
        nameStyle.alignment = TextAnchor.MiddleLeft;
        nameStyle.fixedWidth = nameWidth;
        nameStyle.fixedHeight = noteHeight;

        var nameStyleLeft = new GUIStyle(EditorStyles.miniButtonLeft);
        nameStyleLeft.alignment = nameStyle.alignment;
        nameStyleLeft.fixedWidth = nameStyle.fixedWidth;
        nameStyleLeft.fixedHeight = nameStyle.fixedHeight;

        var fixStyle = new GUIStyle(EditorStyles.miniButton);
        fixStyle.alignment = TextAnchor.MiddleCenter;
        fixStyle.fixedWidth = fixWidth;
        fixStyle.fixedHeight = noteHeight;

        var fixStyleRight = new GUIStyle(EditorStyles.miniButtonRight);
        fixStyleRight.alignment = TextAnchor.MiddleCenter;
        fixStyleRight.fixedWidth = fixWidth;
        fixStyleRight.fixedHeight = noteHeight;

        var indent = EditorGUI.IndentedRect(new Rect());

        var lineWidth = this.position.width - indent.x - 20f;
        int notesPerLine = (int)lineWidth / noteWidth;

        Rect line = indent;

        int col = 0;
        int row = 0;

        foreach (var n in notes)
        {
            if (col == 0)
            {
                line = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, noteHeight));
            }
            var name = n.name != "" ? n.name : n.path;
            var rect = new Rect(line.x + col * noteWidth, line.y, noteWidth, noteHeight);
            var nameButton = name != "";
            var fixButton = n.fix != null && !n.wasFixed;

            var content = new GUIContent();
            content.text = name;
            if (name != null && name.Length > 10)
            {
                content.tooltip = name;
            }

            var nameRect = new Rect(rect.x, rect.y, nameWidth, rect.height);
            var fixRect = new Rect(rect.x + (nameButton ? nameWidth + nameSpace : 0), rect.y, fixWidth, rect.height);

            if (nameButton && GUI.Button(nameRect, content, fixButton ? nameStyleLeft : nameStyle))
            {
                if (n.obj != null)
                {
                    EditorGUIUtility.PingObject(n.go);
                }
                if (n.go != null)
                {
                    Selection.activeGameObject = n.go;
                    if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.pivot = n.go.transform.position;
                }
            }

            if (fixButton && GUI.Button(fixRect, "Fix", nameButton ? fixStyleRight : fixStyle))
            {
                FixResultNote(n);
                if (n.obj != null)
                {
                    EditorGUIUtility.PingObject(n.obj);
                }
            }

            col++;
            if (col >= notesPerLine)
            {
                col = 0;
                row++;
            }
        }
        EditorGUILayout.Space();

        // Defer delete to avoid breaking iteration above.
        foreach (var n in toRemove)
            notes.Remove(n);
    }

    [MenuItem("Tools/MIU Level Validator")]
    static void Validator()
    {
        var w = GetWindow<MarbleValidator>();
        w.ShowTab();
        w.ValidateScene();
    }
}

