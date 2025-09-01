using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexR;

public class SparrowFeet : MonoBehaviour
{
    public GameObject Head, Body;
    public Animator animator;
    public AudioSource AudioSource;
    public SpecialHaptics specialHaptics;
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
            Head.SetActive(true);
            Body.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Table"))
        {
            Fly();
            FlyBool = true;
            Head.SetActive(false);
            Body.SetActive(false);
        }
    }
    public void idle()
    {
        specialHaptics.gameObject.SetActive(false);
        animator.SetTrigger("Idle");
        AudioSource.Stop(); 
    }
    public void Fly()
    {
        specialHaptics.gameObject.SetActive(true);
        animator.SetTrigger("Fly");
        AudioSource.Play();
    }
}
