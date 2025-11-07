using UnityEngine;

public class ForceRectTransform : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private Vector3 scale = Vector3.one;
    [SerializeField] private Quaternion rotation = Quaternion.identity;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (rectTransform == null) return;
        rectTransform.localScale = scale;
    }
}
