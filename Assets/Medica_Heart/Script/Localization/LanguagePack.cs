using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Languages the Patient Heart experience can run in.
/// </summary>
public enum GameLanguage
{
    English,
    Mandarin
}

/// <summary>
/// One UI / label string in a single language.
/// </summary>
[System.Serializable]
public class LocalizedString
{
    public string key;
    [TextArea(1, 3)] public string value;
}

/// <summary>
/// One narration segment (voice over + its subtitles) in a single language.
/// Timings are per language, because the Mandarin recording will not line up
/// with the English one.
/// </summary>
[System.Serializable]
public class LocalizedNarration
{
    public string key;

    [Tooltip("Voice over for this segment. Leave empty to keep using the English clip.")]
    public AudioClip clip;

    [Tooltip("Optional .srt / .vtt subtitle file for this clip. When set it replaces the hand-typed lines below, " +
             "so timings can come straight out of a transcription tool instead of being measured by hand.")]
    public TextAsset subtitleFile;

    [Tooltip("Subtitles for this segment. startTime is seconds from the start of the clip. Leave empty to keep the English subtitles.")]
    public List<SubtitleLine> lines = new List<SubtitleLine>();

    [System.NonSerialized] private SubtitleLine[] parsedFile;
    [System.NonSerialized] private bool parseAttempted;

    /// <summary>
    /// The subtitles to actually play: the subtitle file when one is assigned,
    /// otherwise the hand-typed lines.
    /// </summary>
    public SubtitleLine[] ResolveLines()
    {
        if (subtitleFile != null)
        {
            if (!parseAttempted)
            {
                parseAttempted = true;
                parsedFile = SrtParser.Parse(subtitleFile.text);

                if (parsedFile.Length == 0)
                    Debug.LogWarning("[Localization] No cues parsed from '" + subtitleFile.name + "' for narration '" + key + "'.", subtitleFile);
            }

            if (parsedFile != null && parsedFile.Length > 0)
                return parsedFile;
        }

        return lines != null ? lines.ToArray() : new SubtitleLine[0];
    }

    public void InvalidateParsed()
    {
        parsedFile = null;
        parseAttempted = false;
    }
}

/// <summary>
/// Every piece of translated content for one language, in a single asset.
/// Create with: Assets > Create > Medica Heart > Language Pack.
/// </summary>
[CreateAssetMenu(fileName = "LanguagePack_Mandarin", menuName = "Medica Heart/Language Pack")]
public class LanguagePack : ScriptableObject
{
    public GameLanguage language = GameLanguage.Mandarin;

    [Tooltip("Font applied to every localised TMP text while this pack is active. " +
             "Mandarin REQUIRES a TMP font asset that contains CJK glyphs (LiberationSans does not).")]
    public TMP_FontAsset font;

    [Tooltip("Characters per subtitle line for this language. 0 keeps the SubtitleHelper default. " +
             "Chinese glyphs are double width, so ~20-24 is usually right.")]
    public int subtitleCharsPerLine = 0;

    [Header("Content")]
    public List<LocalizedString> strings = new List<LocalizedString>();
    public List<LocalizedNarration> narrations = new List<LocalizedNarration>();

    private Dictionary<string, string> stringLookup;
    private Dictionary<string, LocalizedNarration> narrationLookup;

    public bool TryGetString(string key, out string value)
    {
        BuildLookups();
        return stringLookup.TryGetValue(key ?? string.Empty, out value) && !string.IsNullOrEmpty(value);
    }

    public bool TryGetNarration(string key, out LocalizedNarration narration)
    {
        BuildLookups();
        return narrationLookup.TryGetValue(key ?? string.Empty, out narration) && narration != null;
    }

    /// <summary>Drops the cached lookups so inspector edits are picked up while playing.</summary>
    public void Invalidate()
    {
        stringLookup = null;
        narrationLookup = null;

        foreach (LocalizedNarration entry in narrations)
        {
            if (entry != null)
                entry.InvalidateParsed();
        }
    }

    private void OnValidate()
    {
        Invalidate();
    }

    private void BuildLookups()
    {
        if (stringLookup != null && narrationLookup != null)
            return;

        stringLookup = new Dictionary<string, string>();
        foreach (LocalizedString entry in strings)
        {
            if (entry == null || string.IsNullOrEmpty(entry.key))
                continue;
            stringLookup[entry.key] = entry.value;
        }

        narrationLookup = new Dictionary<string, LocalizedNarration>();
        foreach (LocalizedNarration entry in narrations)
        {
            if (entry == null || string.IsNullOrEmpty(entry.key))
                continue;
            narrationLookup[entry.key] = entry;
        }
    }
}
