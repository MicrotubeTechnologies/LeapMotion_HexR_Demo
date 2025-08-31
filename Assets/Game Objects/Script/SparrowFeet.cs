using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexR;

public class SparrowFeet : MonoBehaviour
{
    public Animator animator;
    public AudioSource AudioSource;
    private bool FlyBool = false;
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
        if (other.name.Contains("Table") && FlyBool)
        {
            idle();
            FlyBool = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Table"))
        {
            Fly();
            FlyBool = true;
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
