using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-related utility and extension methods.
/// </summary>
public static class UiUtils {
    /// <summary>
    /// When wrap mode of image's sprite is repeat, this will cause a horizontal scrolling effect when called from Update.
    /// </summary>
    public static void ScrollImage(this RawImage image, float scrollSpeed) {
        image.uvRect = new Rect(image.uvRect.position + new Vector2(scrollSpeed * Time.deltaTime, 0), image.uvRect.size);
    }

    /// <summary>
    /// When wrap mode of image's sprite is repeat, this will cause a horizontal scrolling effect when called from Update.
    /// </summary>
    public static void ScrollImages(this RawImage[] images, float scrollSpeed) {
        foreach (RawImage image in images) {
            image.uvRect = new Rect(image.uvRect.position + new Vector2(scrollSpeed * Time.deltaTime, 0), image.uvRect.size);
        }
    }

    public static void VerticalSineMovement(this RectTransform rectTransform, Vector3 startPos, float speed, float amplitude) {
        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        rectTransform.anchoredPosition = startPos + new Vector3(0f, yOffset, 0f);
    }
}
