﻿using UnityEngine;
using UnityEngine.UI;

public class AlphaRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
    public float minAlpha;

    private RectTransform rectTransform;
    private Image image;

    void Awake()
    {
        rectTransform = transform as RectTransform;
        image = GetComponent<Image>();
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        // Get normalized hit point within rectangle (aka UV coordinates originating from bottom-left)
        Vector2 rectPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out rectPoint);
        Vector2 normPoint = (rectPoint - rectTransform.rect.min);
        normPoint.x /= rectTransform.rect.width;
        normPoint.y /= rectTransform.rect.height;

        // Read pixel color at normalized hit point
        Texture2D texture = image.sprite.texture;
        Color color = texture.GetPixel((int)(normPoint.x * texture.width), (int)(normPoint.y * texture.height));

        // Keep hits on pixels above minimum alpha
        return color.a > minAlpha;
    }
}