using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-related utility and extension methods.
/// </summary>
public static class UiUtils {
    public static void ResetYellowTrail(
        Image imgDmgRed,
        Image imgDmgYellow,
        ref HpBarTrailData data
    ) {
        imgDmgYellow.fillAmount = imgDmgRed.fillAmount;
        data.st = HpBarTrailingSt.Settled;
    }

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

    public static void SetHp(Image imgHp, int curHp, int maxHp) {
        imgHp.fillAmount = maxHp > 0
            ? Mathf.Clamp01((float)curHp / maxHp)
            : 0f;
    }

    public static void UpdateYellowTrail(
        Image imgDmgRed,
        Image imgDmgYellow,
        ref HpBarTrailData data,
        float now,
        float yellowBarSpd,
        float yellowWaitUntilTrail
    ) {
        if (imgDmgYellow.fillAmount <= imgDmgRed.fillAmount)
            return;
        switch (data.st) {
            case HpBarTrailingSt.DelayingDecrease:
                if (now > data.delayStartTime + yellowWaitUntilTrail)
                    data.st = HpBarTrailingSt.Decreasing;
                break;
            case HpBarTrailingSt.Decreasing:
                imgDmgYellow.fillAmount -= yellowBarSpd * Time.deltaTime;
                if (imgDmgYellow.fillAmount <= imgDmgRed.fillAmount) {
                    imgDmgYellow.fillAmount = imgDmgRed.fillAmount;
                    data.st = HpBarTrailingSt.Settled;
                }
                break;
            case HpBarTrailingSt.Settled:
                data.st = HpBarTrailingSt.DelayingDecrease;
                data.delayStartTime = now;
                break;
        }
    }

    public static void VerticalSineMovement(
        this RectTransform rectTrf,
        Vector3 startPos,
        float spd,
        float amp
    ) {
        float yOffset = Mathf.Sin(Time.time * spd) * amp;
        rectTrf.anchoredPosition = startPos + new Vector3(0f, yOffset, 0f);
    }
}
