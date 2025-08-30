using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PatientHeartManager : MonoBehaviour
{
    public TextMeshProUGUI HeartTypeText; // Description text of the type of heart
    public List<GameObject> lifestyleChoice = new List<GameObject>();
    public AudioClip NormalAudio1, NormalAudio2, HypertensiveAudio1, HypertensiveAudio2;
    public AudioSource audioSource;
    public SubtitleHelper subtitleHelper; // Create the subtitle effect
    public Custom_Heart custom_Heart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart()
    {
        NormalHeartStart();
    }

    public void NormalHeartStart()
    {
        HideLifestyleChoices();
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Welcome! Today, we’re going on a journey to understand the human heart. ", startTime = Delay },
            new SubtitleLine { text = "Think of it like a guided museum tour, where you can see, and even feel how your heart works. ", startTime = Delay +4f },
            new SubtitleLine { text = "Before we look at different heart problems, let’s meet the healthy heart the gold standard that we all want to protect. ", startTime = Delay +13f },
            new SubtitleLine { text = "Here it is, your heart when it’s strong and well.It beats about 100,000 times every single day without resting.     ", startTime = Delay + 23f},
            new SubtitleLine { text = "Reach out and touch it.    ", startTime = Delay + 35f }
        };
        audioSource.clip = NormalAudio1;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
    }
    public void NormalHeartTrigger()
    {
        HeartType("Normal Heart");
        float Delay = 5f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Do you feel that? That’s the rhythm of life — smooth, steady, and strong.  ", startTime = Delay },
            new SubtitleLine { text = "A healthy heart is like a reliable pump; every beat pushes blood and oxygen through your body.  ", startTime = Delay + 6f },
            new SubtitleLine { text = "Now, here’s the big idea for today ", startTime = Delay +8f },
            new SubtitleLine { text = "the way you live affects whether your heart stays this way or changes for the worse. ", startTime = Delay + 14f},
            new SubtitleLine { text = "Let’s see how everyday choices shape your heart.", startTime = Delay + 23f }
        };
        audioSource.clip = NormalAudio2;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));

    }



    #region Hypertensive
    public void HypertensiveTrigger1()
    {
        foreach (GameObject child in lifestyleChoice)
        {
            if(child.name != "JunkFood")
            {
                child.SetActive(false);
            }
        }

        //enlarged, floppy heart, weak pulsation.
        HeartType("Hypertensive Heart");
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Look closely — the heart has changed. Its walls are thicker, its glow is weaker  ", startTime = Delay },
            new SubtitleLine { text = "and the beat looks more forceful but less flexible. This is the transformation into a hypertensive heart.", startTime = Delay + 7f },
            new SubtitleLine { text = "An unhealthy diet — too much salt, sugar, and fat — together with obesity, forces the heart to work overtime. ", startTime = Delay +15f },
            new SubtitleLine { text = "High blood pressure causes the muscles to bulk up, just as lifting heavy weights makes your arms bigger. ", startTime = Delay + 22f},
            new SubtitleLine { text = "But unlike strong arms, a stiff heart isn’t good news. ", startTime = Delay + 25f },
            new SubtitleLine { text = "Go ahead, reach out and touch the hypertensive heart.", startTime = Delay + 28f },
            new SubtitleLine { text = "Notice the difference in how it feels compared to the healthy one.", startTime = Delay + 31f }
        };
        audioSource.clip = HypertensiveAudio1;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
        custom_Heart.CurrentStepChanger(2f);
        StartCoroutine(WaitForAudioToFinish(HideLifestyleChoices));
    }

    public void HypertensiveTrigger2()
    {
        //enlarged, floppy heart, weak pulsation.
        HeartType("Hypertensive Heart");
        float Delay = 6f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Do you feel that? Less bounce, more strain. That’s the silent burden of high blood pressure. ", startTime = Delay },
            new SubtitleLine { text = "Most people don’t even notice it until damage is already happening. ", startTime = Delay + 4f },
            new SubtitleLine { text = "When the heart walls get too thick, the chambers inside shrink, and less blood can be pumped out with each beat.  ", startTime = Delay +13f },
            new SubtitleLine { text = "Over time, this raises the risk of heart failure, stroke, and kidney problems. ", startTime = Delay + 18f},
            new SubtitleLine { text = "The good news: lifestyle changes — less salt, healthier food, regular exercise ", startTime = Delay + 23f },
            new SubtitleLine { text = "can lower blood pressure and help keep your heart flexible and strong. ", startTime = Delay + 29f },
            new SubtitleLine { text = "Now let's try another lifestyle choice, shall we?", startTime = Delay + 33f },
        };
        audioSource.clip = HypertensiveAudio2;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));
    }
    #endregion


    public void IschemicTrigger()
    {
        //weak beats, slight discoloration.
        HeartType("Ischemic Heart");
    }
    public void DilatedTrigger()
    {
        //weak beats, slight discoloration.
        HeartType("Dilated Heart");
    }

    #region Helper

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

    public void HeartType(string HeartCondition)
    {
        HeartTypeText.text = HeartCondition;
    }
    private void ShowLifestylechoices()
    {
        foreach (GameObject child in lifestyleChoice)
        {
            child.SetActive(true);
        }
    }
    private void HideLifestyleChoices()
    {
        foreach (GameObject child in lifestyleChoice)
        {
            child.SetActive(false);
        }
    }
    #endregion
}
