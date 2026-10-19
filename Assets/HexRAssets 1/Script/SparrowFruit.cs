using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lives on the fruit. Two independent conditions drive the bird's reaction:
///   - Grabbed (call OnGrabbed()/OnReleased() from your own grab system)
///     -> bird Spins. The shared owner already makes Fly override this
///        while airborne, so no extra "not flying" check is needed here.
///   - Touching the bird (trigger collider match, same pattern as the other
///     zones) -> bird Eats. Takes priority over Spin if both are true at
///     once (e.g. holding the fruit right up to the bird's mouth).
/// </summary>
public class SparrowFruit : MonoBehaviour
{
    public Animator animator; // fallback if no owner assigned (standalone bird)

    [Header("Shared bird (optional)")]
    [Tooltip("Assign to the SAME SparrowAnimatorOwner used by the bird's other zones (head, belly, feet, etc.) so Spin/Eat correctly yield to Fly and to whichever hand-touch zone was first active. Leave empty for a standalone sparrow with its own Animator.")]
    public SparrowAnimatorOwner owner;

    [Header("Bird contact (for Eat)")]
    [Tooltip("Collider name substring on the bird that counts as \"touching\" for Eat - adjust once you've decided which collider represents the bird's body/mouth.")]
    public string birdColliderNameContains = "Bird";

    [Header("Animation")]
    public string spinState = "Spin";
    public string eatState = "Eat";
    [Tooltip("State returned to once neither grabbed nor touching the bird. The idle states are Idle_A/B/C - there is no plain \"Idle\".")]
    public string idleState = "Idle_A";

    [Header("Debug")]
    [Tooltip("Logs every collider that enters/exits contact with the bird, and every grab/release call.")]
    public bool logTouches = false;

    // Several bird colliders could plausibly overlap the fruit at once, so
    // count them the same way the other zones count palm/finger colliders.
    private readonly HashSet<Collider> touchingBird = new HashSet<Collider>();
    private bool isGrabbed;

    // Whichever single state (spinState/eatState/null) is currently
    // registered with the owner (or being shown directly, if standalone).
    private string currentRequestedState;

    /// <summary>Call this from your grab system when the fruit is picked up.</summary>
    public void OnGrabbed()
    {
        isGrabbed = true;
        if (logTouches) Debug.Log("[SparrowFruit] Grabbed", this);
        UpdateDesiredState();
    }

    /// <summary>Call this from your grab system when the fruit is let go.</summary>
    public void OnReleased()
    {
        isGrabbed = false;
        if (logTouches) Debug.Log("[SparrowFruit] Released", this);
        UpdateDesiredState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!MatchesBirdCollider(other))
            return;

        bool wasEmpty = touchingBird.Count == 0;
        touchingBird.Add(other);
        if (logTouches) Debug.Log("[SparrowFruit] Touching bird collider '" + other.name + "'", this);
        if (wasEmpty)
            UpdateDesiredState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!touchingBird.Remove(other))
            return;

        if (logTouches) Debug.Log("[SparrowFruit] Left bird collider '" + other.name + "'", this);
        if (touchingBird.Count == 0)
            UpdateDesiredState();
    }

    private void OnDisable()
    {
        bool wasActive = isGrabbed || touchingBird.Count > 0;
        isGrabbed = false;
        touchingBird.Clear();
        if (wasActive)
            UpdateDesiredState();
    }

    private bool MatchesBirdCollider(Collider other)
    {
        return other != null && !string.IsNullOrEmpty(birdColliderNameContains)
            && other.name.Contains(birdColliderNameContains);
    }

    // Eat (touching the bird) takes priority over Spin (just grabbed) -
    // bringing the fruit to the bird is the more specific action.
    private void UpdateDesiredState()
    {
        string desired = touchingBird.Count > 0 ? eatState
            : isGrabbed ? spinState
            : null;

        if (desired == currentRequestedState)
            return;

        if (currentRequestedState != null && owner != null)
            owner.NotifyTouchEnd(this);

        currentRequestedState = desired;

        if (desired != null)
        {
            if (owner != null) owner.NotifyTouchStart(this, desired);
            else if (animator != null) animator.CrossFade(desired, 0.12f);
        }
        else if (owner == null && animator != null)
        {
            animator.CrossFade(idleState, 0.12f);
        }
    }
}
