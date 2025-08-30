using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexR;
public class LifeStyleChoiceHandler : MonoBehaviour
{
    public enum Options { JunkFood, Ciggrate, Exercising, Alcohol }
    public Options LifeStyleChoice;
    public PatientHeartManager patientHeartManager;
    public AudioSource audioSource;
    public SpecialHaptics specialHaptics;
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
            specialHaptics.HapticPressure = 0.8f;
            specialHaptics.InTimer = 0.6f;
            specialHaptics.OutTimer = 0.6f;
        }
        else if(LifeStyleChoice == Options.Ciggrate)
        {
            patientHeartManager.IschemicTrigger1();
            specialHaptics.IrregularHeart();
        }
        else if (LifeStyleChoice == Options.Exercising)
        {

        }
        else if (LifeStyleChoice == Options.Alcohol)
        {

        }
    }

    public void NormalHeart()
    {
        specialHaptics.NormalHeart();
        specialHaptics.HapticPressure = 0.4f;
        specialHaptics.InTimer = 0.5f;
        specialHaptics.OutTimer = 0.5f;
    }
}
