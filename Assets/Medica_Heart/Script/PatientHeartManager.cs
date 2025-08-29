using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PatientHeartManager : MonoBehaviour
{
    public TextMeshProUGUI HeartTypeText;
    public AudioClip Audio1;
    public AudioSource audioSource;
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
        audioSource.clip = Audio1;
        audioSource.Play();
    }
    public void NormalHeartTrigger()
    {
        HeartType("Normal Heart");
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
