using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Object = UnityEngine.Object;

public class ObjectCreator : EditorWindow
{
    #region GUI

    private const float ICON_SIZE = 50f;

    Vector2 scrollPos;

    // element styles
    GUIStyle bigLabel;
    GUIStyle smallLabel;
    GUIStyle bigButton;
    GUIStyle smallButton;
    GUIStyle styleIconButton;

    // foldouts
    bool showCore;
    bool showPowerups;
    bool showGates;
    bool showHazards;
    bool showSigns;

    private void OnGUI()
    {
        SetupStyle();

        // Draw UI
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();
        if(IsReadyForUI())
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0), GUILayout.MaxWidth(9999));
            GUILayout.Label("Prefabs", bigLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0), GUILayout.MaxWidth(9999));
            GUILayout.EndHorizontal();

            showCore = EditorGUILayout.Foldout(showCore, "Core");
            if (showCore)
                CoreUI();

            showPowerups = EditorGUILayout.Foldout(showPowerups, "Powerups");
            if (showPowerups)
                PowerupsUI();

            showGates = EditorGUILayout.Foldout(showGates, "Gates");
            if (showGates)
                GatesUI();

            showHazards = EditorGUILayout.Foldout(showHazards, "Hazards");
            if (showHazards)
                HazardsUI();
            
            showSigns = EditorGUILayout.Foldout(showSigns, "Signs");
            if (showSigns)
                SignsUI();

            // Level Exporter
            EditorGUILayout.Space(10);
            MapExporter.RunGUI();
        }
        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    bool IsReadyForUI()
    {
        bool lightSet = true;
        if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand || Lightmapping.realtimeGI != false || Lightmapping.bakedGI != true)
            lightSet = false;
        bool readyForParts = lightSet;

        if(!readyForParts)
        {
            GUILayout.Label("Scene Setup", bigLabel);
            if (!lightSet) { if (GUILayout.Button("Setup Lighting", bigButton)) SetupLighting(); }
        }

        return readyForParts;
    }

    void CoreUIOld()
    {
        GUILayout.Space(5);
        if (MapComponents.FindFixed("StartPad") == null)
        { if (FoldoutButton("Start Pad", 1)) CreateStartPad(); }
        else
            FoldoutLabel("<color=green>Start Pad in Scene.</color>", 1);

        if (MapComponents.FindFixed("EndPad") == null)
        { if (FoldoutButton("End Pad", 1)) CreateEndPad(); }
        else
            FoldoutLabel("<color=green>End Pad in Scene.</color>", 1);

        if (MapComponents.FindFixed("LevelBounds") == null)
        { if (FoldoutButton("Level Bounds", 1)) CreateLvlBounds(); }
        else
        {
            if (FoldoutButton("Recalculate Level Bounds", 1)) UpdateLevelBounds();
        }

        if (FindObjectOfType<LevelTiming>() == null)
        { if (FoldoutButton("Level Timing", 1)) CreateTimingObj(); }
        else
            FoldoutLabel("<color=green>Level Timing in Scene.</color>", 1);
        GUILayout.Space(5);
    }

    void CoreUI()
    {
        // core
        GUILayout.BeginHorizontal();
        if (IconButton(content_startpad)) CreateSS();
        if (IconButton(content_checkpoint)) CreateSJ();
        if (IconButton(content_endpad)) CreateFF();
        GUILayout.EndHorizontal();
    }

    void PowerupsUI()
    {
        // powerups
        GUILayout.BeginHorizontal();
        if (IconButton(content_boost)) CreatePrefab(BOOST, "Gameplay");
        if (IconButton(content_jump)) CreatePrefab(JUMP, "Gameplay");
        if (IconButton(content_featherfall)) CreatePrefab(FEATHER_FALL, "Gameplay");
        if (IconButton(content_megamarble)) CreatePrefab(MEGA_MARBLE, "Gameplay");
        if (IconButton(content_timetravel)) CreatePrefab(TIME_TRAVEL, "Gameplay");
        GUILayout.EndHorizontal();
    }

    void GatesUI()
    {
        // gates
        GUILayout.BeginHorizontal();
        if (IconButton(content_gate_boost)) CreatePrefab(GATE_BOOST, "Gameplay");
        if (IconButton(content_gate_jump)) CreatePrefab(GATE_JUMP, "Gameplay");
        if (IconButton(content_gate_featherfall)) CreatePrefab(GATE_FEATHER_FALL, "Gameplay");
        if (IconButton(content_gate_gem)) CreatePrefab(GATE_GEM, "Gameplay");
        if (IconButton(content_gate_timetravel)) CreatePrefab(GATE_TIME_TRAVEL, "Gameplay");
        GUILayout.EndHorizontal();

        // ring gates
        GUILayout.BeginHorizontal();
        if (IconButton(content_ring_boost)) CreatePrefab(RING_BOOST, "Gameplay");
        if (IconButton(content_ring_jump)) CreatePrefab(RING_JUMP, "Gameplay");
        if (IconButton(content_ring_featherfall)) CreatePrefab(RING_FEATHER_FALL, "Gameplay");
        if (IconButton(content_ring_gem)) CreatePrefab(RING_GEM, "Gameplay");
        if (IconButton(content_ring_timetravel)) CreatePrefab(RING_TIME_TRAVEL, "Gameplay");
        GUILayout.EndHorizontal();
    }

    void HazardsUI()
    {
        // hazards
        GUILayout.BeginHorizontal();
        if (IconButton(content_basher)) CreateSS();
        if (IconButton(content_bumper)) CreateSJ();
        GUILayout.EndHorizontal();
    }

    void SignsUI()
    {
        // signs
        GUILayout.BeginHorizontal();
        if (IconButton(content_sign_continuous)) CreatePrefab(SIGN_CONTINUOUS, "Skybox");
        if (IconButton(content_sign_curvy)) CreatePrefab(SIGN_CURVY, "Skybox");
        if (IconButton(content_sign_dropoff)) CreatePrefab(SIGN_DROPOFF, "Skybox");
        if (IconButton(content_sign_fork)) CreatePrefab(SIGN_FORK, "Skybox");
        if (IconButton(content_sign_steep)) CreatePrefab(SIGN_STEEP, "Skybox");
        if (IconButton(content_sign_turnleft)) CreatePrefab(SIGN_TURNLEFT, "Skybox");
        if (IconButton(content_sign_turnright)) CreatePrefab(SIGN_TURNRIGHT, "Skybox");
        if (IconButton(content_sign_hazard)) CreatePrefab(SIGN_HAZARD, "Skybox");
        if (IconButton(content_sign_icy)) CreatePrefab(SIGN_ICY, "Skybox");
        GUILayout.EndHorizontal();
    }

    bool IconButton(GUIContent content)
    {
        bool val = GUILayout.Button(content, styleIconButton, GUILayout.MinWidth(10), GUILayout.MaxWidth(ICON_SIZE), GUILayout.Height(ICON_SIZE));
        return val;
    }

    bool FoldoutButton(string label, int indentLevel)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel*20);
        bool val = GUILayout.Button(label, smallButton);
        GUILayout.EndHorizontal();
        return val;
    }

    void FoldoutLabel(string label, int indentLevel)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel*20);
        GUILayout.Label(label, smallLabel);
        GUILayout.EndHorizontal();
    }

    ObjectCreator()
    {
        titleContent = new GUIContent("Marble It Up! Ultra");
    }

    void SetupStyle()
    {
        // load icons
        string path = "Assets/Marble It Up Ultra/_Source/Textures/Kit/";

        this.icon_boost =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_boost.png", typeof(Texture));
        this.icon_jump =                (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_jump.png", typeof(Texture));
        this.icon_featherfall =         (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_featherfall.png", typeof(Texture));
        this.icon_megamarble =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_megamarble.png", typeof(Texture));
        this.icon_timetravel =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_timetravel.png", typeof(Texture));
        this.icon_gate_boost =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_boost.png", typeof(Texture));
        this.icon_gate_jump =           (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_jump.png", typeof(Texture));
        this.icon_gate_featherfall =    (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_featherfall.png", typeof(Texture));
        this.icon_gate_gem =            (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_gem.png", typeof(Texture));
        this.icon_gate_timetravel =     (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_timetravel.png", typeof(Texture));
        this.icon_ring_boost =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_boost.png", typeof(Texture));
        this.icon_ring_jump =           (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_jump.png", typeof(Texture));
        this.icon_ring_featherfall =    (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_featherfall.png", typeof(Texture));
        this.icon_ring_gem =            (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_gem.png", typeof(Texture));
        this.icon_ring_timetravel =     (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_timetravel.png", typeof(Texture));
        this.icon_basher =              (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_basher.png", typeof(Texture));
        this.icon_bumper =              (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_bumper.png", typeof(Texture));
        this.icon_sign_continuous =     (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_continuous.png", typeof(Texture));
        this.icon_sign_curvy =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_curvy.png", typeof(Texture));
        this.icon_sign_dropoff =        (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_dropoff.png", typeof(Texture));
        this.icon_sign_fork =           (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_fork.png", typeof(Texture));
        this.icon_sign_hazard =         (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_hazard.png", typeof(Texture));
        this.icon_sign_icy =            (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_icy.png", typeof(Texture));
        this.icon_sign_steep =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_steep.png", typeof(Texture));
        this.icon_sign_turnleft =       (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_turnleft.png", typeof(Texture));
        this.icon_sign_turnright =      (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_turnright.png", typeof(Texture));
        
        this.content_startpad =         new GUIContent(icon_timetravel, "Start Pad");
        this.content_checkpoint =       new GUIContent(icon_timetravel, "Checkpoint");
        this.content_endpad =           new GUIContent(icon_timetravel, "End Pad");
        this.content_boost =            new GUIContent(icon_boost, "Boost");
        this.content_jump =             new GUIContent(icon_jump, "Super Jump");
        this.content_featherfall =      new GUIContent(icon_featherfall, "Feather Fall");
        this.content_megamarble =       new GUIContent(icon_megamarble, "Mega Marble");
        this.content_timetravel =       new GUIContent(icon_timetravel, "Time Travel");
        this.content_gate_boost =       new GUIContent(icon_gate_boost, "Boost Gate");
        this.content_gate_jump =        new GUIContent(icon_gate_jump, "Super Jump Gate");
        this.content_gate_featherfall = new GUIContent(icon_gate_featherfall, "Feather Fall Gate");
        this.content_gate_gem =         new GUIContent(icon_gate_gem, "Gem Gate");
        this.content_gate_timetravel =  new GUIContent(icon_gate_timetravel, "Time Travel Gate");
        this.content_ring_boost =       new GUIContent(icon_ring_boost, "Boost Ring Gate");
        this.content_ring_jump =        new GUIContent(icon_ring_jump, "Super Jump Ring Gate");
        this.content_ring_featherfall = new GUIContent(icon_ring_featherfall, "Feather Fall Ring Gate");
        this.content_ring_gem =         new GUIContent(icon_ring_gem, "Gem Ring Gate");
        this.content_ring_timetravel =  new GUIContent(icon_ring_timetravel, "Time Travel Ring Gate");
        this.content_basher =           new GUIContent(icon_basher, "Basher");
        this.content_bumper =           new GUIContent(icon_bumper, "Bumper");
        this.content_sign_continuous =  new GUIContent(icon_sign_continuous, "Continuous Turn");
        this.content_sign_curvy =       new GUIContent(icon_sign_curvy, "Curvy");
        this.content_sign_dropoff =     new GUIContent(icon_sign_dropoff, "Drop-off");
        this.content_sign_fork =        new GUIContent(icon_sign_fork, "Fork");
        this.content_sign_hazard =      new GUIContent(icon_sign_hazard, "Hazard");
        this.content_sign_icy =         new GUIContent(icon_sign_icy, "Icy");
        this.content_sign_steep =       new GUIContent(icon_sign_steep, "Steep");
        this.content_sign_turnleft =    new GUIContent(icon_sign_turnleft, "Turn Left");
        this.content_sign_turnright =   new GUIContent(icon_sign_turnright, "Turn Right");
        
        // apply styling
        styleIconButton = new GUIStyle(GUI.skin.button);
        styleIconButton.imagePosition = ImagePosition.ImageAbove;
        styleIconButton.fontSize = 10;
        styleIconButton.padding = new RectOffset(6, 6, 6, 6);

        bigLabel = new GUIStyle(GUI.skin.label);
        bigLabel.fontStyle = FontStyle.Bold;
        bigLabel.richText = true;

        smallLabel = new GUIStyle(GUI.skin.label);
        smallLabel.fontStyle = FontStyle.Normal;
        smallLabel.wordWrap = true;
        smallLabel.richText = true;

        bigButton = new GUIStyle(GUI.skin.button);
        bigButton.fontSize = bigLabel.fontSize;

        smallButton = new GUIStyle(GUI.skin.button);
        smallButton.fontSize = bigLabel.fontSize;
    }

    [MenuItem("Marble It Up/Level Kit Window")]
    static void Baker()
    {
        var w = GetWindow<ObjectCreator>();
        w.ShowTab();
    }

    #endregion

    #region Pickups

    static void CreateGem()
    {
        CreateFromResource("Gem", "Gameplay");
    }

    static void CreateSJ()
    {
        CreateFromResource("Jump", "Gameplay");
    }

    static void CreateFF()
    {
        CreateFromResource("Featherfall", "Gameplay");
    }

    static void CreateSS()
    {
        CreateFromResource("Boost", "Gameplay");
    }

    static void CreateTT()
    {
        CreateFromResource("TimeTravel", "Gameplay");
    }

    static void CreateTrophy()
    {
        CreateFromResource("Easter Egg", "Gameplay", true);
    }

    #endregion

    #region Core

    static void CreateLvlBounds()
    {
        GameObject o = Create("LevelBounds", "", "Gameplay", true);
        if (o != null)
        {
            BoxCollider bc = o.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = new Vector3(150, 50, 150);
        }
        UpdateLevelBounds();
    }

    static void CreateStartPad()
    {
        CreateFromResource("StartPad", "Gameplay", true);
    }

    static void CreateEndPad()
    {
        CreateFromResource("EndPad", "Gameplay", true);
    }

    static void CreateTimingObj()
    {
        GameObject o = Create("LevelTiming", "", "", true);
        if (o != null)
        {
            LevelTiming bc = o.AddComponent<LevelTiming>();
        }
    }

    static void UpdateLevelBounds()
    {
        BoxCollider bc = null;
        GameObject bounds = MapComponents.FindFixed("LevelBounds");
        if (bounds != null)
            bc = bounds.GetComponent<BoxCollider>();
        if(bc != null)
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.zero);
            foreach (Collider r in FindObjectsOfType<MeshCollider>())
            {
                b.Encapsulate(r.bounds);
            }
            bounds.transform.position = b.center;
            bc.center = Vector3.zero;
            bc.size = b.size + new Vector3(20, 20, 20);
        }       
    }

    #endregion

    #region Gameplay

    static void CreateBumper()
    {
        CreateFromResource("Bumper", "Gameplay");
    }

    static void CreateCheckpoint()
    {
        GameObject o = Create("CheckPoint", "checkpoint_icon", "Gameplay");
        BoxCollider bc = o.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(5, 3, 5);
        bc.center = new Vector3(0, 1.5f, 0);
    }

    static void CreateTutorial()
    {
        GameObject o = Create("Tutorial", "", "Gameplay");
        o.AddComponent<TutorialMessage>();
        BoxCollider bc = o.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(5, 3, 5);
        bc.center = new Vector3(0, 1.5f, 0);
    }

    #endregion

    #region Visual

    static void CreateSignCurve()
    {
        Create("Sign_Curvy", "sign_curve", "Skybox");
    }

    static void CreateSignDrop()
    {
        Create("Sign_DropOff", "sign_drop", "Skybox");
    }

    static void CreateSignFork()
    {
        Create("Sign_Fork", "sign_fork", "Skybox");
    }

    static void CreateSignHazard()
    {
        Create("Sign_Hazard", "sign_hazard", "Skybox");
    }

    static void CreateSignIce()
    {
        Create("Sign_Icy", "sign_ice", "Skybox");
    }

    static void CreateSignSteep()
    {
        Create("Sign_Steep", "sign_steep", "Skybox");
    }

    static void CreateSignLeft()
    {
        Create("Sign_TurnLeft", "sign_left", "Skybox");
    }

    static void CreateSignRight()
    {
        Create("Sign_TurnRight", "sign_right", "Skybox");
    }

    static void CreateSignContinuous()
    {
        Create("Sign_ContinuousTurn", "sign_continuous", "Skybox");
    }

    static void CreateBigCrystal()
    {
        Create("GiantCrystal", "bigcrystal_icon", "Skybox");
    }

    #endregion

    static void SetupLighting()
    {
        //Sky
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientIntensity = 1f;
        if(RenderSettings.skybox == null)
            RenderSettings.skybox = Resources.Load<Material>("Sky001");

        //Lightmap Core
#if UNITY_2017_3_OR_NEWER
        LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.Enlighten;
#else
        LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.Radiosity;
#endif
        Lightmapping.realtimeGI = false;
        Lightmapping.bakedGI = true;
        Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        SetInt("m_LightmapEditorSettings.m_LightmapsBakeMode", 0);
        SetBool("m_LightmapEditorSettings.m_FinalGather", false);
        SetFloat("m_GISettings.m_AlbedoBoost", 2);

        //Ambient Occlusion
        LightmapEditorSettings.aoMaxDistance = 8;
        LightmapEditorSettings.aoExponentDirect = 4;
        LightmapEditorSettings.aoExponentIndirect = 4;
        LightmapEditorSettings.enableAmbientOcclusion = true;

        //Texture Generation
        LightmapEditorSettings.bakeResolution = 4;
        SetFloat("m_LightmapEditorSettings.m_Resolution", 2);
        LightmapEditorSettings.padding = 2;
        Lightmapping.bounceBoost = 2;
        LightmapEditorSettings.maxAtlasHeight = 1024;
        LightmapEditorSettings.maxAtlasSize = 1024;
        SetBool("m_LightmapEditorSettings.m_TextureCompression", true);


        //Setup Directional Light
        Light light = FindObjectOfType<Light>();
        if(light == null)
        {
            GameObject o = new GameObject("Sun");
            light = o.AddComponent<Light>();
            light.type = LightType.Directional;
        }
        light.lightmapBakeType = LightmapBakeType.Mixed;
        light.intensity = 1;
        light.color = new Color(1, 244 / 255f, 214 / 255f);
        light.bounceIntensity = 2;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 1;
        GameObject staticObj = GameObject.Find("Static");
        if (staticObj == null)
            staticObj = new GameObject("Static");
        GameObject lighting = GameObject.Find("Lighting");
        if (lighting == null)
            lighting = new GameObject("Lighting");
        if (lighting.transform.parent != staticObj.transform)
            lighting.transform.SetParent(staticObj.transform);
        lighting.transform.localPosition = Vector3.zero;
        lighting.transform.localEulerAngles = Vector3.zero;
        light.transform.SetParent(lighting.transform);
        light.transform.localPosition = Vector3.zero;
        light.transform.localEulerAngles = new Vector3(50, -30, 0);
        light.gameObject.name = "Sun";
        light.gameObject.isStatic = true;
    }

    static GameObject Create(string itemName, string icon, string holderName, bool unique=false)
    {
        if(unique)
        {
            GameObject o = MapComponents.FindFixed(itemName);
            if(o != null)
            {
                Debug.LogError(itemName + " already exists!");
                Selection.activeGameObject = o;
                return null;
            }
        }

        GameObject obj = new GameObject(itemName);
        obj.transform.position = GetSpawnPos();
        SetIcon(obj, icon);
        if(holderName != null && holderName.Length > 0)
        {
            GameObject holder = GameObject.Find(holderName);
            if(holder == null)
                holder = new GameObject(holderName);

            obj.transform.SetParent(holder.transform);
        }
        Selection.activeGameObject = obj;
        return obj;
    }

    static GameObject CreateFromResource(string itemName, string holderName, bool unique = false)
    {
        if (unique)
        {
            GameObject o = GameObject.Find(MapComponents.FixName(itemName));
            if (o != null)
            {
                Debug.LogError(itemName + " already exists!");
                Selection.activeGameObject = o;
                return null;
            }
        }

        GameObject obj = Instantiate(Resources.Load<GameObject>(itemName));
        obj.name = itemName;
        obj.transform.position = GetSpawnPos();
        if (holderName != null && holderName.Length > 0)
        {
            GameObject holder = GameObject.Find(holderName);
            if (holder == null)
                holder = new GameObject(holderName);

            obj.transform.SetParent(holder.transform);
        }
        Selection.activeGameObject = obj;
        return obj;
    }

    static GameObject CreatePrefab(string guid, string holderName, bool unique = false)
    {
        // retrieve prefab by its GUID
        var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
        var prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (!prefabObj)
            return null;

        if (unique)
        {
            GameObject o = GameObject.Find(MapComponents.FixName(prefabObj.name));
            if (o != null)
            {
                Debug.LogError(prefabObj.name + " already exists!");
                Selection.activeGameObject = o;
                return null;
            }
        }
        
        GameObject obj = PrefabUtility.InstantiatePrefab(prefabObj) as GameObject;
        obj.transform.position = GetSpawnPos();

        if (holderName != null && holderName.Length > 0)
        {
            GameObject holder = GameObject.Find(holderName);
            if (holder == null)
                holder = new GameObject(holderName);

            obj.transform.SetParent(holder.transform);
        }

        Selection.activeGameObject = obj;
        return obj;
    }

    static Vector3 GetSpawnPos()
    {
        Ray r = new Ray();
        r.direction = SceneView.lastActiveSceneView.camera.transform.forward;
        r.origin = SceneView.lastActiveSceneView.camera.transform.position;
        RaycastHit hit;
        if(Physics.Raycast(r, out hit, 100))
        {
            return hit.point;
        }
        return SceneView.lastActiveSceneView.camera.transform.position + (SceneView.lastActiveSceneView.camera.transform.forward * 2f);
    }

    static void SetIcon(GameObject o, string icon)
    {
        Texture2D ico = Resources.Load<Texture2D>(icon);
        if (ico != null)
            SetIcon(o, ico);
    }

    private static void SetIcon(GameObject gObj, Texture2D texture)
    {
        var ty = typeof(EditorGUIUtility);
        var mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        mi.Invoke(null, new object[] { gObj, texture });
    }

    #region Lightmap System.Reflection Work

    public static void SetFloat(string name, float val)
    {
        ChangeProperty(name, property => property.floatValue = val);
    }

    public static void SetInt(string name, int val)
    {
        ChangeProperty(name, property => property.intValue = val);
    }

    public static void SetBool(string name, bool val)
    {
        ChangeProperty(name, property => property.boolValue = val);
    }

    public static void ChangeProperty(string name, Action<SerializedProperty> changer)
    {
        var lightmapSettings = getLighmapSettings();
        var prop = lightmapSettings.FindProperty(name);
        if (prop != null)
        {
            changer(prop);
            lightmapSettings.ApplyModifiedProperties();
        }
        else Debug.LogError("lighmap property not found: " + name);
    }

    static SerializedObject getLighmapSettings()
    {
        var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
        var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as Object;
        return new SerializedObject(lightmapSettings);
    }

    #endregion

    // prefab GUIDs
    private const string BOOST = "b48bea942ebbfa14d9448c0463356942";
    private const string JUMP = "4d763d9049a6b5845ba97e2f48608199";
    private const string FEATHER_FALL = "f166b3a35ca94a84ba983c35c0cf1073";
    private const string MEGA_MARBLE = "be0d3ec95eba66647b2aec82c99342ce";
    private const string TIME_TRAVEL = "04e5ed3090125014dbf3eea3e77e0adf";

    private const string GATE_BOOST = "5f503b3b0506ab4459ed1856bbdfa40d";
    private const string GATE_JUMP = "27f99fb1b220d7c48ab3d3324e7e61ca";
    private const string GATE_FEATHER_FALL = "2c0078403db8078439dd9375ac380a11";
    private const string GATE_GEM = "a66503db67a0a6942861fcffe3e0f258";
    private const string GATE_TIME_TRAVEL = "7c28765487dec9d4bb81f576196b4ae1";

    private const string RING_BOOST = "d882e731ae1c69c44b99e329fb9e692a";
    private const string RING_JUMP = "61677fa7b9536d64b9dbdeaaf1229534";
    private const string RING_FEATHER_FALL = "ab808cb0537754349b2f769d2e0bb5e0";
    private const string RING_GEM = "88ea1209a39025f4588dd7d06f9cce68";
    private const string RING_TIME_TRAVEL = "418d4ba566276f14a8e5db59ad8d72b4";

    private const string SIGN_CONTINUOUS = "4694624a8599eef4ab450563af3fdada";
    private const string SIGN_CURVY = "23460650ad533f84c89f25d0d1d097a5";
    private const string SIGN_DROPOFF = "415bbe9734cc6e24c91600ff59d6dede";
    private const string SIGN_FORK = "954e2f68e0718c04eaf404386219d3f0";
    private const string SIGN_HAZARD = "0a2a2255d20784f499aa26d379c3432c";
    private const string SIGN_ICY = "983b10274f185204b80f3ed9c9daa7d7";
    private const string SIGN_STEEP = "19ccaa1095d6874438f9906905ee75d4";
    private const string SIGN_TURNLEFT = "7b0da58e89a366d409351c12fe7c4bfa";
    private const string SIGN_TURNRIGHT = "269bb9d6640f71e41962bdc87d9bb3dd";

    // icon buttons
    private Texture icon_boost;
    private Texture icon_jump;
    private Texture icon_featherfall;
    private Texture icon_megamarble;
    private Texture icon_timetravel;
    private Texture icon_gate_boost;
    private Texture icon_gate_jump;
    private Texture icon_gate_featherfall;
    private Texture icon_gate_gem;
    private Texture icon_gate_timetravel;
    private Texture icon_ring_boost;
    private Texture icon_ring_jump;
    private Texture icon_ring_featherfall;
    private Texture icon_ring_gem;
    private Texture icon_ring_timetravel;
    private Texture icon_basher;
    private Texture icon_bumper;
    private Texture icon_sign_continuous;
    private Texture icon_sign_curvy;
    private Texture icon_sign_dropoff;
    private Texture icon_sign_fork;
    private Texture icon_sign_hazard;
    private Texture icon_sign_icy;
    private Texture icon_sign_steep;
    private Texture icon_sign_turnleft;
    private Texture icon_sign_turnright;
    
    private GUIContent content_startpad;
    private GUIContent content_checkpoint;
    private GUIContent content_endpad;
    private GUIContent content_boost;
    private GUIContent content_jump;
    private GUIContent content_featherfall;
    private GUIContent content_megamarble;
    private GUIContent content_timetravel;
    private GUIContent content_gate_boost;
    private GUIContent content_gate_jump;
    private GUIContent content_gate_featherfall;
    private GUIContent content_gate_gem;
    private GUIContent content_gate_timetravel;
    private GUIContent content_ring_boost;
    private GUIContent content_ring_jump;
    private GUIContent content_ring_featherfall;
    private GUIContent content_ring_gem;
    private GUIContent content_ring_timetravel;
    private GUIContent content_basher;
    private GUIContent content_bumper;
    private GUIContent content_sign_continuous;
    private GUIContent content_sign_curvy;
    private GUIContent content_sign_dropoff;
    private GUIContent content_sign_fork;
    private GUIContent content_sign_hazard;
    private GUIContent content_sign_icy;
    private GUIContent content_sign_steep;
    private GUIContent content_sign_turnleft;
    private GUIContent content_sign_turnright;
}