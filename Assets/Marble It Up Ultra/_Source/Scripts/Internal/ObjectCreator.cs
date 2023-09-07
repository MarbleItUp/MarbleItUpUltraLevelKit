using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Object = UnityEngine.Object;

public class ObjectCreator : EditorWindow
{
    private const float ICON_SIZE = 50f;

    Vector2 scrollPos;

    // element styles
    static GUIStyle styleHeader;
    GUIStyle styleBigButton;
    GUIStyle styleIconButton;
    GUIStyle styleIconButtonDisabled;

    // foldout categories
    bool showCore;
    bool showPowerups;
    bool showGates;
    bool showHazards;
    bool showSigns;

    ObjectCreator()
    {
        titleContent = new GUIContent("Marble It Up! Ultra");
    }

    [MenuItem("Marble It Up/Level Kit Window")]
    static void Baker()
    {
        var w = GetWindow<ObjectCreator>();
        w.ShowTab();
    }

    private void OnGUI()
    {
        SetupStyle();

        // Draw UI
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();
        if(IsSceneReady())
        {
            DrawHeader("Objects");

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

    bool IsSceneReady()
    {
        bool lightSet = true;
        if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand || Lightmapping.realtimeGI != false || Lightmapping.bakedGI != true)
            lightSet = false;
        bool readyForParts = lightSet;

        if(!readyForParts)
        {
            DrawHeader("Scene Setup");
            if (!lightSet) { if (GUILayout.Button("Setup Lighting", styleBigButton)) SetupLighting(); } // TODO: button does not draw
        }

        return readyForParts;
    }

    void CoreUI()
    {
        // core
        GUILayout.BeginHorizontal();

        if (MapComponents.FindFixed("StartPad") == null) {
            if (IconButton(content_startpad)) CreatePrefab(START_PAD, "Gameplay", true);
        }
        else { 
            if (IconButtonDisabled(content_startpad_c)) SelectObject("StartPad");
        }
        
        if (MapComponents.FindFixed("EndPad") == null) {
            if (IconButton(content_endpad)) CreatePrefab(END_PAD, "Gameplay", true);
        }
        else {
            if (IconButtonDisabled(content_endpad_c)) SelectObject("EndPad");
        }

        if (FindObjectOfType<LevelTiming>() == null) {
            if (IconButton(content_timing)) CreatePrefab(LEVEL_TIMING, "Gameplay", true);
        }
        else {
            if (IconButtonDisabled(content_timing_c)) SelectObject("LevelTiming");
        }

        if ((MapComponents.FindFixed("Easter Egg") == null)) {
            if (IconButton(content_egg)) CreatePrefab(EGG, "Gameplay", true);
        }
        else {
            if (IconButtonDisabled(content_egg_c)) SelectObject("Easter Egg");
        }

        if ((MapComponents.FindFixed("LevelBounds") == null)) {
            if (IconButton(content_bounds)) { CreatePrefab(LEVEL_BOUNDS, "Gameplay", true); UpdateLevelBounds(); }
        }
        else {
            if (IconButton(content_bounds_c)) UpdateLevelBounds();
        }

        if (IconButton(content_checkpoint)) CreatePrefab(CHECKPOINT, "Gameplay");
        if (IconButton(content_text)) CreatePrefab(TUTORIAL_MESSAGE, "Gameplay");

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
        if (IconButton(content_gem)) CreatePrefab(GEM, "Gameplay");
        GUILayout.EndHorizontal();
    }

    void GatesUI()
    {
        // gates
        GUILayout.BeginHorizontal();
        if (IconButton(content_gate_boost)) CreatePrefab(GATE_BOOST, "Gameplay");
        if (IconButton(content_gate_jump)) CreatePrefab(GATE_JUMP, "Gameplay");
        if (IconButton(content_gate_featherfall)) CreatePrefab(GATE_FEATHER_FALL, "Gameplay");
        if (IconButton(content_gate_megamarble)) CreatePrefab(GATE_MEGA_MARBLE, "Gameplay");
        if (IconButton(content_gate_timetravel)) CreatePrefab(GATE_TIME_TRAVEL, "Gameplay");
        if (IconButton(content_gate_gem)) CreatePrefab(GATE_GEM, "Gameplay");
        GUILayout.EndHorizontal();

        // ring gates
        GUILayout.BeginHorizontal();
        if (IconButton(content_ring_boost)) CreatePrefab(RING_BOOST, "Gameplay");
        if (IconButton(content_ring_jump)) CreatePrefab(RING_JUMP, "Gameplay");
        if (IconButton(content_ring_featherfall)) CreatePrefab(RING_FEATHER_FALL, "Gameplay");
        if (IconButton(content_ring_megamarble)) CreatePrefab(RING_MEGA_MARBLE, "Gameplay");
        if (IconButton(content_ring_timetravel)) CreatePrefab(RING_TIME_TRAVEL, "Gameplay");
        if (IconButton(content_ring_gem)) CreatePrefab(RING_GEM, "Gameplay");
        GUILayout.EndHorizontal();

        // horizontal ring gates
        GUILayout.BeginHorizontal();
        if (IconButton(content_ringhoriz_boost)) CreatePrefab(RING_HORIZ_BOOST, "Gameplay");
        if (IconButton(content_ringhoriz_jump)) CreatePrefab(RING_HORIZ_JUMP, "Gameplay");
        if (IconButton(content_ringhoriz_featherfall)) CreatePrefab(RING_HORIZ_FEATHER_FALL, "Gameplay");
        if (IconButton(content_ringhoriz_megamarble)) CreatePrefab(RING_HORIZ_MEGA_MARBLE, "Gameplay");
        if (IconButton(content_ringhoriz_timetravel)) CreatePrefab(RING_HORIZ_TIME_TRAVEL, "Gameplay");
        if (IconButton(content_ringhoriz_gem)) CreatePrefab(RING_HORIZ_GEM, "Gameplay");
        GUILayout.EndHorizontal();
    }

    void HazardsUI()
    {
        // hazards
        GUILayout.BeginHorizontal();
        if (IconButton(content_basher)) CreatePrefab(BASHER, "Dynamic");
        if (IconButton(content_bumper)) CreatePrefab(BUMPER, "Gameplay");
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

    bool IconButtonDisabled(GUIContent content)
    {
        bool val = GUILayout.Button(content, styleIconButtonDisabled, GUILayout.MinWidth(10), GUILayout.MaxWidth(ICON_SIZE), GUILayout.Height(ICON_SIZE));
        return val;
    }

    public static void DrawHeader(string text)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0), GUILayout.MaxWidth(9999));
        GUILayout.Label(text, styleHeader);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(0), GUILayout.MaxWidth(9999));
        GUILayout.EndHorizontal();
    }

    void SetupStyle()
    {
        // load icons
        string path = "Assets/Marble It Up Ultra/_Source/Textures/Kit/";

        this.icon_startpad =                 (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_startpad.png", typeof(Texture));
        this.icon_startpad_c =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_startpad_c.png", typeof(Texture));
        this.icon_checkpoint =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_checkpoint.png", typeof(Texture));
        this.icon_endpad =                   (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_endpad.png", typeof(Texture));
        this.icon_endpad_c =                 (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_endpad_c.png", typeof(Texture));
        this.icon_egg =                      (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_egg.png", typeof(Texture));
        this.icon_egg_c =                    (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_egg_c.png", typeof(Texture));
        this.icon_text =                     (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_text.png", typeof(Texture));
        this.icon_timing =                   (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_timing.png", typeof(Texture));
        this.icon_timing_c =                 (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_timing_c.png", typeof(Texture));
        this.icon_bounds =                   (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_bounds.png", typeof(Texture));
        this.icon_bounds_c =                 (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_bounds_c.png", typeof(Texture));
        this.icon_boost =                    (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_boost.png", typeof(Texture));
        this.icon_jump =                     (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_jump.png", typeof(Texture));
        this.icon_featherfall =              (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_featherfall.png", typeof(Texture));
        this.icon_megamarble =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_megamarble.png", typeof(Texture));
        this.icon_timetravel =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_timetravel.png", typeof(Texture));
        this.icon_gem =                      (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gem.png", typeof(Texture));
        this.icon_gate_boost =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_boost.png", typeof(Texture));
        this.icon_gate_jump =                (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_jump.png", typeof(Texture));
        this.icon_gate_featherfall =         (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_featherfall.png", typeof(Texture));
        this.icon_gate_megamarble =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_megamarble.png", typeof(Texture));
        this.icon_gate_timetravel =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_timetravel.png", typeof(Texture));
        this.icon_gate_gem =                 (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_gate_gem.png", typeof(Texture));
        this.icon_ring_boost =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_boost.png", typeof(Texture));
        this.icon_ring_jump =                (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_jump.png", typeof(Texture));
        this.icon_ring_featherfall =         (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_featherfall.png", typeof(Texture));
        this.icon_ring_megamarble =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_megamarble.png", typeof(Texture));
        this.icon_ring_timetravel =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_timetravel.png", typeof(Texture));
        this.icon_ring_gem =                 (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ring_gem.png", typeof(Texture));
        this.icon_ringhoriz_boost =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ringhoriz_boost.png", typeof(Texture));
        this.icon_ringhoriz_jump =           (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ringhoriz_jump.png", typeof(Texture));
        this.icon_ringhoriz_featherfall =    (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ringhoriz_featherfall.png", typeof(Texture));
        this.icon_ringhoriz_megamarble =     (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ringhoriz_megamarble.png", typeof(Texture));
        this.icon_ringhoriz_timetravel =     (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ringhoriz_timetravel.png", typeof(Texture));
        this.icon_ringhoriz_gem =            (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_ringhoriz_gem.png", typeof(Texture));
        this.icon_basher =                   (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_basher.png", typeof(Texture));
        this.icon_bumper =                   (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_bumper.png", typeof(Texture));
        this.icon_sign_continuous =          (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_continuous.png", typeof(Texture));
        this.icon_sign_curvy =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_curvy.png", typeof(Texture));
        this.icon_sign_dropoff =             (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_dropoff.png", typeof(Texture));
        this.icon_sign_fork =                (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_fork.png", typeof(Texture));
        this.icon_sign_hazard =              (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_hazard.png", typeof(Texture));
        this.icon_sign_icy =                 (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_icy.png", typeof(Texture));
        this.icon_sign_steep =               (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_steep.png", typeof(Texture));
        this.icon_sign_turnleft =            (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_turnleft.png", typeof(Texture));
        this.icon_sign_turnright =           (Texture)AssetDatabase.LoadAssetAtPath(path + "icon_sign_turnright.png", typeof(Texture));
        
        this.content_startpad =              new GUIContent(icon_startpad, "Start Pad");
        this.content_startpad_c =            new GUIContent(icon_startpad_c, "Start Pad already exists");
        this.content_checkpoint =            new GUIContent(icon_checkpoint, "Checkpoint");
        this.content_endpad =                new GUIContent(icon_endpad, "End Pad");
        this.content_endpad_c =              new GUIContent(icon_endpad_c, "End Pad already exists");
        this.content_egg =                   new GUIContent(icon_egg, "Treasure Box");
        this.content_egg_c =                 new GUIContent(icon_egg_c, "Treasure Box already exists");
        this.content_text =                  new GUIContent(icon_text, "Tutorial Message");
        this.content_timing =                new GUIContent(icon_timing, "Medal Times");
        this.content_timing_c =              new GUIContent(icon_timing_c, "Medal Times already exist");
        this.content_bounds =                new GUIContent(icon_bounds, "Level Bounds");
        this.content_bounds_c =              new GUIContent(icon_bounds_c, "Recalculate Level Bounds");
        this.content_boost =                 new GUIContent(icon_boost, "Boost");
        this.content_jump =                  new GUIContent(icon_jump, "Super Jump");
        this.content_featherfall =           new GUIContent(icon_featherfall, "Feather Fall");
        this.content_megamarble =            new GUIContent(icon_megamarble, "Mega Marble");
        this.content_timetravel =            new GUIContent(icon_timetravel, "Time Travel");
        this.content_gem =                   new GUIContent(icon_gem, "Gem");
        this.content_gate_boost =            new GUIContent(icon_gate_boost, "Boost Gate");
        this.content_gate_jump =             new GUIContent(icon_gate_jump, "Super Jump Gate");
        this.content_gate_featherfall =      new GUIContent(icon_gate_featherfall, "Feather Fall Gate");
        this.content_gate_megamarble =       new GUIContent(icon_gate_megamarble, "Mega Marble Gate");
        this.content_gate_timetravel =       new GUIContent(icon_gate_timetravel, "Time Travel Gate");
        this.content_gate_gem =              new GUIContent(icon_gate_gem, "Gem Gate");
        this.content_ring_boost =            new GUIContent(icon_ring_boost, "Boost Ring Gate");
        this.content_ring_jump =             new GUIContent(icon_ring_jump, "Super Jump Ring Gate");
        this.content_ring_featherfall =      new GUIContent(icon_ring_featherfall, "Feather Fall Ring Gate");
        this.content_ring_megamarble =       new GUIContent(icon_ring_megamarble, "Mega Marble Ring Gate");
        this.content_ring_timetravel =       new GUIContent(icon_ring_timetravel, "Time Travel Ring Gate");
        this.content_ring_gem =              new GUIContent(icon_ring_gem, "Gem Ring Gate");
        this.content_ringhoriz_boost =       new GUIContent(icon_ringhoriz_boost, "Boost Ring Horizontal Gate");
        this.content_ringhoriz_jump =        new GUIContent(icon_ringhoriz_jump, "Super Jump Horizontal Ring Gate");
        this.content_ringhoriz_featherfall = new GUIContent(icon_ringhoriz_featherfall, "Feather Fall Horizontal Ring Gate");
        this.content_ringhoriz_megamarble =  new GUIContent(icon_ringhoriz_megamarble, "Mega Marble Horizontal Ring Gate");
        this.content_ringhoriz_timetravel =  new GUIContent(icon_ringhoriz_timetravel, "Time Travel Horizontal Ring Gate");
        this.content_ringhoriz_gem =         new GUIContent(icon_ringhoriz_gem, "Gem Horizontal Ring Gate");
        this.content_basher =                new GUIContent(icon_basher, "Basher");
        this.content_bumper =                new GUIContent(icon_bumper, "Bumper");
        this.content_sign_continuous =       new GUIContent(icon_sign_continuous, "Continuous Turn");
        this.content_sign_curvy =            new GUIContent(icon_sign_curvy, "Curvy");
        this.content_sign_dropoff =          new GUIContent(icon_sign_dropoff, "Drop-off");
        this.content_sign_fork =             new GUIContent(icon_sign_fork, "Fork");
        this.content_sign_hazard =           new GUIContent(icon_sign_hazard, "Hazard");
        this.content_sign_icy =              new GUIContent(icon_sign_icy, "Icy");
        this.content_sign_steep =            new GUIContent(icon_sign_steep, "Steep");
        this.content_sign_turnleft =         new GUIContent(icon_sign_turnleft, "Turn Left");
        this.content_sign_turnright =        new GUIContent(icon_sign_turnright, "Turn Right");
        
        // apply styling
        styleIconButton = new GUIStyle(GUI.skin.button);
        styleIconButton.imagePosition = ImagePosition.ImageAbove;
        styleIconButton.fontSize = 10;
        styleIconButton.padding = new RectOffset(6, 6, 6, 6);
        
        styleIconButtonDisabled = new GUIStyle(GUI.skin.textField);
        styleIconButtonDisabled.imagePosition = ImagePosition.ImageAbove;
        styleIconButtonDisabled.fontSize = 10;
        styleIconButtonDisabled.padding = new RectOffset(6, 6, 6, 6);
        styleIconButtonDisabled.alignment = TextAnchor.MiddleCenter;

        styleHeader = new GUIStyle(GUI.skin.label);
        styleHeader.fontStyle = FontStyle.Bold;
        styleHeader.richText = true;

        styleBigButton = new GUIStyle(GUI.skin.button);
        styleBigButton.fontSize = styleHeader.fontSize;
    }

    static void UpdateLevelBounds()
    {
        GameObject bounds = MapComponents.FindFixed("LevelBounds");

        if (bounds == null)
            return;

        BoxCollider bc = bounds.GetComponent<BoxCollider>();
        if (bc != null)
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.zero);
            foreach (Collider r in FindObjectsOfType<MeshCollider>())
            {
                b.Encapsulate(r.bounds);
            }
            bounds.transform.position = b.center;
            bc.center = Vector3.zero;
            bc.size = b.size + new Vector3(20, 20, 20);

            Selection.activeGameObject = bounds;
            Debug.Log("Level Bounds have been automatically resized.");
        }
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

    static GameObject CreatePrefab(string guid, string holderName, bool unique = false)
    {
        // retrieve prefab by its GUID
        var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
        var prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (!prefabObj)
        {
            Debug.LogError("Prefab ID not found: " + guid);
            return null;
        }

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

        Debug.Log("Object created: " + obj.name);
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

    static void SelectObject(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj)
            Selection.activeGameObject = obj;
    }

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
        else Debug.LogError("lightmap property not found: " + name);
    }

    static SerializedObject getLighmapSettings()
    {
        var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
        var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as Object;
        return new SerializedObject(lightmapSettings);
    }

    // prefab GUIDs
    private const string START_PAD = "8d4ca6f9b72fb6043bdaf8c81471795a";
    private const string CHECKPOINT = "e0c71b1fbcb31c7468ec26261a5c1f1e";
    private const string END_PAD = "814cd116c3c551c4a96bc036383e9557";
    private const string EGG = "5745dbe0b15d727409e51b65c80ce0f7";
    private const string LEVEL_BOUNDS = "21b9929ae70d9ad42bbd8eeb8c4acbac";
    private const string LEVEL_TIMING = "c8e56755e90da9e428eba6f9e7859af1";
    private const string TUTORIAL_MESSAGE = "4c6c31f81b483fb42a90d08ec5180947";

    private const string BOOST = "b48bea942ebbfa14d9448c0463356942";
    private const string JUMP = "4d763d9049a6b5845ba97e2f48608199";
    private const string FEATHER_FALL = "f166b3a35ca94a84ba983c35c0cf1073";
    private const string MEGA_MARBLE = "be0d3ec95eba66647b2aec82c99342ce";
    private const string TIME_TRAVEL = "04e5ed3090125014dbf3eea3e77e0adf";
    private const string GEM = "49a51a89af6d3aa46afc6318f6b3b728";

    private const string GATE_BOOST = "5f503b3b0506ab4459ed1856bbdfa40d";
    private const string GATE_JUMP = "27f99fb1b220d7c48ab3d3324e7e61ca";
    private const string GATE_FEATHER_FALL = "2c0078403db8078439dd9375ac380a11";
    private const string GATE_MEGA_MARBLE = "96d1c05f6eacd3a428cde445fceda330";
    private const string GATE_TIME_TRAVEL = "7c28765487dec9d4bb81f576196b4ae1";
    private const string GATE_GEM = "a66503db67a0a6942861fcffe3e0f258";

    private const string RING_BOOST = "d882e731ae1c69c44b99e329fb9e692a";
    private const string RING_JUMP = "61677fa7b9536d64b9dbdeaaf1229534";
    private const string RING_FEATHER_FALL = "ab808cb0537754349b2f769d2e0bb5e0";
    private const string RING_MEGA_MARBLE = "01a3d4795920e71469049e42055ebd95";
    private const string RING_TIME_TRAVEL = "418d4ba566276f14a8e5db59ad8d72b4";
    private const string RING_GEM = "88ea1209a39025f4588dd7d06f9cce68";

    private const string RING_HORIZ_BOOST = "8afce3bc9a427bd4e821c18554312844";
    private const string RING_HORIZ_JUMP = "62a33deaffd43a5469d48688de0b9a7b";
    private const string RING_HORIZ_FEATHER_FALL = "c115c926bf241ef498682428a2d44349";
    private const string RING_HORIZ_MEGA_MARBLE = "48eef5fbb6341c54eb2f67e530e16022";
    private const string RING_HORIZ_TIME_TRAVEL = "3aefead2f8471a94a9a1fcd1367d82f5";
    private const string RING_HORIZ_GEM = "1763c8e12601e4a4b9bf301dab9723ac";

    private const string BASHER = "a1e8d260398941247af4a6f7a5d72792";
    private const string BUMPER = "6bf8a6447fd165a4abcf55f29bcd8061";

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
    private Texture icon_startpad;
    private Texture icon_startpad_c;
    private Texture icon_checkpoint;
    private Texture icon_endpad;
    private Texture icon_endpad_c;
    private Texture icon_egg;
    private Texture icon_egg_c;
    private Texture icon_text;
    private Texture icon_timing;
    private Texture icon_timing_c;
    private Texture icon_bounds;
    private Texture icon_bounds_c;
    private Texture icon_boost;
    private Texture icon_jump;
    private Texture icon_featherfall;
    private Texture icon_megamarble;
    private Texture icon_timetravel;
    private Texture icon_gem;
    private Texture icon_gate_boost;
    private Texture icon_gate_jump;
    private Texture icon_gate_featherfall;
    private Texture icon_gate_megamarble;
    private Texture icon_gate_timetravel;
    private Texture icon_gate_gem;
    private Texture icon_ring_boost;
    private Texture icon_ring_jump;
    private Texture icon_ring_featherfall;
    private Texture icon_ring_megamarble;
    private Texture icon_ring_timetravel;
    private Texture icon_ring_gem;
    private Texture icon_ringhoriz_boost;
    private Texture icon_ringhoriz_jump;
    private Texture icon_ringhoriz_featherfall;
    private Texture icon_ringhoriz_megamarble;
    private Texture icon_ringhoriz_timetravel;
    private Texture icon_ringhoriz_gem;
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
    private GUIContent content_startpad_c;
    private GUIContent content_checkpoint;
    private GUIContent content_endpad;
    private GUIContent content_endpad_c;
    private GUIContent content_egg;
    private GUIContent content_egg_c;
    private GUIContent content_text;
    private GUIContent content_timing;
    private GUIContent content_timing_c;
    private GUIContent content_bounds;
    private GUIContent content_bounds_c;
    private GUIContent content_boost;
    private GUIContent content_jump;
    private GUIContent content_featherfall;
    private GUIContent content_megamarble;
    private GUIContent content_timetravel;
    private GUIContent content_gem;
    private GUIContent content_gate_boost;
    private GUIContent content_gate_jump;
    private GUIContent content_gate_featherfall;
    private GUIContent content_gate_megamarble;
    private GUIContent content_gate_timetravel;
    private GUIContent content_gate_gem;
    private GUIContent content_ring_boost;
    private GUIContent content_ring_jump;
    private GUIContent content_ring_featherfall;
    private GUIContent content_ring_megamarble;
    private GUIContent content_ring_timetravel;
    private GUIContent content_ring_gem;
    private GUIContent content_ringhoriz_boost;
    private GUIContent content_ringhoriz_jump;
    private GUIContent content_ringhoriz_featherfall;
    private GUIContent content_ringhoriz_megamarble;
    private GUIContent content_ringhoriz_timetravel;
    private GUIContent content_ringhoriz_gem;
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