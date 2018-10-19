using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     Autosave scene on run.
/// </summary>
[InitializeOnLoad]
public class AutosaveOnRun
{
    static AutosaveOnRun()
    {
        EditorApplication.playmodeStateChanged = () =>
        {
            var activeScene = SceneManager.GetActiveScene();
            if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying ||
                !activeScene.isDirty) return;
            Debug.Log("Auto-Saving scene before entering Play mode: " + activeScene.name);
            EditorSceneManager.SaveScene(activeScene);
            AssetDatabase.SaveAssets();
        };
    }
}

/// <summary>
///     This script creates a new window in the editor with a autosave function.
///     It is saving your current scene with an interval from 1 minute to 10 minutes.
///     The config window is in on Window/AutoSave
/// </summary>
public class AutoSave : EditorWindow
{
    private readonly string projectPath = Application.dataPath;
    private bool autoSaveScene = true;
    private int intervalScene;
    private bool isStarted;
    private DateTime lastSaveTimeScene = DateTime.Now;
    private bool showMessage = true;

    [MenuItem("Window/AutoSave")]
    private static void Init()
    {
        var saveWindow = (AutoSave) GetWindow(typeof(AutoSave));
        saveWindow.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Info:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Saving to:", "" + projectPath);
        EditorGUILayout.LabelField("Saving scene:", "" + SceneManager.GetActiveScene().name);
        GUILayout.Label("Options:", EditorStyles.boldLabel);
        autoSaveScene = EditorGUILayout.BeginToggleGroup("Auto save", autoSaveScene);
        intervalScene = EditorGUILayout.IntSlider("Interval (minutes)", intervalScene, 1, 10);
        if (isStarted) EditorGUILayout.LabelField("Last save:", "" + lastSaveTimeScene);

        EditorGUILayout.EndToggleGroup();
        showMessage = EditorGUILayout.BeginToggleGroup("Show Message", showMessage);
        EditorGUILayout.EndToggleGroup();
    }


    private void Update()
    {
        if (autoSaveScene)
        {
            if (DateTime.Now.Minute >= lastSaveTimeScene.Minute + intervalScene ||
                DateTime.Now.Minute == 59 && DateTime.Now.Second == 59)
                SaveScene();
        }
        else
        {
            isStarted = false;
        }
    }

    private void SaveScene()
    {
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        lastSaveTimeScene = DateTime.Now;
        isStarted = true;
        if (showMessage)
            Debug.Log("AutoSave saved: " + SceneManager.GetActiveScene().name + " on " + lastSaveTimeScene);
        var repaintSaveWindow = (AutoSave) GetWindow(typeof(AutoSave));
        repaintSaveWindow.Repaint();
    }
}