using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexR;

public class SparrowFeet : MonoBehaviour
{
    public Animator animator;
    [Tooltip("Idle states are named Idle_A/B/C - there is no plain \"Idle\" state.")]
    public string idleState = "Idle_A";
    public string flyState = "Fly";
    public AudioSource AudioSource;

    [Header("Shared bird (optional)")]
    [Tooltip("Assign when this feet zone shares the SAME physical bird's Animator as other zones (head, belly, etc.). When set, Fly always takes priority over hand-touch reactions while airborne - the owner arbitrates the rest. Leave empty for a standalone sparrow with its own Animator.")]
    public SparrowAnimatorOwner owner;

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
        if (owner != null) owner.NotifyFlyEnd();
        else animator.CrossFade(idleState, 0.12f);
        if (AudioSource != null) AudioSource.Stop();
    }
    public void Fly()
    {
        if (owner != null) owner.NotifyFlyStart(flyState);
        else animator.CrossFade(flyState, 0.12f);
        if (AudioSource != null) AudioSource.Play();
    }
}
