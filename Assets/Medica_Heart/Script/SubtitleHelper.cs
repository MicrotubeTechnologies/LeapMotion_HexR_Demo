using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct SubtitleLine
{
    public string text;
    public float startTime; // seconds when this line should appear
}

public class SubtitleHelper : MonoBehaviour
{
    public static SubtitleHelper Instance { get; private set; }

    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private int maxCharsPerLine = 50;

    [Tooltip("Chinese glyphs are double width and there are no spaces to wrap on, so CJK subtitles use their own, shorter line length.")]
    [SerializeField] private int maxCjkCharsPerLine = 22;

    private Coroutine fadeCoroutine;
    private AudioSource currentAudio;
    private Queue<SubtitleLine> subtitleQueue;
    private bool isPlaying = false;
    private TMP_FontAsset defaultFont;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            subtitleQueue = new Queue<SubtitleLine>();
            if (subtitleText != null)
                defaultFont = subtitleText.font;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += ApplyLanguageFont;
        ApplyLanguageFont(LocalizationManager.CurrentLanguage);
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= ApplyLanguageFont;
    }

    /// <summary>Swaps the subtitle font so the active language actually has glyphs to render.</summary>
    private void ApplyLanguageFont(GameLanguage language)
    {
        if (subtitleText == null)
            return;

        TMP_FontAsset font = LocalizationManager.CurrentFont;
        if (font == null)
            font = defaultFont;

        if (font != null)
            subtitleText.font = font;
    }
    private void Update()
    {
        if (isPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            ShowLastSubtitle();
        }
    }

    private void ShowLastSubtitle()
    {
        StopAllCoroutines();
        if (subtitleQueue.Count > 0)
        {
            // Take the last subtitle in the queue
            SubtitleLine last = default;
            foreach (var s in subtitleQueue) last = s;

            string formatted = SplitIntoLines(last.text, maxCharsPerLine);
            subtitleText.text = formatted;
            SetAlpha(1f);
        }

        // End everything
        subtitleQueue.Clear();
        isPlaying = false;
    }
    /// <summary>
    /// Queue subtitles for an audio clip
    /// </summary>
    public void PlaySubtitles(AudioSource audioSource, SubtitleLine[] subtitles)
    {
        if (isPlaying)
            StopAllCoroutines();

        currentAudio = audioSource;
        subtitleQueue.Clear();

        foreach (var line in subtitles)
            subtitleQueue.Enqueue(line);

        isPlaying = true;
        StartCoroutine(PlayQueue());
    }

    private IEnumerator PlayQueue()
    {
        while (subtitleQueue.Count > 0)
        {
            SubtitleLine current = subtitleQueue.Dequeue();
            string formatted = SplitIntoLines(current.text, maxCharsPerLine);

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeSubtitle(formatted, current.startTime));
            yield return new WaitUntil(() => currentAudio.time >= current.startTime || ClipEnded());

            // The clip finished before this cue point, so every remaining line is
            // timed past the end of the audio. Without this the WaitUntil above
            // never comes true (time stops advancing once playback ends) and the
            // queue hangs with a line frozen on screen until the next segment.
            if (currentAudio.time < current.startTime)
                break;
        }

        // Clear text at the end
        isPlaying = false;
    }

    /// <summary>
    /// True once playback has actually finished. AudioListener.pause is the
    /// Escape-key pause, which also reads as "not playing" - that is a pause, not
    /// an ended clip, so subtitles must keep waiting through it.
    /// </summary>
    private bool ClipEnded()
    {
        return !AudioListener.pause && !currentAudio.isPlaying;
    }

    private IEnumerator FadeSubtitle(string text, float startTime)
    {
        subtitleText.text = text;

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(t / fadeDuration);
            yield return null;
        }
        SetAlpha(1f);

        // Wait until next subtitle or end
        bool isLast = subtitleQueue.Count == 0; // check if this is the last subtitle

        if (!isLast)
        {
            SubtitleLine next = subtitleQueue.Peek();

            // Hold on audio time, never on wall time. AudioListener.pause (the
            // Escape key) freezes the voice but not WaitForSeconds, which used to
            // fade this line out and blank the screen mid-sentence while paused.
            // Starting the fade a fade-length early also means it actually
            // completes before the next line replaces it - previously PlayQueue
            // cut the fade off on the same frame it began.
            float holdUntil = next.startTime - fadeDuration;
            yield return new WaitUntil(() => currentAudio.time >= holdUntil || ClipEnded());

            // Playback ended before the next cue, so this is the last line anyone
            // will see for this segment. Leave it up rather than fading to blank.
            if (ClipEnded())
            {
                SetAlpha(1f);
                yield break;
            }

            // Fade out
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                SetAlpha(1f - t / fadeDuration);
                yield return null;
            }
            SetAlpha(0f);
            subtitleText.text = "";
        }
        else
        {
            // For the last subtitle, just leave it fully visible
            SetAlpha(1f);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = subtitleText.color;
        c.a = alpha;
        subtitleText.color = c;
    }

    private string SplitIntoLines(string text, int maxChars)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        // Chinese has no spaces to wrap on, so wrap on character count instead.
        if (ContainsCjk(text))
            return SplitByCharacter(text, LocalizationManager.SubtitleCharsPerLine(maxCjkCharsPerLine));

        maxChars = LocalizationManager.SubtitleCharsPerLine(maxChars);

        string[] words = text.Split(' ');
        List<string> result = new List<string>();
        string line = "";

        foreach (var word in words)
        {
            if ((line + word).Length > maxChars)
            {
                result.Add(line.Trim());
                line = "";
            }
            line += word + " ";
        }

        if (line.Length > 0) result.Add(line.Trim());
        return string.Join("\n", result);
    }

    private static string SplitByCharacter(string text, int maxChars)
    {
        if (maxChars <= 0)
            return text;

        List<string> result = new List<string>();
        string line = "";

        for (int i = 0; i < text.Length; i++)
        {
            line += text[i];

            if (line.Length < maxChars)
                continue;

            // Punctuation stays at the end of a line, never at the start of the next one.
            while (i + 1 < text.Length && IsTrailingPunctuation(text[i + 1]))
            {
                i++;
                line += text[i];
            }

            result.Add(line);
            line = "";
        }

        if (line.Length > 0) result.Add(line);
        return string.Join("\n", result);
    }

    // Codepoints, not literals, so this file is pure ASCII and cannot be mangled by an encoding change.
    private static bool IsTrailingPunctuation(char c)
    {
        return c == (char)0xFF0C || c == (char)0x3002 ||   // fullwidth comma, ideographic full stop
               c == (char)0xFF01 || c == (char)0xFF1F ||   // fullwidth exclamation, question mark
               c == (char)0x3001 || c == (char)0xFF1A || c == (char)0xFF1B ||
               c == ',' || c == '.' || c == '!' || c == '?';
    }

    private static bool ContainsCjk(string text)
    {
        foreach (char c in text)
        {
            if ((c >= (char)0x4E00 && c <= (char)0x9FFF) ||   // CJK Unified Ideographs
                (c >= (char)0x3400 && c <= (char)0x4DBF) ||   // Extension A
                (c >= (char)0x3000 && c <= (char)0x303F) ||   // CJK punctuation
                (c >= (char)0xFF00 && c <= (char)0xFFEF))     // Full-width forms
                return true;
        }
        return false;
    }

    public void ClearSubtitles()
{
    StopAllCoroutines(); // stop any running subtitle coroutines
    subtitleText.text = "";
    SetAlpha(0f);
    isPlaying = false;
    subtitleQueue.Clear();
}
}



