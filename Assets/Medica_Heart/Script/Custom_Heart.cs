using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_Heart : MonoBehaviour
{
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
                CurrentStep = 2;
            }
        }

    }
}
