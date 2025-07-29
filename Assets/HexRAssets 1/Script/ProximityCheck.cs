using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaptGlove;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;

namespace HexR
{
    public class ProximityCheck : MonoBehaviour
    {
        public PressureTrackerMain rightpressureTrackerMain, leftpressureTrackerMain;
        private bool restart = false;
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
            if (other.name.Contains("L_Palm") || other.name.Contains("LeftGhostPalm"))
            {
                restart = true;
                leftpressureTrackerMain.IsPhysicsCollisionNear(true);
                removeCollisiontrue(leftpressureTrackerMain);
            }
            else if(other.name.Contains("R_Palm") || other.name.Contains("RightGhostPalm"))
            {
                restart = true;
                rightpressureTrackerMain.IsPhysicsCollisionNear(true);
                removeCollisiontrue(rightpressureTrackerMain);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.name.Contains("L_Palm") || other.name.Contains("LeftGhostPalm"))
            {
                restart = false;
                leftpressureTrackerMain.IsPhysicsCollisionNear(true);
            }
            else if (other.name.Contains("R_Palm") || other.name.Contains("RightGhostPalm"))
            {
                restart = false;
                rightpressureTrackerMain.IsPhysicsCollisionNear(true);
            }
        }
        IEnumerator removeCollisiontrue(PressureTrackerMain pressureTrackerMain)
        {
            // Wait for the specified delay time
            yield return new WaitForSeconds(0.5f);
            if(restart == true)
            {
                pressureTrackerMain.IsPhysicsCollisionNear(false);
            }
            else
            {
                restart = true;
                StartCoroutine(removeCollisiontrue(pressureTrackerMain));
            }


        }
        private void OnTriggerExit(Collider other)
        {
            if (other.name.Contains("L_Palm") || other.name.Contains("LeftGhostPalm"))
            {
                leftpressureTrackerMain.IsPhysicsCollisionNear(false);
            }
            else if (other.name.Contains("R_Palm") || other.name.Contains("RightGhostPalm"))
            {
                rightpressureTrackerMain.IsPhysicsCollisionNear(false);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ProximityCheck))]
    public class ProximityCheckEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            // Get reference to the target script
            ProximityCheck controller = (ProximityCheck)target;

            // Add fields to assign RightHandPhysics and LeftHandPhysics
            controller.rightpressureTrackerMain = (PressureTrackerMain)EditorGUILayout.ObjectField(
                "Right Hand Physics",
                controller.rightpressureTrackerMain,
                typeof(PressureTrackerMain),
                true // Allow scene objects
            );

            controller.leftpressureTrackerMain = (PressureTrackerMain)EditorGUILayout.ObjectField(
                "Left Hand Physics",
                controller.leftpressureTrackerMain,
                typeof(PressureTrackerMain),
                true // Allow scene objects
            );



            GUILayout.Space(15); // Add vertical spacing

            if (GUILayout.Button("Auto Set Up"))
            {
                try
                {
                    controller.rightpressureTrackerMain = GameObject.Find("Right Hand Physics").GetComponent<PressureTrackerMain>(); // Replace with the name of your target object
                    controller.leftpressureTrackerMain = GameObject.Find("Left Hand Physics").GetComponent<PressureTrackerMain>(); // Replace with the name of your target object

                }
                catch
                {
                    Debug.Log("Pressure Tracker Main Not Found Remember to assign them.");
                }


                // Check if the controller's GameObject has a collider
                Collider existingCollider = controller.gameObject.GetComponent<Collider>();
                if (existingCollider != null)
                {
                    Debug.Log("Collider already exists on the controller.");
                    if (existingCollider is BoxCollider boxCollider)
                    {
                        boxCollider.isTrigger = true; // Set existing BoxCollider as a trigger
                        Debug.Log("Existing BoxCollider set to trigger.");
                        Debug.Log("Remember to adjust the size of your collider");
                    }
                }
                else
                {
                    // Add a BoxCollider and set it as a trigger
                    Debug.Log("No collider found. Adding a BoxCollider.");
                    BoxCollider newBoxCollider = controller.gameObject.AddComponent<BoxCollider>();
                    newBoxCollider.isTrigger = true; // Set the newly added BoxCollider as a trigger
                    Debug.Log("New BoxCollider added and set to trigger.");
                    Debug.Log("Remember to adjust the size of your collider");
                }

                EditorUtility.SetDirty(controller); // Mark as dirty to save changes
            }
            // Save changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }

#endif
}
