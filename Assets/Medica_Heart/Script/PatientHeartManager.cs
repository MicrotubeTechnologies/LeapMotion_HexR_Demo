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

    [HideInInspector]
    public float ThrowCounter = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Stop();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }

        if(ThrowCounter > 10 && !audioSource.isPlaying && !SadDone)
        {
            SadTrigger();
        }

    }
    void Quit()
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
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Hey, can you stop throwing my heart away? It's making me sad. ", startTime = Delay }
        };
        SadDone = true;
        audioSource.clip = Sad;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
    }
    public void GameStart()
    {
        Recap_GameObject.SetActive(false);
        NormalHeartStart();
    }

    #region Normal Heart
    public void NormalHeartStart()
    {
        HeartType("Normal", NormalHeatModel);
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

    #endregion

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
        HeartType("Hypertensive", HypertensiveHeartModel);
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
        StartCoroutine(WaitForAudioToFinish(HideLifestyleChoices));
    }

    public void HypertensiveTrigger2()
    {
        Hyperdone = true;
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

    #region Ischemic
    public void IschemicTrigger1()
    {
        //weak beats, slight discoloration.
        HeartType("Ischemic", IschemicModel);
        foreach (GameObject child in lifestyleChoice)
        {
            if (child.name != "Cigrattes")
            {
                child.SetActive(false);
            }
        }
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Take a look at how the heart has changed after prolonged smoking.  ", startTime = Delay },
            new SubtitleLine { text = "Notice the uneven areas, some parts are dimmer than others.", startTime = Delay + 3.5f },
            new SubtitleLine { text = "This represents damage from restricted blood flow.", startTime = Delay + 6.5f },
            new SubtitleLine { text = "This is called an ischemic heart. Smoking damages blood vessels,", startTime = Delay + 11f},
            new SubtitleLine { text = "promotes plaque build-up, and limits oxygen supply to the heart muscle.", startTime = Delay + 14.5f },
            new SubtitleLine { text = "Over time, some areas may weaken or even die off, leaving scars. ", startTime = Delay + 18f },
            new SubtitleLine { text = "Go ahead, reach out and touch the ischemic heart to feel the difference yourself", startTime = Delay + 22f }
        };
        audioSource.clip = IschemicAudio1;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
        StartCoroutine(WaitForAudioToFinish(HideLifestyleChoices));
    }
    public void IschemicTrigger2()
    {
        float Delay = 5.5f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Do you feel that? The heartbeat is uneven, unpredictable, and weaker in some areas. ", startTime = Delay },
            new SubtitleLine { text = "That’s the hallmark of an ischemic heart. Ischemic hearts are at risk of chest pain, ", startTime = Delay + 6f },
            new SubtitleLine { text = "heart attacks, and even heart failure if blood flow is not restored.  ", startTime = Delay +9f },
            new SubtitleLine { text = "The good news is that quitting smoking, eating healthy, and exercising can help prevent further damage and improve heart function. ", startTime = Delay + 17.5f},
            new SubtitleLine { text = "Now let's try another lifestyle choice, shall we?	", startTime = Delay + 18f }

        };
        audioSource.clip = IschemicAudio2;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
        Ischemicdone = true;
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));
    }

    #endregion

    #region Athletic
    public void AthleticTrigger1()
    {
        //weak beats, slight discoloration.
        HeartType("Athletic", AtheleticModel);
        Athleticdone = true;
        foreach (GameObject child in lifestyleChoice)
        {
            if (child.name != "Dumbbell")
            {
                child.SetActive(false);
            }
        }
        float Delay = 4.5f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Now let’s look at what happens when we care for the heart through regular exercise. ", startTime = Delay },
            new SubtitleLine { text = "Notice how this heart is a little bigger and glowing brightly, pumping with strength and efficiency. ", startTime = Delay + 5f },
            new SubtitleLine { text = "This is sometimes called an athletic heart. Unlike a hypertensive heart that grows stiff from pressure", startTime = Delay + 11f },
            new SubtitleLine { text = "an athletic heart grows stronger and more efficient. ", startTime = Delay + 14f},
            new SubtitleLine { text = "Go ahead  reach out and touch the athletic heart to feel the difference yourself.", startTime = Delay + 20f }
        };
        audioSource.clip = AthleticAudio1;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
        StartCoroutine(WaitForAudioToFinish(HideLifestyleChoices));
    }

    public void AthleticTrigger2()
    {
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Do you feel that? Stronger, but still smooth and steady.  ", startTime = Delay },
            new SubtitleLine { text = "That’s the mark of a well-trained, healthy heart.  ", startTime = Delay + 3.5f },
            new SubtitleLine { text = "Regular exercise helps the heart pump more efficiently, lowers blood pressure, improves circulation, and protects against heart disease. ", startTime = Delay +11f },
            new SubtitleLine { text = "Just 30 minutes of activity a day can make a lifetime of difference.  ", startTime = Delay + 16f},
            new SubtitleLine { text = "Now let's try another lifestyle choice, shall we?", startTime = Delay + 18f }

        };
        audioSource.clip = AthleticAudio2;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
        StartCoroutine(WaitForAudioToFinish(ShowLifestylechoices));
    }

    #endregion

    private void AllDone()
    {
        HeartType("Press R to return to menu", Recap_GameObject);
        audioSource.clip = Completed;
        audioSource.Play();
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "You’ve now discovered how lifestyle choices can weaken or strengthen the heart.  ", startTime = Delay },
            new SubtitleLine { text = "From here on, you’re free to explore. Touch each heart again, compare how they feel,  ", startTime = Delay +4f },
            new SubtitleLine { text = "and reflect on the choices that lead to them. ", startTime = Delay +7f },
            new SubtitleLine { text = "Remember the heart you take care of today is the one that carries you into tomorrow.", startTime = Delay + 10f}
        };
        subtitleHelper.PlaySubtitles(audioSource, lines);
        Recap_GameObject.SetActive(true);
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

    public void HeartType(string HeartCondition, GameObject HeartSelected)
    {
        HypertensiveHeartModel.SetActive(false);
        NormalHeatModel.SetActive(false);
        IschemicModel.SetActive(false);
        AtheleticModel.SetActive(false);
        HeartTypeText.text = HeartCondition;
        HeartSelected.SetActive(true);
    }
    private void ShowLifestylechoices()
    {
        HeartType("Normal Heart", NormalHeatModel);
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
    #endregion
}
