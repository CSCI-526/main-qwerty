using Unity.Netcode;
using UnityEngine;

public class RectTransformSync : NetworkBehaviour
{
    private RectTransform rectTransform;

    private NetworkVariable<Vector2> syncedPosition = new NetworkVariable<Vector2>();
    private NetworkVariable<Vector3> syncedScale = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> syncedRotation = new NetworkVariable<Quaternion>();

    [SerializeField] private float lerpSpeed = 10f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            syncedPosition.Value = rectTransform.anchoredPosition;
            syncedRotation.Value = rectTransform.localRotation;
            syncedScale.Value = rectTransform.localScale;
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, syncedPosition.Value, Time.deltaTime * lerpSpeed);
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, syncedRotation.Value, Time.deltaTime * lerpSpeed);
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, syncedScale.Value, Time.deltaTime * lerpSpeed);
        }
    }
}
