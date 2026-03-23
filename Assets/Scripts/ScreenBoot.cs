using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class ScreenBoot : MonoBehaviour
{
    [Header("Shader")]
    public Material mat;
    public float powerSpeed = 1.2f;

    [Header("Volumes")]
    public PostProcessVolume dofVolume;
    public PostProcessVolume lensVolume;

    [Header("DOF")]
    public float dofOnDistance = 0.1f;
    public float dofOffDistance = 30f;
    public float dofSpeed = 4f;

    [Header("Lens")]
    public float lensMin = -100f;
    public float lensMax = 100f;
    public float lensFinal = 35f;
    public float lensSpeed = 1f;

    [Header("Glow")]
    public float glowSpeed = 4f;
    public float glowMax = 1.5f;

    [Header("State")]
    public bool screenOn;

    float power;
    float currentDOF;

    float glowBoost;
    float glowTimer;
    bool isGlowing;

    float shutdown;

    DepthOfField dof;
    LensDistortion lens;

    bool lastState;
    Coroutine lensRoutine;

    void Start()
    {
        SetupPostProcessing();
        SetupMaterialInstance();
        ApplyOffState();
    }

    void Update()
    {
        HandleStateChange();
        UpdatePower();
        UpdateDOF();
        UpdateGlow();
        UpdateShader();
    }

    void OnDisable()
    {
        ApplyOffState();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
            ApplyOffState();
    }
#endif

    // Initialization

    void SetupPostProcessing()
    {
        if (dofVolume != null)
            dofVolume.profile.TryGetSettings(out dof);

        if (lensVolume != null)
            lensVolume.profile.TryGetSettings(out lens);

        currentDOF = dofOffDistance;

        if (dof != null)
            dof.focusDistance.value = currentDOF;

        if (lens != null)
            lens.intensity.value = 0f;
    }

    void SetupMaterialInstance()
    {
        if (mat != null) return;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            mat = renderer.material;
    }

    // State handling

    void HandleStateChange()
    {
        if (screenOn == lastState) return;

        RestartLensRoutine();

        if (screenOn)
            TriggerGlow();

        lastState = screenOn;
    }

    void RestartLensRoutine()
    {
        if (lensRoutine != null)
            StopCoroutine(lensRoutine);

        lensRoutine = StartCoroutine(screenOn ? LensOn() : LensOff());
    }

    void TriggerGlow()
    {
        isGlowing = true;
        glowTimer = 0f;
    }

    // Updates

    void UpdatePower()
    {
        float target = screenOn ? 1f : 0f;
        power = Mathf.MoveTowards(power, target, Time.deltaTime * powerSpeed);
    }

    void UpdateDOF()
    {
        if (dof == null) return;

        float target = screenOn ? dofOnDistance : dofOffDistance;
        currentDOF = Mathf.Lerp(currentDOF, target, Time.deltaTime * dofSpeed);

        dof.focusDistance.value = currentDOF;
    }

    void UpdateGlow()
    {
        if (!isGlowing)
        {
            glowBoost = 0f;
            return;
        }

        glowTimer += Time.deltaTime * glowSpeed;
        glowBoost = Mathf.Sin(glowTimer * Mathf.PI) * glowMax;

        if (glowTimer >= 1f)
        {
            isGlowing = false;
            glowBoost = 0f;
        }
    }

    void UpdateShader()
    {
        if (mat == null) return;

        shutdown = screenOn ? 0f : 1f;

        mat.SetFloat("_Power", power);
        mat.SetFloat("_GlowBoost", glowBoost);
        mat.SetFloat("_Shutdown", shutdown);
    }

    // Force off state

    void ApplyOffState()
    {
        power = 0f;
        glowBoost = 0f;
        shutdown = 1f;
        currentDOF = dofOffDistance;
        isGlowing = false;

        if (mat != null)
        {
            mat.SetFloat("_Power", 0f);
            mat.SetFloat("_GlowBoost", 0f);
            mat.SetFloat("_Shutdown", 1f);
        }

        if (dof != null)
            dof.focusDistance.value = dofOffDistance;

        if (lens != null)
            lens.intensity.value = 0f;
    }

    // Lens sequences

    IEnumerator LensOn()
    {
        if (lens == null) yield break;

        yield return LerpLens(lens.intensity.value, lensMin, 0.08f);
        yield return LerpLens(lensMin, lensMax, 0.12f);
        yield return LerpLens(lensMax, lensFinal, 0.2f);
    }

    IEnumerator LensOff()
    {
        if (lens == null) yield break;

        yield return LerpLens(lens.intensity.value, lensMax, 0.15f);
        yield return LerpLens(lensMax, lensMin, 0.12f);
        yield return LerpLens(lensMin, 0f, 0.1f);
    }

    IEnumerator LerpLens(float from, float to, float duration)
    {
        float t = 0f;
        duration /= lensSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            lens.intensity.value = Mathf.Lerp(from, to, t);
            yield return null;
        }

        lens.intensity.value = to;
    }
}