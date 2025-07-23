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

    static string filePath;

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

        var stylePath = new GUIStyle(GUI.skin.textField);
        stylePath.alignment = TextAnchor.MiddleLeft;

        GUILayout.BeginVertical();

        ObjectCreator.DrawHeader("Level Exporter");

        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(true);
        GUILayout.Button(new GUIContent(PlayerPrefs.GetString("prefExportPath", "No Export Location Selected"), null, PlayerPrefs.GetString("prefExportPath", "")), stylePath, GUILayout.MinWidth(0), GUILayout.MaxWidth(9999));
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("FolderOpened On Icon", "|Open Folder")), bigButton, GUILayout.Width(24), GUILayout.Height(18))) OpenExportPath();
        if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_SaveAs", "|Choose Export Folder")), bigButton, GUILayout.Width(24), GUILayout.Height(18))) ChangeExportPath();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export", bigButton)) BakeScene();
        GUILayout.EndHorizontal();

        if(hasResult)
        {
            GUILayout.BeginHorizontal();
            bool failed = LevelSerializer.failCause != "";
            string str = failed ? "<color=#AA3333>Failed</color>" : "<color=#33AA33>Success</color>";
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
                GUILayout.Label("File Size: " + LevelSerializer.levelSize + " KB" , smallLabel);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("File Location: " + filePath, smallLabel);
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }

    public static void BakeScene()
    {
        ClearConsole();

        hasResult = false;
        LevelSerializer.failCause = "";
        Debug.Log("Starting Level Export");

        if (MapComponents.GetNumOf("StartPad") != 1)
            LevelSerializer.failCause = "Level needs one StartPad!";
            
        if (MapComponents.GetNumOf("EndPad") != 1)
            LevelSerializer.failCause = "Level needs one EndPad!";
            
        if (FindObjectsOfType<LevelTiming>().Length != 1)
            LevelSerializer.failCause = "Level needs one LevelTiming object!";

        // Note: You CAN export levels with more, and they will run, but we take no responsibility for their
        // correct functioning.
        if (FindObjectsOfType<ElevatorMover>().Length > 127)
            LevelSerializer.failCause = "Levels can only have 128 movers maximum.";

        if (MapComponents.GetNumOf("LevelBounds") != 1)
        {
            LevelSerializer.failCause = "Level needs one LevelBounds volume!";
        }
        else
        {
            GameObject bounds = MapComponents.FindFixed("LevelBounds");
            BoxCollider box = bounds.GetComponent<BoxCollider>();
            if (box.size.x > 4096 || box.size.y > 4096 || box.size.z > 4096)
                LevelSerializer.failCause = "Map is too large! Ensure level bounds are under 4096 units in size.";
        }

        var serializer = new LevelSerializer();
        ByteStream levelBits = new ByteStream();
        if (LevelSerializer.failCause.Length == 0)
            serializer.Serialize(ref levelBits);

        if(LevelSerializer.failCause.Length == 0)
        {
            // if no export path has been specified, ask now.
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("prefExportPath", "")))
                ChangeExportPath();
            
            filePath = PlayerPrefs.GetString("prefExportPath", "Assets") + "/" + LevelSerializer.GetCurrentSceneLevelId() + ".level";

            FileStream file;
            if (!File.Exists(filePath))
                file = File.Create(filePath);
            else
                file = File.Open(filePath, FileMode.Truncate, FileAccess.Write);
            file.Write(levelBits.Buffer, 0, levelBits.Position);
            file.Close();

            Debug.Log("<b><color=#33AA33>Level Exported:</color></b> " + filePath);
        }
        else
        {
            //if(LevelSerializer.failCause.Contains("Lightmap"))
            //        Application.OpenURL("https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/wiki/Lightmap-Settings");
            Debug.Log("<b><color=#AA3333>Export Failed:</color></b> " + LevelSerializer.failCause);
        }

        hasResult = true;
    }

    private static void OpenExportPath()
    {
        string path = PlayerPrefs.GetString("prefExportPath", "");

        if (!String.IsNullOrEmpty(path))
            EditorUtility.OpenWithDefaultApp(path);
    }

    private static void ChangeExportPath()
    {
        string path = EditorUtility.OpenFolderPanel("Choose Export Folder", PlayerPrefs.GetString("prefExportPath", ""), "");

        if (!String.IsNullOrEmpty(path))
            PlayerPrefs.SetString("prefExportPath", path);
    }

    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
