using UnityEngine;

public class CursorToShader : MonoBehaviour
{
    public Material mat;

    [Header("Movement")]
    public float smooth = 6f;
    public float sensitivity = 1f;
    public bool invertY = false;

    Vector2 current;

    void Update()
    {
        if (mat == null) return;

        Vector2 target = GetNormalizedCursor();

        current = Vector2.Lerp(current, target, Time.deltaTime * smooth);

        mat.SetVector("_Cursor", new Vector4(current.x, current.y, 0, 0));
    }

    Vector2 GetNormalizedCursor()
    {
        Vector2 mouse = Input.mousePosition;

        // normalize 0-1
        mouse.x /= Screen.width;
        mouse.y /= Screen.height;

        // to -1 to 1
        mouse = mouse * 2f - Vector2.one;

        // invert
        if (invertY)
            mouse.y *= -1f;

        return mouse * sensitivity;
    }
}