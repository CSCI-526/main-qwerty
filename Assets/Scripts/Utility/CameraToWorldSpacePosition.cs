using UnityEngine;
using UnityEngine.UIElements;

public class CameraToWorldSpacePosition : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    [SerializeField] private Vector3 offset; // screen‑space offset at 4K

    private const float REF_HEIGHT = 2160f;  // 4K reference

    private void Update()
    {
        if (Camera.main == null || parentObject == null) return;

        // 1. Get the screen position for the parent + offset (same as before)
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            null,
            parentObject.transform.position + offset
        );

        // 2. Scale the depth so lower resolutions move the object closer to camera
        float baseDepth = Camera.main.farClipPlane - 10f; // your original depth
        float heightScale = Screen.height / REF_HEIGHT;   // < 1 on lower res
        float scaledDepth = baseDepth * heightScale;      // closer when scale < 1

        // 3. Convert to world position using the scaled depth
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPoint.x, screenPoint.y, scaledDepth)
        );

        transform.position = worldPoint;
    }
}
