using System.Collections.Generic;
using Microtube.WebcamHand;
using UnityEngine;

namespace HexR.WebcamTracking
{
    /// <summary>
    /// Drives the Ghost Capsule Hands rig's bones from webcam tracking data, replacing
    /// Ultraleap's HandBinder. Position is only set on the wrist; every other bone is
    /// rotation-only, preserving the rig's own authored bone lengths (setting every bone's
    /// world position directly would stretch the mesh, since the tracked hand's proportions
    /// won't match the rig's). Each bone's rest-pose offset is captured once at Start() from
    /// its own next child Transform, so this doesn't need to know the rig's local bone-axis
    /// convention up front.
    /// </summary>
    public class WebcamBoneBinder : MonoBehaviour
    {
        [SerializeField] private WebcamHandProvider provider;
        [SerializeField] private Chirality chirality = Chirality.Right;
        [SerializeField] private Transform wrist;
        [SerializeField] private FingerRig thumb;
        [SerializeField] private FingerRig index;
        [SerializeField] private FingerRig middle;
        [SerializeField] private FingerRig ring;
        [SerializeField] private FingerRig pinky;

        private static readonly HandJoint[] ThumbChain =
            { HandJoint.ThumbCMC, HandJoint.ThumbMCP, HandJoint.ThumbIP, HandJoint.ThumbTip };
        private static readonly HandJoint[] IndexChain =
            { HandJoint.Wrist, HandJoint.IndexMCP, HandJoint.IndexPIP, HandJoint.IndexDIP, HandJoint.IndexTip };
        private static readonly HandJoint[] MiddleChain =
            { HandJoint.Wrist, HandJoint.MiddleMCP, HandJoint.MiddlePIP, HandJoint.MiddleDIP, HandJoint.MiddleTip };
        private static readonly HandJoint[] RingChain =
            { HandJoint.Wrist, HandJoint.RingMCP, HandJoint.RingPIP, HandJoint.RingDIP, HandJoint.RingTip };
        private static readonly HandJoint[] PinkyChain =
            { HandJoint.Wrist, HandJoint.PinkyMCP, HandJoint.PinkyPIP, HandJoint.PinkyDIP, HandJoint.PinkyTip };

        private struct BoneRig
        {
            public Transform bone;
            public HandJoint fromJoint;
            public HandJoint toJoint;
            public Quaternion restOffset;
        }

        private readonly List<BoneRig> _rigs = new List<BoneRig>();
        private Quaternion _wristRestOffset;
        private bool _wristRigValid;

        private void Start()
        {
            AddFinger(thumb.Bones, ThumbChain);
            AddFinger(index.Bones, IndexChain);
            AddFinger(middle.Bones, MiddleChain);
            AddFinger(ring.Bones, RingChain);
            AddFinger(pinky.Bones, PinkyChain);

            // Wrist's orientation proxy: direction toward the middle finger's metacarpal,
            // the same rest-offset pattern used for every finger bone.
            if (wrist != null && middle.meta != null)
            {
                Vector3 restDir = (middle.meta.position - wrist.position).normalized;
                if (restDir.sqrMagnitude > 1e-6f)
                {
                    _wristRestOffset = Quaternion.Inverse(Quaternion.LookRotation(restDir)) * wrist.rotation;
                    _wristRigValid = true;
                }
            }
        }

        private void OnEnable()
        {
            if (provider != null) provider.OnFrame += HandleFrame;
        }

        private void OnDisable()
        {
            if (provider != null) provider.OnFrame -= HandleFrame;
        }

        private void HandleFrame(HandFrame frame)
        {
            var hand = chirality == Chirality.Left ? frame.Left : frame.Right;
            if (hand == null) return;

            if (wrist != null)
            {
                wrist.position = hand.GetJoint(HandJoint.Wrist).Position;
                if (_wristRigValid)
                {
                    Vector3 dir = (hand.GetJoint(HandJoint.MiddleMCP).Position - hand.GetJoint(HandJoint.Wrist).Position).normalized;
                    if (dir.sqrMagnitude > 1e-6f)
                        wrist.rotation = Quaternion.LookRotation(dir) * _wristRestOffset;
                }
            }

            foreach (var rig in _rigs)
            {
                Vector3 from = hand.GetJoint(rig.fromJoint).Position;
                Vector3 to = hand.GetJoint(rig.toJoint).Position;
                Vector3 dir = (to - from).normalized;
                if (dir.sqrMagnitude < 1e-6f) continue;
                rig.bone.rotation = Quaternion.LookRotation(dir) * rig.restOffset;
            }
        }

        private void AddFinger(Transform[] bones, HandJoint[] chain)
        {
            // bones[i] spans chain[i] -> chain[i+1]; bones.Length must be chain.Length - 1.
            for (int i = 0; i < bones.Length; i++)
            {
                Transform bone = bones[i];
                if (bone == null || bone.childCount == 0) continue;

                Transform child = bone.GetChild(0);
                Vector3 restDir = (child.position - bone.position).normalized;
                if (restDir.sqrMagnitude < 1e-6f) continue;

                Quaternion offset = Quaternion.Inverse(Quaternion.LookRotation(restDir)) * bone.rotation;
                _rigs.Add(new BoneRig
                {
                    bone = bone,
                    fromJoint = chain[i],
                    toJoint = chain[i + 1],
                    restOffset = offset,
                });
            }
        }
    }

    [System.Serializable]
    public class FingerRig
    {
        public Transform meta;
        public Transform a;
        public Transform b;
        [Tooltip("Leave empty for the thumb - it has one fewer segment than the other fingers.")]
        public Transform c;

        public Transform[] Bones => c != null
            ? new[] { meta, a, b, c }
            : new[] { meta, a, b };
    }
}
