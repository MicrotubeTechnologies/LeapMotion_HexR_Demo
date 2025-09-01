using UnityEngine;

public class ResetOnDistance : MonoBehaviour
{
    private float maxDistance = 0.35f; // max allowed distance
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;
    private Rigidbody rb;

    private void Start()
    {
        // Record spawn point and rotation at start
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, spawnPoint);
        if (distance > maxDistance)
        {
            Respawn();
        }
    }


    public void Respawn()
    {
        transform.position = spawnPoint;
        transform.rotation = spawnRotation;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;        // stop movement
            rb.angularVelocity = Vector3.zero; // stop spinning
        }
    }
}


