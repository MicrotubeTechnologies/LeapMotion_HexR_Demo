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

    private Coroutine fadeCoroutine;
    private AudioSource currentAudio;
    private Queue<SubtitleLine> subtitleQueue;
    private bool isPlaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            subtitleQueue = new Queue<SubtitleLine>();
        }
        else
        {
            Destroy(gameObject);
        }
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
            yield return new WaitUntil(() => currentAudio.time >= current.startTime);
        }

        // Clear text at the end
        isPlaying = false;
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
        float waitTime = 0f;
        bool isLast = subtitleQueue.Count == 0; // check if this is the last subtitle

        if (!isLast)
        {
            SubtitleLine next = subtitleQueue.Peek();
            waitTime = next.startTime - currentAudio.time;
            yield return new WaitForSeconds(waitTime);

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

    public void ClearSubtitles()
{
    StopAllCoroutines(); // stop any running subtitle coroutines
    subtitleText.text = "";
    SetAlpha(0f);
    isPlaying = false;
    subtitleQueue.Clear();
}
}



