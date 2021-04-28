using UnityEngine;

public static class Utilities
{
    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return  new Rect((Vector2)transform.position - size * transform.pivot, size);
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
}