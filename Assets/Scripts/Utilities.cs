using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }
    
    public static bool IsTouchIntersectsButtons(this Rect rect, Vector2 point, params RectTransform[] transforms)
    {
        return rect.Contains(point) && IsIntersectsUI(point, transforms);
    }
    
    public static bool IsIntersectsUI(Vector2 point, params RectTransform[] transforms)
    {
        bool intersection = true;
        foreach (var transform in transforms)
        {
            var rect = RectTransformToScreenSpace(transform);
            intersection &= !rect.Contains(point);
        }

        return intersection;
    }
    public static int Round(float input)
    {
        float output = input;
        int outputModifier = 0;
        output -= (int) input;
        if (output >= 0.5f)
        {
            outputModifier = 1;
        }

        return (int) input + outputModifier;
    }
    public static int GetMaxInt(this List<int> list)
    {
        int max = list[0];
        for(int i = 1; i < list.Count; i++) 
        {
            max = Math.Max(max, list[i]);
        }
        return max;
    }
}