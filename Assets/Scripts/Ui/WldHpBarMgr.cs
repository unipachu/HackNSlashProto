using UnityEngine;

public sealed class WldHpBarMgr : Singleton<WldHpBarMgr> {
    [SerializeField] RectTransform wldHpBarLayer;
    [SerializeField] WldHpBar wldHpBarPrefab;
    [SerializeField] Camera cam;
    [SerializeField] float screenMargin = 40f;
    [SerializeField] float visibleAfterDamageTime = 2f;
    [SerializeField] int initCapacity = 8;

    WldHpBarData[] bars;
    int entityCount;

    // TODO: init in game manager
    protected override void Awake() {
        base.Awake();
        bars = new WldHpBarData[initCapacity];
    }

    // ----------------------------------------------------------------------------------
    // Register and Unregister
    // ----------------------------------------------------------------------------------

    public WldHpBar Register(Transform anchor) {
        WldHpBar hpBar = Instantiate(wldHpBarPrefab, wldHpBarLayer);
        WldHpBarData data = new() {
            hpBar = hpBar,
            rect = (RectTransform)hpBar.transform,
            anchor = anchor,
            isLocked = false,
            visibleUntil = 0f
        };
        hpBar.gameObject.SetActive(false);
        entityCount = ArrayUtils.Add(
            ref bars,
            entityCount,
            data
        );
        return hpBar;
    }

    /// <summary>
    /// NOTE: Expects that the hp bar game object has been destroyed earlier.
    /// </summary>
    public void Unregister(int i) {
        //Debug.Log($"Unregistering hp bar {i}");
        GameObject.Destroy(bars[i].hpBar.gameObject);
        entityCount = ArrayUtils.RemoveAtSwapBack(bars, entityCount, i);
    }

    // ----------------------------------------------------------------------------------
    // Tick Methods
    // ----------------------------------------------------------------------------------

    public void LateTick() {
        LateTick_UpdateBarPosition();
        LateTick_UnregisterDestroyedHandles();
    }

    void LateTick_UpdateBarPosition() {
        for (int i = 0; i < entityCount; i++) {
            if (bars[i].pendingUnregister)
                continue;
            float now = Time.time;
            //Debug.Log($"{nameof(WldHpBarMgr)} {nameof(entityCount)}: {entityCount}");
            ref WldHpBarData data = ref bars[i];
            bool shouldBeVisible = data.isLocked || now < data.visibleUntil;
            if (!shouldBeVisible) {
                data.hpBar.gameObject.SetActive(false);
                continue;
            }
            // TODO: Enemy should have a destroyed action this can listen to.
            if (data.anchor == null) {
                data.pendingUnregister = true;
                return;
            }
            Vector3 screenPos = cam.WorldToScreenPoint(data.anchor.position);
            if (screenPos.z <= 0f) {
                data.hpBar.gameObject.SetActive(false);
                return;
            }
            data.hpBar.gameObject.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                wldHpBarLayer,
                screenPos,
                null,
                out Vector2 localPos
            );
            Rect layerRect = wldHpBarLayer.rect;
            float minX = layerRect.xMin + screenMargin + data.rect.rect.width * data.rect.pivot.x;
            float maxX = layerRect.xMax - screenMargin - data.rect.rect.width * (1f - data.rect.pivot.x);
            float minY = layerRect.yMin + screenMargin + data.rect.rect.height * data.rect.pivot.y;
            float maxY = layerRect.yMax - screenMargin - data.rect.rect.height * (1f - data.rect.pivot.y);
            localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
            localPos.y = Mathf.Clamp(localPos.y, minY, maxY);
            data.rect.anchoredPosition = localPos;
        }
    }

    void LateTick_UnregisterDestroyedHandles() {
        int i = 0;
        // We swap the last element in the place of the unregistered one, so we onlu increment index if we
        // don't unregister a cp.
        while (i < entityCount) {
            //Debug.Log($"Checked {i} for null hpbar: {bars[i].hpBar}");
            if (bars[i].pendingUnregister)
                Unregister(i);
            else
                i++;
        }
    }

    // ----------------------------------------------------------------------------------
    // Other Methods
    // ----------------------------------------------------------------------------------

    // TODO: enemy should have a event action this can listen to.
    public void ReportDamage(WldHpBar hpBar) {
        for (int i = 0; i < entityCount; i++) {
            ref WldHpBarData data = ref bars[i];
            if (data.hpBar != hpBar)
                continue;
            data.visibleUntil = Time.time + visibleAfterDamageTime;
            return;
        }
    }

    // TODO: enemy should have a event action this can listen to.
    public void SetLocked(WldHpBar hpBar, bool isLocked) {
        for (int i = 0; i < entityCount; i++) {
            ref WldHpBarData data = ref bars[i];
            if (data.hpBar != hpBar)
                continue;
            data.isLocked = isLocked;
            if (isLocked)
                data.visibleUntil = float.PositiveInfinity;
            else
                data.visibleUntil = Time.time + visibleAfterDamageTime;
            return;
        }
    }
}