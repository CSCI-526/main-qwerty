using UnityEngine;
using UnityEngine.UIElements;

public class CameraToWorldSpacePosition : MonoBehaviour
{

    [SerializeField] private GameObject parentObject;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        if (Camera.main != null && parentObject != null)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, parentObject.transform.position + offset);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, Camera.main.farClipPlane - 10));

            transform.position = worldPoint;
        }
    }

}
