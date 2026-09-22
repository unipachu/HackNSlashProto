using UnityEngine;

public sealed class WldHpBarMgr : Singleton<WldHpBarMgr> {
    public float barVisibleDur = 3;
    [SerializeField] Camera cam;
    [SerializeField] int initCapacity = 8;
    [SerializeField] float screenMargin = 40;
    [SerializeField] RectTransform wldHpBarLayer;
    [SerializeField] WldHpBar wldHpBarPrefab;

    [HideInInspector] public WldHpBarData[] bars;

    int entityCount;

    // TODO: init in game manager
    protected override void Awake() {
        base.Awake();
        bars = new WldHpBarData[initCapacity];
    }

    // ----------------------------------------------------------------------------------
    // Register and Unregister
    // ----------------------------------------------------------------------------------

    public void Register(Transform anchor, CpHandle cpHandle) {
        int newI = entityCount;
        // TODO: Pool these.
        WldHpBar hpBar = Instantiate(wldHpBarPrefab, wldHpBarLayer);
        WldHpBarData data = new() {
            anchor = anchor,
            cpHandle = cpHandle,
            hpBar = hpBar,
            isLocked = false,
            rect = (RectTransform)hpBar.transform,
            barVisibleUntil = 0f
        };
        ArrayUtils.Add(ref bars, entityCount, data);
        data.cpHandle.GetData().action_curHpChanged += hpBar.OnCurHpChanged;
        data.cpHandle.GetData().action_dmgTaken += hpBar.OnDmgTaken;
        data.cpHandle.GetData().action_markedForPendingUnregister += hpBar.OnCpMarkedForUnregister;
        data.cpHandle.GetData().action_maxHpChanged += hpBar.OnMaxHpChanged;
        data.cpHandle.GetData().action_plrLockedOnStarted += hpBar.OnPlrLockedOnStarted;
        data.cpHandle.GetData().action_plrLockedOnEnded += hpBar.OnPlrLockedOnEnded;
        hpBar.I = newI;
        entityCount++;
        hpBar.gameObject.SetActive(false);
        hpBar.SetName(cpHandle.GetData().displayName);
        hpBar.SetHp(cpHandle.GetData().hp_Cur, cpHandle.GetData().hp_Max);
        hpBar.SetYellowHp(cpHandle.GetData().hp_Cur, cpHandle.GetData().hp_Max);
    }

    /// <summary>
    /// NOTE: Expects that the hp bar game object has been destroyed earlier.
    /// </summary>
    public void Unregister(int i) {
        ref var data = ref bars[i];
        data.cpHandle.GetData().action_curHpChanged -= data.hpBar.OnCurHpChanged;
        data.cpHandle.GetData().action_dmgTaken -= data.hpBar.OnDmgTaken;
        data.cpHandle.GetData().action_markedForPendingUnregister -= data.hpBar.OnCpMarkedForUnregister;
        data.cpHandle.GetData().action_maxHpChanged -= data.hpBar.OnMaxHpChanged;
        data.cpHandle.GetData().action_plrLockedOnStarted -= data.hpBar.OnPlrLockedOnStarted;
        data.cpHandle.GetData().action_plrLockedOnEnded -= data.hpBar.OnPlrLockedOnEnded;
        int lastId = entityCount - 1;
        WldHpBar swappedCtrl = i != lastId
            ? bars[lastId].hpBar
            : null;
        GameObject.Destroy(bars[i].hpBar.gameObject);
        ArrayUtils.RemoveAtSwapBack(bars, entityCount, i);
        entityCount--;
        if (swappedCtrl != null)
            swappedCtrl.I = i;
        Debug.Log($"Unregistered {typeof(WldHpBar)} i: {i}");
    }

    // ----------------------------------------------------------------------------------
    // Tick Methods
    // ----------------------------------------------------------------------------------

    public void LateTick() {
        LateTick_UpdateBarPosAndVisibility();
        LateTick_UnregisterPending();
    }

    void LateTick_UpdateBarPosAndVisibility() {
        for (int i = 0; i < entityCount; i++) {
            if (bars[i].pendingUnregister)
                continue;
            float now = Time.time;
            //Debug.Log($"{nameof(WldHpBarMgr)} {nameof(entityCount)}: {entityCount}");
            ref WldHpBarData data = ref bars[i];
            bool barShouldBeVisible = data.isLocked || now < data.barVisibleUntil;
            if (!barShouldBeVisible) {
                data.hpBar.gameObject.SetActive(false);
                continue;
            }
            if (now > data.dmgNumberVisibleUntil) {
                data.accumulatedDmg = 0;
                data.hpBar.SetDmgText(0); // Hide dmg number after successive atk window.
            }
            Vector3 screenPos = cam.WorldToScreenPoint(data.anchor.position);
            if (screenPos.z < 0) { // Hide hp bar if its behind camera.
                data.hpBar.gameObject.SetActive(false);
                return;
            }
            //Debug.Log("Set hp bar visible");
            data.hpBar.gameObject.SetActive(true);
            // Clamp the hp bar to screen (like in Elden Ring!).
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

    /// <summary>
    /// Destroy hp bars marked for unregisteration.
    /// </summary>
    void LateTick_UnregisterPending() {
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
}