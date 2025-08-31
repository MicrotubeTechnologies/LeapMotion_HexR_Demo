using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_Heart : MonoBehaviour
{
    // 1 = Normal Heart
    // 2 = Hypertensive Heart
    // 3 = Ischemic Heart
    public enum Options { Normal, Ischemic, Hypertensive, Athletic }
    public Options TypeOfHeart;
    private bool Done = false;
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
            if (TypeOfHeart == Options.Normal && Done)
            {
                patientHeartManager.NormalHeartTrigger();
                Done = true;
            }
            else if(TypeOfHeart == Options.Hypertensive && Done)
            {
                patientHeartManager.HypertensiveTrigger2();
                Done = true;
            }
            else if (TypeOfHeart == Options.Ischemic && Done)
            {
                patientHeartManager.IschemicTrigger2();
                Done = true;
            }
            else if (TypeOfHeart == Options.Athletic && Done)
            {
                patientHeartManager.AthleticTrigger2();
                Done = true;
            }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (!audiosource.isPlaying)
        {
            if (TypeOfHeart == Options.Normal && Done)
            {
                patientHeartManager.NormalHeartTrigger();
                Done = true;
            }
            else if (TypeOfHeart == Options.Hypertensive && Done)
            {
                patientHeartManager.HypertensiveTrigger2();
                Done = true;
            }
            else if (TypeOfHeart == Options.Ischemic && Done)
            {
                patientHeartManager.IschemicTrigger2();
                Done = true;
            }
            else if (TypeOfHeart == Options.Athletic && Done)
            {
                patientHeartManager.AthleticTrigger2();
                Done = true;
            }
        }
    }
}
