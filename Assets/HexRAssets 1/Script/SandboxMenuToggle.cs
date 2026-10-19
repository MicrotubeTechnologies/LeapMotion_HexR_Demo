using UnityEngine;

/// <summary>
/// Brings the start menu back with Escape.
///
/// This has to live on an object that stays active, not on the menu itself - a
/// disabled GameObject gets no Update, so a toggle parented to the canvas could
/// turn it off but never back on.
/// </summary>
public class SandboxMenuToggle : MonoBehaviour
{
    [Tooltip("The menu canvas to show and hide.")]
    public GameObject menu;

    [Tooltip("Key that brings the menu back.")]
    public KeyCode toggleKey = KeyCode.Escape;

    [Tooltip("Escape hides the menu again when it is already open.")]
    public bool toggle = true;

    private void Update()
    {
        if (menu == null || !Input.GetKeyDown(toggleKey))
            return;

        menu.SetActive(toggle ? !menu.activeSelf : true);
    }

    /// <summary>Hook to a button if you want to open the menu without the keyboard.</summary>
    public void ShowMenu()
    {
        if (menu != null) menu.SetActive(true);
    }

    public void HideMenu()
    {
        if (menu != null) menu.SetActive(false);
    }
}
