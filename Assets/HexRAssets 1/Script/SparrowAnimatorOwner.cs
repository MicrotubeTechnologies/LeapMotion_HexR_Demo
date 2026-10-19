using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arbitrates one shared Animator (and optional shared AudioSource) across
/// several zones on the same physical bird - hand-touch zones like
/// SparrowController (head, "Bounce") and SparrowBelly ("Attack"), plus
/// SparrowFeet's Table-based Fly/Idle behaviour.
///
/// Priority rules:
///  - Fly always wins while airborne. Hand touches are ignored visually
///    during flight, but stay tracked underneath so the correct zone's
///    animation resumes the moment Fly ends.
///  - Among hand zones, whichever was touched FIRST keeps the shown
///    animation until IT releases - touching a second zone afterwards does
///    not steal the animation away from the first.
/// </summary>
public class SparrowAnimatorOwner : MonoBehaviour
{
    public Animator animator;

    [Tooltip("Optional - shared sound for hand-touch reactions (head/belly/etc). Not used for Fly, since SparrowFeet has its own AudioSource field it doesn't currently use.")]
    public AudioSource audioSource;

    [Header("Animation")]
    [Tooltip("State shown when no hand zone is active and the bird isn't flying. The idle states are Idle_A/B/C - there is no plain \"Idle\".")]
    public string idleState = "Idle_A";

    // One entry per hand-touch zone currently holding the bird active.
    // Order matters: index 0 is whichever zone was touched FIRST, and its
    // state is what gets shown - not whichever zone is newest.
    private struct HandZone
    {
        public object zone;
        public string state;
    }
    private readonly List<HandZone> activeHandZones = new List<HandZone>();

    private bool isFlying;
    private string flyState;

    // Cached so Recompute() only issues a CrossFade / Play / Stop when the
    // effective result actually changes, instead of on every notify call.
    private string currentState;
    private bool audioIsPlaying;

    /// <summary>Called by a hand-touch zone (e.g. SparrowController, SparrowBelly) on first contact.</summary>
    public void NotifyTouchStart(object zone, string stateName)
    {
        if (activeHandZones.Exists(z => Equals(z.zone, zone)))
            return; // already registered - ignore a duplicate enter

        activeHandZones.Add(new HandZone { zone = zone, state = stateName });
        Recompute();
    }

    /// <summary>Called by a hand-touch zone once its own last collider has released.</summary>
    public void NotifyTouchEnd(object zone)
    {
        activeHandZones.RemoveAll(z => Equals(z.zone, zone));
        Recompute();
    }

    /// <summary>Called by SparrowFeet when it leaves the table (takes off).</summary>
    public void NotifyFlyStart(string flyStateName)
    {
        isFlying = true;
        flyState = flyStateName;
        Recompute();
    }

    /// <summary>Called by SparrowFeet when it lands back on the table.</summary>
    public void NotifyFlyEnd()
    {
        isFlying = false;
        Recompute();
    }

    private void Recompute()
    {
        string targetState = isFlying
            ? flyState
            : (activeHandZones.Count > 0 ? activeHandZones[0].state : idleState);

        if (targetState != currentState)
        {
            currentState = targetState;
            if (animator != null) animator.CrossFade(targetState, 0.12f);
        }

        // Hand-reaction audio only plays while a hand zone is showing -
        // silent during Fly and back to silent once every zone releases.
        bool audioShouldPlay = !isFlying && activeHandZones.Count > 0;
        if (audioShouldPlay != audioIsPlaying)
        {
            audioIsPlaying = audioShouldPlay;
            if (audioSource != null)
            {
                if (audioShouldPlay) audioSource.Play();
                else audioSource.Stop();
            }
        }
    }

    private void OnDisable()
    {
        // Safety net: don't leave stale state if the owner itself is
        // disabled mid-touch/mid-flight.
        activeHandZones.Clear();
        isFlying = false;
        currentState = null;
        audioIsPlaying = false;
    }
}
