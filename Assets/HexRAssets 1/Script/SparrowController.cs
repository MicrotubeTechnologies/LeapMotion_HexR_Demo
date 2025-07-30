using HexR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowController : MonoBehaviour
{
    public Animator animator;
    public PressureTrackerMain RighPTM, LeftPTM;
    public AudioSource AudioSource;
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
        if(other.name.Contains("Palm") && other.name.Contains("R_"))
        {
            Bounce();
            AudioSource.Play();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("Palm") && other.name.Contains("R_"))
        {

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Palm") && other.name.Contains("R_"))
        {
            Idle();
            RighPTM.RemoveAllVibrations();
            AudioSource.Stop();
        }
    }
    public void Bounce()
    {
        Debug.Log("Bouncing");
        animator.SetTrigger("Bounce");
    }
    public void Idle()
    {
        animator.SetTrigger("Idle");
    }
}
