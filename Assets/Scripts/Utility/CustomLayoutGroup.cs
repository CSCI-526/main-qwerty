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
    public bool stretchChildren = false;

    public List<RectTransform> children = new List<RectTransform>();

    private void Awake() => RefreshLayout();
#if UNITY_EDITOR
    private void OnValidate() => RefreshLayout();
#endif

    /// <summary>
    /// Call this method to refresh the child list and reposition. (Useful if children added/removed manually.)
    /// </summary>
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

    /// <summary>
    /// Adds a RectTransform child to this layout group and repositions all.
    /// </summary>
    public void AddToLayout(RectTransform child)
    {
        if (!children.Contains(child))
        {
            child.SetParent(transform, false);
            children.Add(child);
            ArrangeChildren();
        }
    }

    /// <summary>
    /// Removes a RectTransform child from this layout group and repositions all.
    /// </summary>
    public void RemoveFromLayout(RectTransform child)
    {
        if (children.Contains(child))
        {
            children.Remove(child);
            ArrangeChildren();
        }
    }

    /// <summary>
    /// Toggle layout direction between horizontal/vertical.
    /// </summary>
    public void ToggleDirection()
    {
        direction = direction == LayoutDirection.Horizontal ? LayoutDirection.Vertical : LayoutDirection.Horizontal;
        ArrangeChildren();
    }

    /// <summary>
    /// Toggle alignment among Start, Center, End.
    /// </summary>
    public void CycleAlignment()
    {
        alignment = (LayoutAlignment)(((int)alignment + 1) % 3);
        ArrangeChildren();
    }

    private void ArrangeChildren()
    {
        RefreshChildren();

        var parentRect = transform as RectTransform;
        if (parentRect == null || children.Count == 0) return;

        // Parent inner bounds minus padding
        float innerWidth = parentRect.rect.width - padding.left - padding.right;
        float innerHeight = parentRect.rect.height - padding.top - padding.bottom;

        // Calculate total size of children (not including spacing yet)
        float totalChildrenSize = 0f;
        foreach (RectTransform child in children)
        {
            Debug.Log("Checking RectTransform: " + child.name);
            totalChildrenSize += direction == LayoutDirection.Horizontal ? child.sizeDelta.x : child.sizeDelta.y;
        }

        float totalSpacing = Mathf.Max(0, children.Count - 1) * spacing;
        float totalLayoutSize = totalChildrenSize + totalSpacing;

        float startOffset;
        if (direction == LayoutDirection.Horizontal)
        {
            switch (alignment)
            {
                case LayoutAlignment.Start:
                    startOffset = padding.left;
                    break;
                case LayoutAlignment.Center:
                    startOffset = padding.left + (innerWidth - totalLayoutSize) / 2f;
                    break;
                case LayoutAlignment.End:
                    startOffset = parentRect.rect.width - padding.right - totalLayoutSize;
                    break;
                default:
                    startOffset = padding.left;
                    break;
            }
        }
        else
        {
            switch (alignment)
            {
                case LayoutAlignment.Start:
                    startOffset = -padding.top;
                    break;
                case LayoutAlignment.Center:
                    startOffset = -padding.top - (innerHeight - totalLayoutSize) / 2f;
                    break;
                case LayoutAlignment.End:
                    startOffset = -parentRect.rect.height + padding.bottom + totalLayoutSize;
                    break;
                default:
                    startOffset = -padding.top;
                    break;
            }
        }

        float offset = startOffset;
        foreach (RectTransform child in children)
        {
            Vector2 pos;
            if (direction == LayoutDirection.Horizontal)
            {
                pos = new Vector2(offset, -padding.top);

                // Optionally stretch children to fit height
                if (stretchChildren)
                    child.sizeDelta = new Vector2(child.sizeDelta.x, innerHeight);

                offset += child.sizeDelta.x + spacing;
            }
            else
            {
                pos = new Vector2(padding.left, offset);

                // Optionally stretch children to fit width
                if (stretchChildren)
                    child.sizeDelta = new Vector2(innerWidth, child.sizeDelta.y);

                offset -= (child.sizeDelta.y + spacing);
            }

            child.anchorMin = child.anchorMax = new Vector2(0, 1); // Top-left anchor
            child.pivot = new Vector2(0, 1);
            child.anchoredPosition = pos;
        }
    }
}
