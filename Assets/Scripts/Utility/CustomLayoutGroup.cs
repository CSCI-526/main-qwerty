using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteAlways]
[AddComponentMenu("UI/Custom Layout Group")]
public class CustomLayoutGroup : MonoBehaviour
{
    public enum LayoutDirection { Horizontal, Vertical }
    public enum LayoutAlignment { Start, Center, End }

    [Header("Layout Settings")]
    public LayoutDirection direction = LayoutDirection.Horizontal;
    public LayoutAlignment alignment = LayoutAlignment.Center;
    public float spacing = 10f;
    public RectOffset padding = new RectOffset();
    public bool reverseOrder = false;

    private List<RectTransform> children = new List<RectTransform>();

    private void Awake() => RefreshLayout();
#if UNITY_EDITOR
    private void OnValidate() => RefreshLayout();
#endif

    public void RefreshLayout()
    {
        RefreshChildren();
        ArrangeChildren();
    }

    private void RefreshChildren()
    {
        children.Clear();
        foreach (Transform child in transform)
        {
            if (child is RectTransform rt && child.gameObject.activeSelf)
                children.Add(rt);
        }
        if (reverseOrder) children.Reverse();
    }

    public void AddToLayout(RectTransform child)
    {
        if (!children.Contains(child))
        {
            child.SetParent(transform, false);
            children.Add(child);
            ArrangeChildren();
        }
    }

    public void RemoveFromLayout(RectTransform child)
    {
        if (children.Contains(child))
        {
            children.Remove(child);
            ArrangeChildren();
        }
    }

    public void ToggleDirection()
    {
        direction = direction == LayoutDirection.Horizontal ? LayoutDirection.Vertical : LayoutDirection.Horizontal;
        ArrangeChildren();
    }

    private void ArrangeChildren()
    {
        // Get parent size
        var parentRect = transform as RectTransform;
        if (parentRect == null) return;

        // Calculate total size required
        float totalLength = -spacing;
        foreach (RectTransform child in children)
            totalLength += (direction == LayoutDirection.Horizontal ? child.rect.width : child.rect.height) + spacing;

        float startOffset;
        if (direction == LayoutDirection.Horizontal)
        {
            if (alignment == LayoutAlignment.Start)
                startOffset = padding.left;
            else if (alignment == LayoutAlignment.Center)
                startOffset = (parentRect.rect.width - totalLength) / 2 + padding.left - padding.right;
            else // End
                startOffset = parentRect.rect.width - totalLength - padding.right;
        }
        else
        {
            if (alignment == LayoutAlignment.Start)
                startOffset = -padding.top;
            else if (alignment == LayoutAlignment.Center)
                startOffset = -(parentRect.rect.height - totalLength) / 2 - padding.top + padding.bottom;
            else // End
                startOffset = -(parentRect.rect.height - totalLength) - padding.bottom;
        }

        float offset = startOffset;
        foreach (RectTransform child in children)
        {
            Vector2 pos = direction == LayoutDirection.Horizontal
                ? new Vector2(offset, -padding.top)
                : new Vector2(padding.left, offset);

            child.anchoredPosition = pos;

            offset += (direction == LayoutDirection.Horizontal ? child.rect.width : child.rect.height) + spacing;
        }
    }
}
