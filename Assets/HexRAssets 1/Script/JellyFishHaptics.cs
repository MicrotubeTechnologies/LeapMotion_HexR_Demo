using HexR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFishHaptics : MonoBehaviour
{
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
        Debug.Log(other.name);
        if (other.gameObject.TryGetComponent(out HapticFingerTrigger hapticFingerTrigger)) // Only triggering this using the Tip of the finger
        {
            other.GetComponent<HapticFingerTrigger>().TriggerVibrationPressure(20,0.3f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out HapticFingerTrigger hapticFingerTrigger)) // Only triggering this using the Tip of the finger
        {
            other.GetComponent<HapticFingerTrigger>().RemoveVibration();
        }
    }
}
