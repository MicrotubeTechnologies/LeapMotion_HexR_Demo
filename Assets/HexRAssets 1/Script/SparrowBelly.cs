using HexR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowBelly : MonoBehaviour
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
        if (other.name.Contains("Index") && other.name.Contains("R_"))
        {
            Attack();
            AudioSource.Play();
        }
    }
    private void OnTriggerStay(Collider other)
    {

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Index") && other.name.Contains("R_"))
        {
            Idle();
            AudioSource.Stop();
        }
    }
    public void Attack()
    {
        Debug.Log("Bouncing");
        animator.SetTrigger("Attack");
    }
    public void Idle()
    {
        animator.SetTrigger("Idle");
    }


}

