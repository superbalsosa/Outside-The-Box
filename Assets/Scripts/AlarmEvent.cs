using UnityEngine;

public class AlarmEvent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotatingChild; // Target child to rotate

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 180f; // Degrees per second
    [SerializeField] public bool isActive = false; // External toggle

    private void Update()
    {
        // Skip if inactive or missing reference
        if (!isActive || rotatingChild == null) return;

        // Rotate around Z axis
        rotatingChild.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
    }
}