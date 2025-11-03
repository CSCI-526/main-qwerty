using UnityEngine;
using TMPro;

public class DamageManager : MonoBehaviour
{
    [Header("Popup Prefab")]
    [SerializeField] private GameObject damagePopupPrefab;

    [Header("Optional Visuals")]
    [SerializeField] private GameObject damageScreen;

    [Header("Popup Settings")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Vector3 popupOffset = new Vector3(0, 1.5f, 0);

    public void applyHealthChange(TargetableController target, int amount)
    {
        if (target == null) return;

        target.ModifyCurrentHealth(amount);
        showPopup(target.transform.position, amount);

        // Optional damage flash if it’s damage
        if (amount < 0 && damageScreen != null)
            StartCoroutine(flashDamageScreen(0.2f));
    }

    private void showPopup(Vector3 position, int amount)
    {
        if (damagePopupPrefab == null) return;

        var popup = Instantiate(damagePopupPrefab, position + popupOffset, Quaternion.identity);
        var popupScript = popup.GetComponent<DamagePopup>();

        if (popupScript != null)
        {
            popupScript.text.color = amount < 0 ? damageColor : healColor; // red for damage, green for heal
            popupScript.Setup(Mathf.Abs(amount)); // display absolute number
        }
    }

    public System.Collections.IEnumerator flashDamageScreen(float duration)
    {
        damageScreen.SetActive(true);
        yield return new WaitForSeconds(duration);
        damageScreen.SetActive(false);
    }
}
