using UnityEngine;
using System.Collections;

public class UIFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    void Start()
    {
        // Start fully visible
        canvasGroup.alpha = 1f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(FadeIn());
            togglePause();
        }
    }
    public void Fadeit()
    {
        StartCoroutine(FadeOut());
    }
    public IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
    }

    public void togglePause()
    {
        Debug.Log("Pausing");
        AudioListener.pause = true;   // pauses all AudioSources
    }
    public void toggleResume()
    {
        StartCoroutine(FadeOut());
        AudioListener.pause = false;  // resumes
    }
}