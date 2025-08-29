using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PatientHeartManager : MonoBehaviour
{
    public TextMeshProUGUI HeartTypeText;
    public AudioClip Audio1, Audio2;
    public AudioSource audioSource;
    public SubtitleHelper subtitleHelper;
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
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Welcome! Today, we’re going on a journey to understand the human heart. ", startTime = Delay },
            new SubtitleLine { text = "Think of it like a guided museum tour, where you can see, and even feel how your heart works. ", startTime = Delay +4f },
            new SubtitleLine { text = "Before we look at different heart problems, let’s meet the healthy heart the gold standard that we all want to protect. ", startTime = Delay +10f },
            new SubtitleLine { text = "Here it is, your heart when it’s strong and well.It beats about 100,000 times every single day without resting.     ", startTime = Delay + 23f},
            new SubtitleLine { text = "Reach out and touch it.    ", startTime = Delay + 35f }
        };
        audioSource.clip = Audio1;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
    }

    public void NormalHeartTrigger()
    {
        HeartType("Normal Heart");
        float Delay = 4f;
        SubtitleLine[] lines = new SubtitleLine[]
        {
            new SubtitleLine { text = "Do you feel that? That’s the rhythm of life — smooth, steady, and strong.  ", startTime = Delay },
            new SubtitleLine { text = "A healthy heart is like a reliable pump; every beat pushes blood and oxygen through your body.  ", startTime = Delay + 4f },
            new SubtitleLine { text = "Now, here’s the big idea for today ", startTime = Delay +8f },
            new SubtitleLine { text = "the way you live affects whether your heart stays this way or changes for the worse. ", startTime = Delay + 15f},
            new SubtitleLine { text = "Let’s see how everyday choices shape your heart.", startTime = Delay + 20f }
        };
        audioSource.clip = Audio2;
        audioSource.Play();
        subtitleHelper.PlaySubtitles(audioSource, lines);
    }
    public void HypertensiveTrigger()
    {
        //enlarged, floppy heart, weak pulsation.
        HeartType("Hypertensive Heart");
    }
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
    public void HeartType(string HeartCondition)
    {
        HeartTypeText.text = HeartCondition;
    }
}
