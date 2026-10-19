using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Jumps to another demo scene. Put it on a Button and set the target scene by
/// name - it wires its own onClick, so nothing needs setting up in the inspector.
///
/// Scenes are loaded by name rather than build index deliberately: the build
/// order changes as scenes are added and reordered, and an index that silently
/// starts pointing at a different scene is a nasty bug to chase.
/// </summary>
[RequireComponent(typeof(Button))]
public class SceneSwitchButton : MonoBehaviour
{
    [Tooltip("Scene to load, e.g. Patient_Heart. Must be enabled in Build Settings.")]
    public string sceneName;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Switch);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(Switch);
    }

    public void Switch()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneSwitch] No target scene set on " + name, this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            // The usual cause is the scene being present but unticked in Build
            // Settings, which fails at runtime with a much less obvious message.
            Debug.LogError("[SceneSwitch] '" + sceneName + "' is not in Build Settings (or is disabled) - cannot load it.", this);
            return;
        }

        // Audio is paused globally by the Escape menus; a new scene should not
        // inherit that or it starts silent.
        AudioListener.pause = false;

        SceneManager.LoadScene(sceneName);
    }
}
