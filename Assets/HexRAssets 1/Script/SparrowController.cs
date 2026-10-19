using HexR;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The bird's head zone: petting it with an open palm makes it bounce.
/// Works with either hand.
/// </summary>
public class SparrowController : MonoBehaviour
{
    public enum AcceptedHand { Either, RightOnly, LeftOnly }

    /// <summary>Which hand touched the sparrow, resolved from collider/bone names.</summary>
    private enum Side { Unknown, Left, Right }

    // Bone/root naming conventions used to resolve a Side from a collider's
    // transform hierarchy. See SideOf() for why both are needed.
    private const string RightPrefix = "R_";
    private const string RightName = "Right";
    private const string LeftPrefix = "L_";
    private const string LeftName = "Left";

    public Animator animator;
    // Field names kept as-is so the existing scene references survive.
    // NOTE: "RighPTM" is a typo, kept intentionally - renaming it would break
    // the Inspector binding on the existing scene/prefab.
    public PressureTrackerMain RighPTM, LeftPTM;
    public AudioSource AudioSource;

    [Header("Shared bird (optional)")]
    [Tooltip("Assign when several zones (head, belly, feet, etc.) drive the SAME physical bird's Animator. When set, this zone reports touches to the owner instead of calling Bounce/Idle directly - the owner arbitrates which zone's animation is shown when several are active at once. Leave empty for a standalone sparrow with its own Animator.")]
    public SparrowAnimatorOwner owner;

    [Header("Which hand")]
    [Tooltip("Which hand may pet this zone. Either means both work.")]
    public AcceptedHand acceptedHand = AcceptedHand.Either;

    [Header("Animation")]
    [Tooltip("State played while the palm is on the bird.")]
    public string bounceState = "Bounce";
    [Tooltip("State returned to on release. The idle states are Idle_A/B/C - there is no plain \"Idle\".")]
    public string idleState = "Idle_A";

    [Header("Debug")]
    [Tooltip("Logs every collider that enters, with the hand it resolved to. Turn off once matching works.")]
    public bool logTouches = false;

    // The palm is several colliders; count them so the reaction fires on the
    // first contact and only ends on the last release.
    private readonly HashSet<Collider> touching = new HashSet<Collider>();
    // Whichever glove is doing the petting, so the correct one is cleared.
    private PressureTrackerMain activeTracker;

    private void OnTriggerEnter(Collider other)
    {
        Side side = SideOf(other);
        if (logTouches)
            Debug.Log("[Sparrow] " + name + " touched by '" + other.name + "' -> " + SideName(side), this);

        if (!IsPalm(other) || !HandAllowed(side))
            return;

        bool isFirstTouch = touching.Count == 0;
        touching.Add(other);
        if (!isFirstTouch)
            return;

        activeTracker = TrackerFor(side);
        if (owner != null) owner.NotifyTouchStart(this, bounceState);
        else Bounce();
        if (AudioSource != null) AudioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        bool wasTracked = touching.Remove(other);
        bool isLastRelease = touching.Count == 0;
        if (!wasTracked || !isLastRelease)
            return;

        Release();
    }

    private void Update()
    {
        if (touching.Count == 0)
            return;

        // Tracking loss disables the bone colliders without an exit event,
        // so colliders that went dead/disabled/inactive have to be swept out
        // here rather than relying on OnTriggerExit to catch them.
        touching.RemoveWhere(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy);
        if (touching.Count == 0)
            Release();
    }

    private void OnDisable()
    {
        if (touching.Count > 0)
        {
            touching.Clear();
            Release();
        }
    }

    private void Release()
    {
        if (owner != null) owner.NotifyTouchEnd(this);
        else Idle();
        if (activeTracker != null) activeTracker.RemoveAllVibrations();
        activeTracker = null;
        if (AudioSource != null) AudioSource.Stop();
    }

    private static bool IsPalm(Collider other)
    {
        return other != null && other.name.Contains("Palm");
    }

    private bool HandAllowed(Side side)
    {
        if (acceptedHand == AcceptedHand.RightOnly) return side == Side.Right;
        if (acceptedHand == AcceptedHand.LeftOnly) return side == Side.Left;
        return side != Side.Unknown;
    }

    private PressureTrackerMain TrackerFor(Side side)
    {
        return side == Side.Left ? LeftPTM : RighPTM;
    }

    private static string SideName(Side side)
    {
        return side == Side.Right ? "right hand" : side == Side.Left ? "left hand" : "unknown hand";
    }

    /// <summary>
    /// Resolves which hand touched, or Unknown if it can't be determined.
    ///
    /// Two hand rigs are in play and they name bones differently:
    ///   HexR ghost hands   -> "R_Wrist_Palm"  (side is on the bone itself)
    ///   Ultraleap physical -> "Right Palm" / "Index Distal" (side only on the root)
    /// so the hierarchy is walked upward until a side is found.
    /// </summary>
    private static Side SideOf(Collider other)
    {
        if (other == null) return Side.Unknown;

        for (Transform t = other.transform; t != null; t = t.parent)
        {
            string n = t.name;
            if (n.Contains(RightPrefix) || n.Contains(RightName)) return Side.Right;
            if (n.Contains(LeftPrefix) || n.Contains(LeftName)) return Side.Left;
        }
        return Side.Unknown;
    }

    // AC_Sparrow has no parameters and no transitions - it is a bare library of
    // 18 states, so SetTrigger does nothing and states must be played directly.
    public void Bounce()
    {
        if (animator != null) animator.CrossFade(bounceState, 0.12f);
    }

    public void Idle()
    {
        if (animator != null) animator.CrossFade(idleState, 0.12f);
    }
}
