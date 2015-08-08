using UnityEngine;
using System.Collections;

public class CanvasConstants : MonoBehaviour
{
    public Canvas canvas;
    [HideInInspector]
    public static float canvasWidth
    {
        get
        {
            if (canvasRect != null)
                return canvasRect.rect.width;
            else
                return Screen.width;
        }
    }
    [HideInInspector]
    public static float canvasHeight
    {
        get
        {
            if (canvasRect != null)
                return canvasRect.rect.height;
            else
                return Screen.height;
        }
    }

    private static RectTransform canvasRect;

    // Use this for initialization
    void Start()
    {
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }
}