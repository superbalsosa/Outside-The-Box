using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Noise Motion")]
    public float noisePositionAmount = 0.2f;
    public float noiseRotationAmount = 0.25f;
    public float noiseSpeed = 0.5f;

    [Header("Breathing")]
    public float breathingAmplitude = 0.1f;
    public float breathingSpeed = 0.5f;

    [Header("FOV Dynamics")]
    public float baseFOV = 35f;
    public float fovAmplitude = 2f;
    public float fovSpeed = 0.5f;

    [Header("Warp")]
    public bool warpActive = false;
    public float warpFOV = 40f;
    public float warpTransitionSpeed = 2f;

    Transform tr;
    Camera cam;

    Vector3 startPos;
    Quaternion startRot;

    void Start()
    {
        tr = transform;
        startPos = tr.position;
        startRot = tr.rotation;

        cam = GetComponent<Camera>();
        if (cam) cam.fieldOfView = baseFOV;
    }

    void Update()
    {
        float t = Time.time;

        float nt = t * noiseSpeed;
        float bt = t * breathingSpeed;

        // --- POSITION ---

        float nx = Mathf.PerlinNoise(nt, 0f) - 0.5f;
        float ny = Mathf.PerlinNoise(0f, nt) - 0.5f;

        Vector3 noisePos = new Vector3(nx, ny, nx) * noisePositionAmount;

        float breathe = Mathf.Sin(bt) * breathingAmplitude;

        tr.position = startPos + noisePos + Vector3.up * breathe;

        // --- ROTATION ---

        float rx = Mathf.PerlinNoise(nt, 1f) - 0.5f;
        float ry = Mathf.PerlinNoise(1f, nt) - 0.5f;

        Vector3 noiseRot = new Vector3(rx, ry, rx) * noiseRotationAmount;

        tr.rotation = startRot * Quaternion.Euler(noiseRot);

        // --- FOV ---

        if (!cam) return;

        float targetFOV = baseFOV;

        if (warpActive)
        {
            targetFOV = warpFOV;
        }
        else
        {
            targetFOV += Mathf.Sin(t * fovSpeed) * fovAmplitude;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * warpTransitionSpeed);
    }

    public void SetWarp(bool state)
    {
        warpActive = state;
    }
}