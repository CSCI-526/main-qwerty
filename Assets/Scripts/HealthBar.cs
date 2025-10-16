using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject fillBar;
    [SerializeField] GameObject healthBar;

    private void Start()
    {
        fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBar.GetComponent<RectTransform>().sizeDelta.x, fillBar.GetComponent<RectTransform>().sizeDelta.y);
    }

    public void SetFillAmount(float amount)
    {
        fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBar.GetComponent<RectTransform>().sizeDelta.x * amount, fillBar.GetComponent<RectTransform>().sizeDelta.y);
    }
}
