using System.Collections;
using System.Collections.Generic;
using HaptGlove;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace HexR
{
    public class HexR_Manager : MonoBehaviour
    {
        public HaptGloveHandler leftHand, rightHand;
        public TextMeshProUGUI RightBtText, LeftBtText;

        private List<string> controlledHandsList = new List<string>();
        private bool ConnectedLeft =false, ConnectedRight=false;
        void Start()
        {
            if (leftHand != null)
            {
                leftHand.onBluetoothConnected += HaptGlove_OnConnected;
                leftHand.onBluetoothConnectionFailed += HaptGlove_OnConnectedFailed;
                leftHand.onBluetoothDisconnected += HaptGlove_OnDisconnected;
            }
            else Debug.LogError("[HexR] Left glove handler is not assigned on HexR_Manager.", this);

            if (rightHand != null)
            {
                rightHand.onBluetoothConnected += HaptGlove_OnConnected;
                rightHand.onBluetoothConnectionFailed += HaptGlove_OnConnectedFailed;
                rightHand.onBluetoothDisconnected += HaptGlove_OnDisconnected;
            }
            else Debug.LogError("[HexR] Right glove handler is not assigned on HexR_Manager.", this);
        }

        void Update()
        {
            ConnectionControl();
            if(Input.GetKeyDown(KeyCode.P))
            {
                try
                {
                    ConnectRightBT();
                }
                catch
                {

                }
            }

        }

        private void ConnectionControl()
        {
            // The right-hand branches used to assign ConnectedLeft - a copy/paste
            // slip that meant the right glove's connected state was never tracked,
            // so its label never reset on disconnect.
            if (leftHand != null)
            {
                if (!ConnectedLeft && leftHand.bleConnected)
                {
                    ConnectedLeft = true;
                }
                else if (ConnectedLeft && !leftHand.bleConnected)
                {
                    ConnectedLeft = false;
                    if (LeftBtText != null) LeftBtText.color = Color.black;
                }
            }

            if (rightHand != null)
            {
                if (!ConnectedRight && rightHand.bleConnected)
                {
                    ConnectedRight = true;
                }
                else if (ConnectedRight && !rightHand.bleConnected)
                {
                    ConnectedRight = false;
                    if (RightBtText != null) RightBtText.color = Color.black;
                }
            }
        }
        public void ConnectRightBT()
        {
            controlledHandsList.Remove("Left");
            controlledHandsList.Add("Right");
            Connect(rightHand, RightBtText, "Right");
        }
        public void ConnectLeftBT()
        {
            controlledHandsList.Add("Left");
            controlledHandsList.Remove("Right");
            Connect(leftHand, LeftBtText, "Left");
        }

        /// <summary>
        /// Shared connect path for both gloves.
        ///
        /// The original code activated the glove object and called BTConnection()
        /// on the same line. Activating a GameObject runs Awake and OnEnable
        /// immediately but NOT Start, so the handler could still be half
        /// initialised when the connection was kicked off - which surfaces as a
        /// NullReferenceException thrown from inside HaptGloveHandler itself.
        /// Waiting a frame lets Start run first.
        /// </summary>
        private void Connect(HaptGloveHandler hand, TextMeshProUGUI label, string handName)
        {
            if (hand == null)
            {
                Debug.LogError("[HexR] " + handName + " glove handler is not assigned on HexR_Manager.", this);
                return;
            }

            bool wasInactive = !hand.gameObject.activeInHierarchy;
            hand.gameObject.SetActive(true);

            if (label != null)
                label.text = "Searching for device...";

            StartCoroutine(ConnectRoutine(hand, label, handName, wasInactive));
        }

        private IEnumerator ConnectRoutine(HaptGloveHandler hand, TextMeshProUGUI label, string handName, bool wasInactive)
        {
            if (wasInactive)
                yield return null;   // let the handler's Start() run before talking to it

            try
            {
                hand.BTConnection();
            }
            catch (System.Exception e)
            {
                // BTConnection throws when there is no paired glove or no
                // Bluetooth adapter available. Reporting it beats an unexplained
                // stack trace every time the button is pressed.
                Debug.LogWarning("[HexR] Could not start a " + handName + " glove connection: " + e.Message, this);
                if (label != null)
                    label.text = "No glove found";
                yield break;
            }

            yield return CheckConnected(hand, label, handName);
        }

        /// <summary>
        /// Replaces the old CheckLeftConnected/CheckRightConnected pair, which
        /// restarted themselves forever - every failed attempt left another
        /// coroutine chain polling for the rest of the session.
        /// </summary>
        private IEnumerator CheckConnected(HaptGloveHandler hand, TextMeshProUGUI label, string handName)
        {
            const float timeout = 20f;
            float waited = 0f;

            while (waited < timeout)
            {
                if (hand.bleConnected)
                {
                    if (label != null)
                        label.text = "HexR " + handName + " Connected";
                    yield break;
                }

                yield return new WaitForSeconds(0.1f);
                waited += 0.1f;
            }

            if (label != null)
                label.text = "Connection timed out";
        }
        private void HaptGlove_OnConnected(HaptGloveHandler.HandType hand)
        {
            if (hand == HaptGloveHandler.HandType.Left)
            {
                if (LeftBtText != null)
                {
                    LeftBtText.text = "Left Glove Connected";
                }
            }
            else if (hand == HaptGloveHandler.HandType.Right)
            {
                if (RightBtText != null)
                {
                    RightBtText.text = "Right Glove Connected";
                }
            }

        }

        private void HaptGlove_OnConnectedFailed(HaptGloveHandler.HandType hand)
        {
            if (hand == HaptGloveHandler.HandType.Left)
            {
                if (LeftBtText != null)
                {
                    LeftBtText.text = "Connection failed";
                }
            }
            else if (hand == HaptGloveHandler.HandType.Right)
            {
                if (RightBtText != null)
                {
                    RightBtText.text = "Connection failed";
                }
            }
        }

        private void HaptGlove_OnDisconnected(HaptGloveHandler.HandType hand)
        {
            if (hand == HaptGloveHandler.HandType.Left)
            {
                if (LeftBtText != null)
                {
                    LeftBtText.text = "HexR Left Disconnected";
                }
            }
            else if (hand == HaptGloveHandler.HandType.Right)
            {
                if (RightBtText != null)
                {
                    RightBtText.text = "HexR Right Disconnected";
                }
            }
        }
    }


}
