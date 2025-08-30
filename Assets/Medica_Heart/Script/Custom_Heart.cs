using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_Heart : MonoBehaviour
{
    // 1 = Normal Heart
    // 2 = Hypertensive Heart
    // 3 = Ischemic Heart
    private float CurrentStep = 1;
    public AudioSource audiosource;
    public PatientHeartManager patientHeartManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!audiosource.isPlaying)
        {
            if (CurrentStep == 1)
            {
                patientHeartManager.NormalHeartTrigger();
                CurrentStep = 0;
            }
            else if(CurrentStep == 2)
            {
                patientHeartManager.HypertensiveTrigger2();
                CurrentStep = 0;
            }
            else if (CurrentStep == 3)
            {
                patientHeartManager.IschemicTrigger2();
                CurrentStep = 0;
            }
        }

    }

    public void CurrentStepChanger(float newStep)
    {
        CurrentStep = newStep;
    }
}
