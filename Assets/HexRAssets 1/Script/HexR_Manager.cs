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
            leftHand.onBluetoothConnected += HaptGlove_OnConnected;
            leftHand.onBluetoothConnectionFailed += HaptGlove_OnConnectedFailed;
            leftHand.onBluetoothDisconnected += HaptGlove_OnDisconnected;

            rightHand.onBluetoothConnected += HaptGlove_OnConnected;
            rightHand.onBluetoothConnectionFailed += HaptGlove_OnConnectedFailed;
            rightHand.onBluetoothDisconnected += HaptGlove_OnDisconnected;
        }

        void Update()
        {
            ConnectionControl();
            if(Input.GetKeyDown(KeyCode.P))
            {
                ConnectRightBT();
            }
        }
        private void ConnectionControl()
        {
            if (!ConnectedLeft && leftHand.bleConnected)
            {
                ConnectedLeft = true;
            }
            if (!ConnectedRight && rightHand.bleConnected)
            {
                ConnectedLeft = true;
            }
            if (ConnectedLeft && !leftHand.bleConnected)
            {
                ConnectedLeft = false;
                LeftBtText.color = Color.black;
            }
            if (ConnectedRight && !rightHand.bleConnected)
            {
                ConnectedLeft = false;
                RightBtText.color = Color.black;
            }
        }
        public void ConnectRightBT()
        {
            controlledHandsList.Remove("Left");
            controlledHandsList.Add("Right");
            RightBtText.text = "Searching for device...";
            rightHand.GetComponent<HaptGloveHandler>().BTConnection();
            StartCoroutine(CheckRightConnected());
        }
        public void ConnectLeftBT()
        {
            controlledHandsList.Add("Left");
            controlledHandsList.Remove("Right");
            LeftBtText.text = "Searching for device...";
            leftHand.GetComponent<HaptGloveHandler>().BTConnection();
            StartCoroutine(CheckLeftConnected());
        }
        IEnumerator CheckRightConnected()
        {
            yield return new WaitForSeconds(0.1f); // Wait 2 seconds

            if (rightHand.GetComponent<HaptGloveHandler>().bleConnected)
            {
                RightBtText.text = "HexR Right Connected";
            }
            else
            {
                StartCoroutine(CheckRightConnected()); // Retry
            }

        }
        IEnumerator CheckLeftConnected()
        {
            yield return new WaitForSeconds(0.1f); // Wait 2 seconds

            if (leftHand.GetComponent<HaptGloveHandler>().bleConnected)
            {
                LeftBtText.text = "HexR Left Connected";
            }
            else
            {
                StartCoroutine(CheckLeftConnected()); // Retry
            }

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
