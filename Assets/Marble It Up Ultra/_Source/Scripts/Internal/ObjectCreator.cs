﻿using System.Collections;
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

    // element styles
    GUIStyle bigLabel;
    GUIStyle smallLabel;
    GUIStyle bigButton;
    GUIStyle smallButton;
    GUIStyle styleIconButton;

    // foldouts
    bool showUltra;

    private void OnGUI()
    {
        SetupStyle();

        //UI Content
        GUILayout.BeginVertical();
        if(BaseSetupUI())
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0), GUILayout.MaxWidth(9999));
            GUILayout.Label("Prefabs", bigLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0), GUILayout.MaxWidth(9999));
            GUILayout.EndHorizontal();

            // showUltra = EditorGUILayout.Foldout(showUltra, "Ultra");
            // if (showUltra)
            UltraUI();

            // Level Exporter
            EditorGUILayout.Space(10);
            MapExporter.RunGUI();
        }


        GUILayout.EndVertical();
    }

    bool BaseSetupUI()
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

    void CoreUI()
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

    void GameplayUI()
    {
        int gemCount = MapComponents.GetNumOf("Gem");
        FoldoutLabel("Pickups", 1);
        GUILayout.Space(2);
        if (FoldoutButton("Gem", 1)) CreateGem();
        if (FoldoutButton("Super Jump", 1)) CreateSJ();
        if (FoldoutButton("Super Speed", 1)) CreateSS();
        if (FoldoutButton("Feather Fall", 1)) CreateFF();
        if (FoldoutButton("Time Travel", 1)) CreateTT();
        if (FoldoutButton("Trophy", 1)) CreateTrophy();
        GUILayout.Space(2);
        FoldoutLabel("Gems: <color=#0085ff>" + gemCount + "</color>", 1);
        GUILayout.Space(10);
        FoldoutLabel("Interactive", 1);
        GUILayout.Space(2);
        if (FoldoutButton("Bumper", 1)) CreateBumper();
        if (FoldoutButton("Checkpoint", 1)) CreateCheckpoint();
        if (FoldoutButton("Tutorial", 1)) CreateTutorial();
        GUILayout.Space(5);
    }

    bool showSigns;
    void FXUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(1 * 20);
        showSigns = EditorGUILayout.Foldout(showSigns, "Signs");
        GUILayout.EndHorizontal();
        if (showSigns)
        {
            if (FoldoutButton("Curvy", 2)) CreateSignCurve();
            if (FoldoutButton("Dropoff", 2)) CreateSignDrop();
            if (FoldoutButton("Fork", 2)) CreateSignFork();
            if (FoldoutButton("Hazard", 2)) CreateSignHazard();
            if (FoldoutButton("Icy", 2)) CreateSignIce();
            if (FoldoutButton("Steep", 2)) CreateSignSteep();
            if (FoldoutButton("Left Turn", 2)) CreateSignLeft();
            if (FoldoutButton("Right Turn", 2)) CreateSignRight();
            if (FoldoutButton("Continuous Turn", 2)) CreateSignContinuous();
        }
        GUILayout.Space(1);
        FoldoutLabel("More FX Objects can be found in Assets/MIU/Prefabs/FX folder.", 1);
        GUILayout.Space(5);
    }

    void MiscUI()
    {
        if (FoldoutButton("Marble Size Ref", 1)) CreateMableRef();
    }

    void UltraUI()
    {
        // core
        GUILayout.BeginHorizontal();
        if (IconButton(content_startpad)) CreateSS();
        if (IconButton(content_checkpoint)) CreateSJ();
        if (IconButton(content_endpad)) CreateFF();
        GUILayout.EndHorizontal();

        // powerups
        GUILayout.BeginHorizontal();
        if (IconButton(content_boost)) CreateSS();
        if (IconButton(content_jump)) CreateSJ();
        if (IconButton(content_featherfall)) CreateFF();
        if (IconButton(content_megamarble)) CreateSS();
        if (IconButton(content_timetravel)) CreateTT();
        GUILayout.EndHorizontal();

        // gates
        GUILayout.BeginHorizontal();
        if (IconButton(content_gate_boost)) CreateSS();
        if (IconButton(content_gate_jump)) CreateSJ();
        if (IconButton(content_gate_featherfall)) CreateFF();
        if (IconButton(content_gate_gem)) CreateSS();
        if (IconButton(content_gate_timetravel)) CreateTT();
        GUILayout.EndHorizontal();

        // ring gates
        GUILayout.BeginHorizontal();
        if (IconButton(content_ring_boost)) CreateSS();
        if (IconButton(content_ring_jump)) CreateSJ();
        if (IconButton(content_ring_featherfall)) CreateFF();
        if (IconButton(content_ring_gem)) CreateSS();
        if (IconButton(content_ring_timetravel)) CreateTT();
        GUILayout.EndHorizontal();

        // hazards
        GUILayout.BeginHorizontal();
        if (IconButton(content_basher)) CreateSS();
        if (IconButton(content_bumper)) CreateSJ();
        GUILayout.EndHorizontal();

        // signs
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

    #region Multiplayer

    static void CreateSpawnPoint()
    {
        Create("SpawnPoint", "mpspawn_icon", "Multiplayer", true);
    }

    #endregion

    static void CreateMableRef()
    {
        GameObject o = GameObject.Find("marble_size_reference");
        if (o != null)
        {
            Debug.LogError("Size reference already exists!");
            Selection.activeGameObject = o;
            return;
        }

        o = Instantiate(Resources.Load<GameObject>("marble_size_reference"));
        o.transform.name = "marble_size_reference";
        Selection.activeGameObject = o;
    }

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

    #region Lightmap Syste.Reflection Work

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
}