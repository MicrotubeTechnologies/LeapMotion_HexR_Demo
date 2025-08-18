using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexR;

public class SparrowFeet : MonoBehaviour
{
    public Animator animator;
    public AudioSource AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("Table") )
        {
            idle();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Table"))
        {
            Fly();
        }
    }
    public void idle()
    {
        animator.SetTrigger("Idle");
    }
    public void Fly()
    {
        animator.SetTrigger("Fly");
    }
}
