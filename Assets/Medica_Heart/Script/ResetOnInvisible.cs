using UnityEngine;

public class ResetOnInvisible : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer; // assign your child renderer here
    private Vector3 spawnPoint;

    private void Start()
    {
        spawnPoint = transform.position;
    }

    private void Update()
    {
        if (targetRenderer != null && !targetRenderer.isVisible)
        {
            transform.position = spawnPoint;
        }
    }
}

