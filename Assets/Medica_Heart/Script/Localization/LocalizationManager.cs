using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Single switch that flips the whole Patient Heart experience between English
/// and Mandarin - on-screen text, subtitles, subtitle font and the voice over
/// clips.
///
/// Setup:
///   1. Drop this on a GameObject in the scene (an empty "Localization" object is fine).
///   2. Assign the Mandarin <see cref="LanguagePack"/> asset to <c>mandarinPack</c>.
///   3. Wire a UI button / toggle to <see cref="SetMandarin"/>, <see cref="SetEnglish"/>
///      or <see cref="ToggleLanguage"/>.
///
/// English needs no pack: anything the active pack does not translate falls back
/// to the English text and clips already wired up in the scene, so the build
/// keeps working before the Mandarin assets arrive.
/// </summary>
[DefaultExecutionOrder(-100)]
public class LocalizationManager : MonoBehaviour
{
    #region Parameters
    public static LocalizationManager Instance { get; private set; }

    /// <summary>Raised after the language changed. Listeners should re-read their text.</summary>
    public static event Action<GameLanguage> OnLanguageChanged;

    [SerializeField] private GameLanguage startLanguage = GameLanguage.English;

    [Tooltip("Translated content for Mandarin. Assign the LanguagePack asset here once the Chinese audio is ready.")]
    [SerializeField] private LanguagePack mandarinPack;

    [Tooltip("Optional. Only needed if you also want the English text to live in an asset instead of in the scene / code.")]
    [SerializeField] private LanguagePack englishPack;

    [Tooltip("Remember the chosen language between runs (PlayerPrefs).")]
    [SerializeField] private bool rememberChoice = true;

    [SerializeField] private bool keepAcrossScenes = false;

    [Tooltip("Optional hotkey for testing the switch before the UI button exists. KeyCode.None disables it.")]
    [SerializeField] private KeyCode toggleKey = KeyCode.None;

    private const string PrefKey = "MedicaHeart.Language";

    private GameLanguage current = GameLanguage.English;
    #endregion

    #region Static access
    public static GameLanguage CurrentLanguage => Instance != null ? Instance.current : GameLanguage.English;

    public static bool IsMandarin => CurrentLanguage == GameLanguage.Mandarin;

    /// <summary>Font the active language wants, or null to keep whatever the text already uses.</summary>
    public static TMP_FontAsset CurrentFont
    {
        get
        {
            LanguagePack pack = ActivePack;
            return pack != null ? pack.font : null;
        }
    }

    /// <summary>
    /// Font for a specific language, whether or not it is the active one. Needed
    /// by the language button, which may have to render a Chinese label while the
    /// rest of the UI is still English.
    /// </summary>
    public static TMP_FontAsset FontFor(GameLanguage language)
    {
        if (Instance == null)
            return null;

        LanguagePack pack = language == GameLanguage.Mandarin ? Instance.mandarinPack : Instance.englishPack;
        return pack != null ? pack.font : null;
    }

    private static LanguagePack ActivePack
    {
        get
        {
            if (Instance == null)
                return null;
            return Instance.current == GameLanguage.Mandarin ? Instance.mandarinPack : Instance.englishPack;
        }
    }

    /// <summary>
    /// Returns the translated string for <paramref name="key"/>, or
    /// <paramref name="english"/> when the active language has no translation.
    /// </summary>
    public static string Text(string key, string english)
    {
        LanguagePack pack = ActivePack;
        if (pack != null && pack.TryGetString(key, out string translated))
            return translated;
        return english;
    }

    /// <summary>
    /// Swaps <paramref name="clip"/> and <paramref name="lines"/> for the active
    /// language's version of the narration segment. Anything the pack does not
    /// provide is left untouched, so a missing Mandarin clip simply plays the
    /// English one instead of playing silence.
    /// </summary>
    /// <returns>True if anything was replaced.</returns>
    public static bool ResolveNarration(string key, ref AudioClip clip, ref SubtitleLine[] lines)
    {
        LanguagePack pack = ActivePack;
        if (pack == null || !pack.TryGetNarration(key, out LocalizedNarration narration))
            return false;

        bool replaced = false;

        if (narration.clip != null)
        {
            clip = narration.clip;
            replaced = true;
        }

        SubtitleLine[] resolved = narration.ResolveLines();
        if (resolved.Length > 0)
        {
            lines = resolved;
            replaced = true;
        }

        return replaced;
    }

    /// <summary>Characters per subtitle line for the active language.</summary>
    public static int SubtitleCharsPerLine(int fallback)
    {
        LanguagePack pack = ActivePack;
        if (pack != null && pack.subtitleCharsPerLine > 0)
            return pack.subtitleCharsPerLine;
        return fallback;
    }
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (keepAcrossScenes)
            DontDestroyOnLoad(gameObject);

        current = startLanguage;
        if (rememberChoice && PlayerPrefs.HasKey(PrefKey))
        {
            string saved = PlayerPrefs.GetString(PrefKey);
            if (Enum.TryParse(saved, out GameLanguage parsed))
                current = parsed;
        }
    }

    private void Start()
    {
        // Broadcast once so everything that subscribed in Awake/OnEnable picks up
        // the starting language (important when it was restored from PlayerPrefs).
        Broadcast();
    }

    private void Update()
    {
        if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
            ToggleLanguage();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    #region Public API (hook these to UI buttons)
    public void SetEnglish()
    {
        SetLanguage(GameLanguage.English);
    }

    public void SetMandarin()
    {
        SetLanguage(GameLanguage.Mandarin);
    }

    public void ToggleLanguage()
    {
        SetLanguage(current == GameLanguage.English ? GameLanguage.Mandarin : GameLanguage.English);
    }

    /// <summary>For UnityEvents / dropdowns that can only pass an int (0 = English, 1 = Mandarin).</summary>
    public void SetLanguageIndex(int index)
    {
        SetLanguage(index == 1 ? GameLanguage.Mandarin : GameLanguage.English);
    }

    public void SetLanguage(GameLanguage language)
    {
        if (language == current)
            return;

        if (language == GameLanguage.Mandarin && mandarinPack == null)
            Debug.LogWarning("[Localization] No Mandarin LanguagePack assigned - text and audio will stay English.", this);

        current = language;

        if (rememberChoice)
        {
            PlayerPrefs.SetString(PrefKey, current.ToString());
            PlayerPrefs.Save();
        }

        Broadcast();
    }
    #endregion

    private void Broadcast()
    {
        LanguagePack pack = ActivePack;
        if (pack != null)
            pack.Invalidate(); // pick up inspector edits made while playing

        OnLanguageChanged?.Invoke(current);
    }
}
