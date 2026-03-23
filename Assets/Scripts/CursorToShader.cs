using UnityEngine;

public class CursorToShader : MonoBehaviour
{
    public Material mat;

    [Header("Movement")]
    public float smooth = 6f;
    public float sensitivity = 1f;
    public bool invertY = false;

    Vector2 current;

    Renderer rend;
    MaterialPropertyBlock mpb;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (rend == null) return;

        Vector2 target = GetNormalizedCursor();
        current = Vector2.Lerp(current, target, Time.deltaTime * smooth);

        ApplyCursor(current);
    }

    Vector2 GetNormalizedCursor()
    {
        Vector2 mouse = Input.mousePosition;

        mouse.x /= Screen.width;
        mouse.y /= Screen.height;

        mouse = mouse * 2f - Vector2.one;

        if (invertY)
            mouse.y *= -1f;

        return mouse * sensitivity;
    }

    void ApplyCursor(Vector2 value)
    {
        rend.GetPropertyBlock(mpb);
        mpb.SetVector("_Cursor", new Vector4(value.x, value.y, 0, 0));
        rend.SetPropertyBlock(mpb);
    }

    void OnDisable()
    {
        ResetCursor();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
            ResetCursor();
    }
#endif

    void ResetCursor()
    {
        if (rend == null) return;

        rend.GetPropertyBlock(mpb);
        mpb.SetVector("_Cursor", Vector4.zero);
        rend.SetPropertyBlock(mpb);
    }
}