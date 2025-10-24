using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject fillBar;
    [SerializeField] GameObject healthBar;
    [SerializeField] TMP_Text healthText;

    private void Start()
    {
        fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBar.GetComponent<RectTransform>().sizeDelta.x, fillBar.GetComponent<RectTransform>().sizeDelta.y);
    }

    public void SetFillAmount(int currentHealth, int maxHealth)
    {
        fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBar.GetComponent<RectTransform>().sizeDelta.x * currentHealth / maxHealth, fillBar.GetComponent<RectTransform>().sizeDelta.y);
        healthText.text = currentHealth + " / " + maxHealth;
    }
}
