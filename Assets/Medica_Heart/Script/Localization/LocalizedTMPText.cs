using TMPro;
using UnityEngine;

/// <summary>
/// Put this on any TextMeshPro label that should follow the language switch
/// (menu buttons, titles, instructions...).
///
/// Leave <see cref="key"/> empty and it derives one from the English text that
/// is already in the label, so you can add it to existing UI without renaming
/// anything.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class LocalizedTMPText : MonoBehaviour
{
    [Tooltip("Key to look up in the LanguagePack. Leave empty to derive it from the English text below.")]
    [SerializeField] private string key;

    [Tooltip("English text. Leave empty to capture whatever the label already shows.")]
    [TextArea(1, 4)]
    [SerializeField] private string englishText;

    [Tooltip("Also swap the font to the one the language pack specifies (needed for CJK glyphs).")]
    [SerializeField] private bool applyLanguageFont = true;

    private TMP_Text label;
    private TMP_FontAsset defaultFont;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
        defaultFont = label.font;

        if (string.IsNullOrEmpty(englishText))
            englishText = label.text;

        if (string.IsNullOrEmpty(key))
            key = HeartNarration.LabelKey(englishText);
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

    private void Refresh(GameLanguage language)
    {
        if (label == null)
            return;

        label.text = LocalizationManager.Text(key, englishText);

        if (applyLanguageFont)
        {
            TMP_FontAsset font = LocalizationManager.CurrentFont;
            label.font = font != null ? font : defaultFont;
        }
    }

    /// <summary>Change the English source text at runtime and re-localise it.</summary>
    public void SetEnglishText(string english)
    {
        englishText = english;
        key = HeartNarration.LabelKey(english);
        Refresh(LocalizationManager.CurrentLanguage);
    }
}
