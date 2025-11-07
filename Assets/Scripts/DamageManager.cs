using UnityEngine;
using TMPro;
using System.Collections;

public class DamageManager : MonoBehaviour
{
    [Header("Popup Prefab")]
    [SerializeField] private GameObject damagePopupPrefab;

    [Header("Canvas for Popups")]
    [SerializeField] private Canvas popupCanvas; // Assign the canvas for popups here

    [Header("Optional Visuals")]
    [SerializeField] private GameObject damageScreen;

    [Header("Popup Settings")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Vector2 popupOffset = new Vector2(0, 50); // Offset in canvas units


    public void applyHealthChange(TargetableController target, int amount)
    {
        if (target == null) return;

        RectTransform targetRect = target.GetComponent<RectTransform>();
        if (targetRect != null)
        {
            showPopup(targetRect, amount);
        }

        if (amount < 0 && damageScreen != null && target.tag != "Enemy")
        {
            StartCoroutine(FlashDamageScreen(0.2f));
        }
    }

    private void showPopup(RectTransform targetRect, int amount)
    {
        if (damagePopupPrefab == null || popupCanvas == null)
        {
            return;
        }

        // Get the target's world position
        Vector3 worldPos = targetRect.position;

        // Convert world position to screen point
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);

        // Convert screen point to local position in popup canvas
        RectTransform canvasRect = popupCanvas.transform as RectTransform;
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out localPos);

        // Apply popup offset
        localPos += popupOffset;

        // Instantiate popup as child of popup canvas
        GameObject popup = Instantiate(damagePopupPrefab, popupCanvas.transform);
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = localPos;

        // Set text and color
        DamagePopup popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript != null)
        {
            if(amount < 0)
            {
                popupScript.text.color = damageColor;
                popupScript.Setup(amount);
            }
            else if (amount > 0)
            {
                popupScript.text.color = healColor;
                popupScript.Setup(amount);
            }
        }
    }

    public IEnumerator FlashDamageScreen(float duration)
    {
        if (damageScreen == null) yield break;

        damageScreen.SetActive(true);
        yield return new WaitForSeconds(duration);
        damageScreen.SetActive(false);
    }
}
