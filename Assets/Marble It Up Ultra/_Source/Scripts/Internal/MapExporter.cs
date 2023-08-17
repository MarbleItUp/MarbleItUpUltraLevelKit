using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using MIU;

public class MapExporter : EditorWindow
{
    public static bool hasResult = false;

    MapExporter()
    {
        titleContent = new GUIContent("Level Exporter");
    }

    public static void RunGUI()
    {
        var bigLabel = new GUIStyle(GUI.skin.label);
        bigLabel.fontStyle = FontStyle.Bold;
        bigLabel.richText = true;

        var smallLabel = new GUIStyle(GUI.skin.label);
        smallLabel.wordWrap = true;
        smallLabel.fontStyle = FontStyle.Normal;
        smallLabel.richText = true;

        var bigButton = new GUIStyle(GUI.skin.button);
        bigButton.fontSize = bigLabel.fontSize;

        GUILayout.BeginVertical();

        GUILayout.Label("Level Export", bigLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export", bigButton)) BakeScene();
        GUILayout.EndHorizontal();

        if(hasResult)
        {
            GUILayout.BeginHorizontal();
            bool failed = LevelSerializer.failCause != "";
            string str = failed ? "<color=red>Failed</color>" : "<color=green>Success</color>";
            GUILayout.Label("Status: " + str, bigLabel);
            GUILayout.EndHorizontal();
            if(failed)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LevelSerializer.failCause, smallLabel);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Export Size: " + LevelSerializer.levelSize + "kb" , smallLabel);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("File Location: " + ("Assets/" + LevelSerializer.GetCurrentSceneLevelId().Replace('_', ' ') + ".level"), smallLabel);
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }

    static void ReloadCurrentScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.OpenScene(scene.path);
    }

    public static void BakeScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        if (scene.isDirty)
        {
            if(EditorUtility.DisplayDialog(
                "OK to lose changes?", 
                "You must save your level before exporting. If you click OK you will LOSE UNSAVED WORK. Do you want to lose your current changes?", 
                "Yes, DELETE MY CHANGES AND EXPORT", 
                "No, keep my changes and don't export.") == false)
                return;
        }

        ReloadCurrentScene();

        hasResult = false;
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
        LevelSerializer.failCause = "";
        Debug.Log("Starting Level Export");

        if(MapComponents.GetNumOf("SpawnPoint") == 0)
        {
            if (MapComponents.GetNumOf("StartPad") != 1)
            {
                LevelSerializer.failCause = "Singleplayer Level needs one StartPad!";
            }
            if (MapComponents.GetNumOf("EndPad") != 1)
            {
                LevelSerializer.failCause = "Singleplayer Level needs one EndPad!";
            }
            if (FindObjectsOfType<LevelTiming>().Length != 1)
            {
                LevelSerializer.failCause = "Singleplayer Level needs one Level Times object!";
            }
        }  
        else
        {
            if (MapComponents.GetNumOf("StartPad") > 0)
            {
                LevelSerializer.failCause = "Multiplayer Maps can't have StartPads.";
            }
            if (MapComponents.GetNumOf("EndPad") > 0)
            {
                LevelSerializer.failCause = "Multiplayer Maps can't have EndPads.";
            }
            if (FindObjectsOfType<LevelTiming>().Length > 0)
            {
                LevelSerializer.failCause = "Multiplayer Maps can't have timers.";
            }
        }

        if (MapComponents.GetNumOf("LevelBounds") != 1)
        {
            LevelSerializer.failCause = "Level needs one Level Bounds!";
        }
        else
        {
            GameObject bounds = MapComponents.FindFixed("LevelBounds");
            BoxCollider box = bounds.GetComponent<BoxCollider>();
            if (box.size.x > 4096 || box.size.y > 4096 || box.size.z > 4096)
                LevelSerializer.failCause = "Map is too large! Ensure leve bounds dimensions are under 4096.";
        }

        var serializer = new LevelSerializer();
        ByteStream levelBits = new ByteStream();
        if (LevelSerializer.failCause.Length == 0)
            serializer.Serialize(ref levelBits);

        if(LevelSerializer.failCause.Length == 0)
        {
            // TODO: allow user to choose export path
            // string filePath = /* "Assets/" */ "C:/Users/Ryan/AppData/LocalLow/Bad Habit/MarbleItUpUltra/CustomLevels/Test/" + LevelSerializer.GetCurrentSceneLevelId() + ".level";
            string filePath = "Assets/" + LevelSerializer.GetCurrentSceneLevelId() + ".level";

            FileStream file;
            if (!File.Exists(filePath))
                file = File.Create(filePath);
            else
                file = File.Open(filePath, FileMode.Truncate, FileAccess.Write);
            file.Write(levelBits.Buffer, 0, levelBits.Position);
            file.Close();

            Debug.Log("<color=green>Level Exported:</color> " + filePath);
        }
        else
        {
            if(LevelSerializer.failCause.Contains("Lightmap"))
                    Application.OpenURL("https://github.com/MarbleItUp/MIULevelCreationKit/blob/master/README.md#lightmaps");
            Debug.Log("<color=#ff3232>Export Failed:</color> " + LevelSerializer.failCause);
        }

        hasResult = true;

        ReloadCurrentScene();
    }

    private Transform TestContainer = null;
}
