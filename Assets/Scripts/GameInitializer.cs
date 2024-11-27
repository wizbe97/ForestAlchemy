#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameManagerSO gameManager;

    private void Start()
    {
        gameManager.saveManager.LoadAllData();

#if UNITY_EDITOR
        // Hook into Play Mode state changes when in the editor.
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private void OnApplicationQuit()
    {
        gameManager.saveManager.AutoSaveAll();
    }

#if UNITY_EDITOR
    // This function is called when the play mode state changes.
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Trigger AutoSave when exiting Play Mode
            gameManager.saveManager.AutoSaveAll();
        }
    }
#endif

    // Unsubscribe from the play mode state changes when the object is destroyed.
    private void OnDestroy()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }
}
