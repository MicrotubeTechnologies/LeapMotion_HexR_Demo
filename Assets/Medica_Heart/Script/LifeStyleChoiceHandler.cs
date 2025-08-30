using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStyleChoiceHandler : MonoBehaviour
{
    public enum Options { JunkFood, Ciggrate, Exercising, Alcohol }
    public Options LifeStyleChoice;
    public PatientHeartManager patientHeartManager;
    public AudioSource audioSource;
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
        if (LifeStyleChoice == Options.JunkFood && !audioSource.isPlaying)
        {
            patientHeartManager.HypertensiveTrigger1();
        }
        else if(LifeStyleChoice == Options.Ciggrate)
        {

        }
        else if (LifeStyleChoice == Options.Exercising)
        {

        }
        else if (LifeStyleChoice == Options.Alcohol)
        {

        }
    }
}
