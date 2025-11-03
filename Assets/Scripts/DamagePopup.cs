using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro text;
    public float lifetime = 0.5f;
    public Vector3 moveSpeed = new Vector3(0, 1f, 0);
    public float fadeDuration = 0.5f;
    public float scaleUp = 1.2f;

    private Color startColor;
    private float timer;

    void Start()
    {
        startColor = text.color;
        transform.localScale = Vector3.one * scaleUp;
        moveSpeed.x += Random.Range(-0.5f, 0.5f);
    }


    void Update()
    {
        timer += Time.deltaTime;

        // Move upward
        transform.position += moveSpeed * Time.deltaTime;

        // Shrink
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, timer / lifetime);

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

    public void Setup(int damage)
    {
        text.text = damage.ToString();
    }
}
