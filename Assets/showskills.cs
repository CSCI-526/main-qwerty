using UnityEngine;
using UnityEngine.EventSystems;

public class showskills : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI to Show on Hover")]
    public GameObject hoverUI; // The panel or text you want to show

    void Start()
    {
        if (hoverUI != null)
            hoverUI.SetActive(false); // hide by default
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverUI != null)
            hoverUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverUI != null)
            hoverUI.SetActive(false);
    }
}
