using UnityEngine;
using TMPro;
public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject fillBar;      // Standard health fill
    [SerializeField] GameObject shieldBar;    // Overlay fill for shield (assign in Inspector)
    [SerializeField] GameObject healthBar;    // The full background bar
    [SerializeField] TMP_Text healthText;     // Health text display

    private void Start()
    {
        float maxWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;
        fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, fillBar.GetComponent<RectTransform>().sizeDelta.y);
        shieldBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, shieldBar.GetComponent<RectTransform>().sizeDelta.y);
        shieldBar.SetActive(false);
    }

    public void SetFillAmount(int currentHealth, int currentShield, int maxHealth)
    {
        float maxWidth = healthBar.GetComponent<RectTransform>().sizeDelta.x;

        if (currentShield <= 0)
        {
            // Case 1: No shield, only health
            float healthPortion = Mathf.Clamp01((float)currentHealth / maxHealth);
            float healthWidth = maxWidth * healthPortion;
            fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthWidth, fillBar.GetComponent<RectTransform>().sizeDelta.y);
            fillBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, fillBar.GetComponent<RectTransform>().anchoredPosition.y);

            shieldBar.SetActive(false);
        }
        else if (currentHealth + currentShield >= maxHealth)
        {
            // Case 2: Full bar, proportioned by health and shield
            float total = Mathf.Max(currentHealth + currentShield, 1); // Prevent div by zero
            float healthPercent = (float)currentHealth / total;
            float shieldPercent = (float)currentShield / total;

            float healthWidth = maxWidth * healthPercent;
            float shieldWidth = maxWidth * shieldPercent;

            fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthWidth, fillBar.GetComponent<RectTransform>().sizeDelta.y);
            fillBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, fillBar.GetComponent<RectTransform>().anchoredPosition.y);

            shieldBar.SetActive(true);
            var shieldRect = shieldBar.GetComponent<RectTransform>();
            shieldRect.sizeDelta = new Vector2(shieldWidth, shieldRect.sizeDelta.y);
            shieldRect.anchoredPosition = new Vector2(healthWidth, shieldRect.anchoredPosition.y);
        }
        else
        {
            // Case 3: Partially filled, separate health/shield/empty
            float healthPortion = Mathf.Clamp01((float)currentHealth / maxHealth);
            float shieldPortion = Mathf.Clamp01((float)currentShield / maxHealth);

            float healthWidth = maxWidth * healthPortion;
            float shieldWidth = maxWidth * shieldPortion;

            fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthWidth, fillBar.GetComponent<RectTransform>().sizeDelta.y);
            fillBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, fillBar.GetComponent<RectTransform>().anchoredPosition.y);

            shieldBar.SetActive(true);
            var shieldRect = shieldBar.GetComponent<RectTransform>();
            shieldRect.sizeDelta = new Vector2(shieldWidth, shieldRect.sizeDelta.y);
            shieldRect.anchoredPosition = new Vector2(healthWidth, shieldRect.anchoredPosition.y);
        }

        if (currentShield > 0)
            healthText.text = currentHealth + " (+" + currentShield + ") / " + maxHealth;
        else
            healthText.text = currentHealth + " / " + maxHealth;
    }
}

