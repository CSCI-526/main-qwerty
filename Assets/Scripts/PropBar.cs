using System;
using UnityEngine;
using UnityEngine.UI;

public class PropBar : MonoBehaviour
{
    public Image[] cells;
    public Color activeColor;
    public Color inactiveColor;

    private void Start()
    {
        SetLevel(0);
    }

    public void SetLevel(int level)
    {
        // [-3, 3]
        level = Math.Min(Math.Max(level, -3), 3);

        int index = level + 3;
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].color = (i <= index) ? activeColor : inactiveColor;            
        }
    }
}