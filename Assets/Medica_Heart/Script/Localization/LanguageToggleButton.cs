using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drop this on the "EN" button. It flips English &lt;-&gt; Mandarin and keeps its own
/// label in sync, so the button reads EN while the experience is in English and
/// the Chinese label while it is in Mandarin.
///
/// Works with a UGUI <see cref="Button"/> (it wires itself to onClick, no manual
/// setup) and with an Ultraleap Physical Hands button or any other UnityEvent -
/// point that event at <see cref="Toggle"/>.
///
/// Do not also put <see cref="LocalizedTMPText"/> on the button's label; the two
/// would fight over the same text.
/// </summary>
public class LanguageToggleButton : MonoBehaviour
{
    #region Parameters
    [Tooltip("Label to drive. Leave empty to use the first TMP text found on this object or its children.")]
    [SerializeField] private TMP_Text label;

    [Tooltip("Legacy UI Text label, if the button does not use TextMeshPro.")]
    [SerializeField] private Text legacyLabel;

    [Tooltip("True: the button shows the language you are in now (EN while English). " +
             "False: it shows the language you would switch to.")]
    [SerializeField] private bool showCurrentLanguage = true;

    [SerializeField] private string englishLabel = "EN";

    [Tooltip("Label shown for Mandarin. Leave empty to use the default Chinese label.")]
    [SerializeField] private string mandarinLabel = "";

    [Tooltip("Swap the label to the language pack's font while Mandarin is active. " +
             "Without a CJK font the Chinese label renders as empty boxes.")]
    [SerializeField] private bool applyLanguageFont = true;

    private Button uiButton;
    private TMP_FontAsset defaultFont;
    #endregion

    private void Awake()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>(true);

        if (label == null && legacyLabel == null)
            legacyLabel = GetComponentInChildren<Text>(true);

        if (label != null)
            defaultFont = label.font;

        uiButton = GetComponent<Button>();
        if (uiButton != null)
            uiButton.onClick.AddListener(Toggle);
    }

    private void OnDestroy()
    {
        if (uiButton != null)
            uiButton.onClick.RemoveListener(Toggle);
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += Refresh;
        Refresh(LocalizationManager.CurrentLanguage);
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= Refresh;
    }

    /// <summary>Hook this to the button's click / press event.</summary>
    public void Toggle()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("[Localization] No LocalizationManager in the scene - the language button does nothing.", this);
            return;
        }

        LocalizationManager.Instance.ToggleLanguage();
    }

    private void Refresh(GameLanguage language)
    {
        bool mandarinActive = language == GameLanguage.Mandarin;

        // Either show the language you are in, or the one the button switches to.
        bool showMandarinText = showCurrentLanguage ? mandarinActive : !mandarinActive;
        string text = showMandarinText ? MandarinLabel : englishLabel;

        if (label != null)
        {
            label.text = text;

            if (applyLanguageFont)
            {
                // The Chinese label needs the CJK font even when the rest of the UI
                // is still English, so pick the font from the text being shown
                // rather than from the active language.
                TMP_FontAsset font = showMandarinText
                    ? LocalizationManager.FontFor(GameLanguage.Mandarin)
                    : defaultFont;
                if (font == null)
                    font = defaultFont;
                if (font != null)
                    label.font = font;
            }
        }

        if (legacyLabel != null)
            legacyLabel.text = text;
    }

    /// <summary>The Chinese label, built from codepoints so this file stays plain ASCII.</summary>
    private string MandarinLabel
    {
        get
        {
            if (!string.IsNullOrEmpty(mandarinLabel))
                return mandarinLabel;
            return new string(new[] { (char)0x4E2D, (char)0x6587 });
        }
    }
}
