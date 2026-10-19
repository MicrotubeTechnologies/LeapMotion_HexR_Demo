using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PatientHeartManager : MonoBehaviour
{
    #region Parameters
    public TextMeshProUGUI HeartTypeText; // Description text of the type of heart
    public List<GameObject> lifestyleChoice = new List<GameObject>();
    public AudioClip NormalAudio1, NormalAudio2, HypertensiveAudio1, HypertensiveAudio2, IschemicAudio1, IschemicAudio2, AthleticAudio1, AthleticAudio2, Completed, Sad;
    public AudioSource audioSource;
    public SubtitleHelper subtitleHelper; // Create the subtitle effect
    public GameObject HypertensiveHeartModel, NormalHeatModel, IschemicModel, AtheleticModel, Recap_GameObject;
    private bool Hyperdone = false, Ischemicdone = false, Athleticdone = false, SadDone = false;
    // Declare a delegate
    private Action currentFunction;
    [HideInInspector]
    public float ThrowCounter = 0;

    [Header("Localization")]
    [Tooltip("Restart the segment that is playing in the new language when the language is switched mid-narration.")]
    public bool replayNarrationOnLanguageChange = true;

    // What is on screen right now, so it can be re-localised when the language flips.
    private string currentNarrationKey;
    private string currentHeartLabelEnglish;

    // The heading's original Latin font, so English can be restored on switching back.
    private TMP_FontAsset heartLabelDefaultFont;
    #endregion

    private void Awake()
    {
        // Captured before anything localises the label, so the English font is not lost.
        if (HeartTypeText != null)
            heartLabelDefaultFont = HeartTypeText.font;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Stop();
            StartCoroutine(Invoker());

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }


        if(ThrowCounter > 10 && !audioSource.isPlaying && !SadDone)
        {
            SadTrigger();
        }

    }
    public void Quit()
    {
        // If running in the Unity editor
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                            // If running as a build
                            Application.Quit();
        #endif
    }

    private void SadTrigger()
    {
        SadDone = true;
        Narrate(HeartNarration.Sad, Sad);
    }
    public void GameStart()
    {
        AudioListener.pause = false;  // resumes
        Hyperdone = false;
        Ischemicdone = false;
        Athleticdone = false;
        SadDone = false;
        ThrowCounter = 0;      // else a previous run's throws fire the sad line straight away
        currentFunction = null; // else a stale Space press invokes last run's trigger
        Recap_GameObject.SetActive(false);
        audioSource.Stop();
        NormalHeartStart();
    }

    #region Normal Heart
    public void NormalHeartStart()
    {
        HeartType(HeartNarration.LabelNormal, NormalHeatModel);
        HideLifestyleChoices();
        Narrate(HeartNarration.NormalIntro, NormalAudio1);
        currentFunction = NormalHeartTrigger;
    }
    public void NormalHeartTrigger()
    {
        currentFunction = null;
        Narrate(HeartNarration.NormalTouch, NormalAudio2);
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));

    }

    #endregion

    #region Hypertensive
    public void HypertensiveTrigger1()
    {
        HideLifestyleChoices();
        //enlarged, floppy heart, weak pulsation.
        HeartType(HeartNarration.LabelHypertensive, HypertensiveHeartModel);
        Narrate(HeartNarration.HypertensiveIntro, HypertensiveAudio1);
        currentFunction = HypertensiveTrigger2;
    }

    public void HypertensiveTrigger2()
    {
        currentFunction = null;
        Hyperdone = true;
        Narrate(HeartNarration.HypertensiveTouch, HypertensiveAudio2);
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));
    }
    #endregion

    #region Ischemic
    public void IschemicTrigger1()
    {
        //weak beats, slight discoloration.
        HeartType(HeartNarration.LabelIschemic, IschemicModel);
        HideLifestyleChoices();
        Narrate(HeartNarration.IschemicIntro, IschemicAudio1);
        currentFunction = IschemicTrigger2;
    }
    public void IschemicTrigger2()
    {
        currentFunction = null;
        Narrate(HeartNarration.IschemicTouch, IschemicAudio2);
        Ischemicdone = true;
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));
    }

    #endregion

    #region Athletic
    public void AthleticTrigger1()
    {
        //weak beats, slight discoloration.
        HeartType(HeartNarration.LabelAthletic, AtheleticModel);
        HideLifestyleChoices();
        Narrate(HeartNarration.AthleticIntro, AthleticAudio1);
        currentFunction = AthleticTrigger2;
    }

    public void AthleticTrigger2()
    {
        currentFunction = null;
        Athleticdone = true; // set here, like Hyperdone/Ischemicdone - only once the heart is actually touched
        HideLifestyleChoices();
        Narrate(HeartNarration.AthleticTouch, AthleticAudio2);
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));
    }

    #endregion

    private void AllDone()
    {
        HeartType(HeartNarration.LabelRecap, Recap_GameObject);
        Narrate(HeartNarration.Completed, Completed);
        Recap_GameObject.SetActive(true);
    }
    #region Helper

    /// <summary>
    /// Plays a narration segment in the active language: the Mandarin pack supplies
    /// the clip and the retimed subtitles, and anything it does not translate falls
    /// back to <paramref name="englishClip"/> and the English script.
    /// </summary>
    private void Narrate(string key, AudioClip englishClip)
    {
        AudioClip clip = englishClip;
        SubtitleLine[] lines = HeartNarration.EnglishLines(key);
        LocalizationManager.ResolveNarration(key, ref clip, ref lines);

        currentNarrationKey = key;

        audioSource.clip = clip;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
    }

    /// <summary>English clip wired up in the inspector for a narration key.</summary>
    private AudioClip EnglishClipFor(string key)
    {
        switch (key)
        {
            case HeartNarration.NormalIntro: return NormalAudio1;
            case HeartNarration.NormalTouch: return NormalAudio2;
            case HeartNarration.HypertensiveIntro: return HypertensiveAudio1;
            case HeartNarration.HypertensiveTouch: return HypertensiveAudio2;
            case HeartNarration.IschemicIntro: return IschemicAudio1;
            case HeartNarration.IschemicTouch: return IschemicAudio2;
            case HeartNarration.AthleticIntro: return AthleticAudio1;
            case HeartNarration.AthleticTouch: return AthleticAudio2;
            case HeartNarration.Completed: return Completed;
            case HeartNarration.Sad: return Sad;
            default: return null;
        }
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        // Re-localise the heading without touching which heart is visible.
        if (HeartTypeText != null && !string.IsNullOrEmpty(currentHeartLabelEnglish))
        {
            HeartTypeText.text = LocalizationManager.Text(HeartNarration.LabelKey(currentHeartLabelEnglish), currentHeartLabelEnglish);
            ApplyHeartLabelFont();
        }

        // A half-finished English voice over cannot be resumed in Mandarin, so
        // restart the current segment in the new language.
        if (replayNarrationOnLanguageChange && !string.IsNullOrEmpty(currentNarrationKey))
            Narrate(currentNarrationKey, EnglishClipFor(currentNarrationKey));
    }

    private IEnumerator WaitForAudioToFinish(Action onFinished)
    {
        // Wait while the audio is playing
        while (audioSource.isPlaying)
        {
            yield return null; // wait for the next frame
        }
        // Call the function you passed in
        onFinished?.Invoke();
    }
    private IEnumerator Invoker()
    {
        yield return new WaitForSeconds(0.2f); // wait for the next frame
        // Call the function you passed in
        currentFunction?.Invoke();
    }
    public void HeartType(string HeartCondition, GameObject HeartSelected)
    {
        HypertensiveHeartModel.SetActive(false);
        NormalHeatModel.SetActive(false);
        IschemicModel.SetActive(false);
        AtheleticModel.SetActive(false);
        HeartName(HeartCondition);
        HeartSelected.SetActive(true);
    }
    private void ShowLifestylechoices()
    {
        NormalHeatModel.SetActive(false);
        HypertensiveHeartModel.SetActive(false);
        IschemicModel.SetActive(false);
        AtheleticModel.SetActive(false);
        //HeartType("Normal Heart", NormalHeatModel);
        lifestyleChoice[0].SetActive(true);
        lifestyleChoice[1].SetActive(true);
        lifestyleChoice[2].SetActive(true);
        // Dont show object that is already completed
        if (Hyperdone)
        {
            lifestyleChoice[1].SetActive(false);
        }

        if (Athleticdone)
        {
            lifestyleChoice[0].SetActive(false);
        }

        if (Ischemicdone)
        {
            lifestyleChoice[2].SetActive(false);
        }

        if(Hyperdone && Athleticdone && Ischemicdone)
        {
            AllDone();
        }
    }
    private void HideLifestyleChoices()
    {
        foreach (GameObject child in lifestyleChoice)
        {
            child.SetActive(false);
        }
    }
    /// <summary>
    /// Sets the heading. The English string doubles as the localisation key
    /// (e.g. "Normal" -> "label.normal"), so inspector-wired calls keep working.
    /// </summary>
    public void HeartName(string HeartName)
    {
        currentHeartLabelEnglish = HeartName;
        HeartTypeText.text = LocalizationManager.Text(HeartNarration.LabelKey(HeartName), HeartName);
        ApplyHeartLabelFont();
    }

    /// <summary>
    /// This heading is written straight from code rather than through a
    /// LocalizedTMPText, so it needs the language font applied by hand -
    /// otherwise the Chinese renders as empty boxes in the default Latin font.
    /// </summary>
    private void ApplyHeartLabelFont()
    {
        if (HeartTypeText == null)
            return;

        TMP_FontAsset font = LocalizationManager.CurrentFont;
        if (font == null)
            font = heartLabelDefaultFont;

        if (font != null)
            HeartTypeText.font = font;
    }
    #endregion
}
