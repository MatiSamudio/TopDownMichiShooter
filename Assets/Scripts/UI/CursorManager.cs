using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D crosshairTexture;
    // punto “caliente” (el centro de la mira)
    public Vector2 hotspot = new Vector2(16, 16); // ajustá según tamaño

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(crosshairTexture, hotspot, CursorMode.Auto);
    }
}
