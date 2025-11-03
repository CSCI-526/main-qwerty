using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("Popup Settings")]
    public TMP_Text text;                   // Assign the TMP_Text component in prefab
    public float lifetime = 0.8f;           // Total lifetime of the popup
    public float fadeDuration = 0.5f;       // How long it takes to fade out
    public float minUpSpeed = 50f;          // Minimum upward speed
    public float maxUpSpeed = 100f;         // Maximum upward speed
    public float horizontalRange = 30f;     // Random horizontal movement range
    public float scaleUp = 1.2f;            // Initial pop scale

    private Vector2 moveDirection;
    private float moveSpeed;
    private float timer = 0f;
    private Color startColor;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void Setup(int amount)
    {
        text.text = amount.ToString();
        startColor = text.color;

        // Random upward speed
        moveSpeed = Random.Range(minUpSpeed, maxUpSpeed);

        // Random horizontal direction
        float horizontal = Random.Range(-horizontalRange, horizontalRange);
        moveDirection = new Vector2(horizontal, moveSpeed);

        // Start slightly bigger
        rectTransform.localScale = Vector3.one * scaleUp;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Move popup
        rectTransform.anchoredPosition += moveDirection * Time.deltaTime;

        // Gradually shrink to normal size
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, Vector3.one, timer / lifetime);

        // Fade out
        if (timer > lifetime - fadeDuration)
        {
            float fade = 1 - (timer - (lifetime - fadeDuration)) / fadeDuration;
            text.color = new Color(startColor.r, startColor.g, startColor.b, fade);
        }

        // Destroy after lifetime
        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
