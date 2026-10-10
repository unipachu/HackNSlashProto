using UnityEngine;

public sealed class WldHpBarMgr : Singleton<WldHpBarMgr> {
    public float barVisibleDur = 3;
    [SerializeField] Camera cam;
    [SerializeField] int initCapacity = 8;
    [SerializeField] float screenMargin = 40;
    [SerializeField] RectTransform wldHpBarLayer;
    [SerializeField] WldHpBar wldHpBarPrefab;
    [SerializeField] float yellowBarSpd = 1;
    [SerializeField] float yellowWaitUntilTrail = 0.5f;

    [HideInInspector] public WldHpBarData[] bars;

    int entityCount;

    public void Init() {
        bars = new WldHpBarData[initCapacity];
    }

    // ----------------------------------------------------------------------------------
    // Register and Unregister
    // ----------------------------------------------------------------------------------

    public void Register(Transform anchor, ICp cpHandle) {
        int newI = entityCount;
        // TODO MINOR: Pool these?
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
        data.cpHandle.CommonData.action_CurHpChanged += hpBar.OnCurHpChanged;
        data.cpHandle.CommonData.action_DmgTaken += hpBar.OnDmgTaken;
        data.cpHandle.CommonData.action_MarkedForPendingUnregister += hpBar.OnCpMarkedForUnregister;
        data.cpHandle.CommonData.action_MaxHpChanged += hpBar.OnMaxHpChanged;
        data.cpHandle.CommonData.action_PlrLockedOnStarted += hpBar.OnPlrLockedOnStarted;
        data.cpHandle.CommonData.action_PlrLockedOnEnded += hpBar.OnPlrLockedOnEnded;
        hpBar.I = newI;
        entityCount++;
        hpBar.gameObject.SetActive(false);
        hpBar.SetName(cpHandle.CpCommonConfig.displayName);
        hpBar.SetHp(cpHandle.CommonData.hp_Cur, cpHandle.CpCommonConfig.hp_Max);
        hpBar.SetYellowHp(cpHandle.CommonData.hp_Cur, cpHandle.CpCommonConfig.hp_Max);
    }

    /// <summary>
    /// NOTE: Expects that the hp bar game object has been destroyed earlier.
    /// </summary>
    public void Unregister(int i) {
        ref var data = ref bars[i];
        data.cpHandle.CommonData.action_CurHpChanged -= data.hpBar.OnCurHpChanged;
        data.cpHandle.CommonData.action_DmgTaken -= data.hpBar.OnDmgTaken;
        data.cpHandle.CommonData.action_MarkedForPendingUnregister -= data.hpBar.OnCpMarkedForUnregister;
        data.cpHandle.CommonData.action_MaxHpChanged -= data.hpBar.OnMaxHpChanged;
        data.cpHandle.CommonData.action_PlrLockedOnStarted -= data.hpBar.OnPlrLockedOnStarted;
        data.cpHandle.CommonData.action_PlrLockedOnEnded -= data.hpBar.OnPlrLockedOnEnded;
        int lastId = entityCount - 1;
        WldHpBar swappedCtrl = i != lastId
            ? bars[lastId].hpBar
            : null;
        GameObject.Destroy(bars[i].hpBar.gameObject);
        ArrayUtils.RemoveAtSwapBack(bars, entityCount, i);
        entityCount--;
        if (swappedCtrl != null)
            swappedCtrl.I = i;
        //Debug.Log($"Unregistered {typeof(WldHpBar)} i: {i}");
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
                UiUtils.ResetYellowTrail(data.hpBar.imgDmgRed, data.hpBar.imgDmgYellow, ref data.trailData);
                data.hpBar.gameObject.SetActive(false);
                continue;
            }
            // Move yellow trail.
            UiUtils.UpdateYellowTrail(
                data.hpBar.imgDmgRed,
                data.hpBar.imgDmgYellow,
                ref data.trailData,
                now,
                yellowBarSpd,
                yellowWaitUntilTrail
            );
            // Handle dmg number visibility
            if (now > data.dmgNumberVisibleUntil) {
                data.accumulatedDmg = 0;
                data.hpBar.SetDmgText(0);
            }
            Vector3 screenPos = cam.WorldToScreenPoint(data.anchor.position);
            // Hide hp bar if its behind camera.
            if (screenPos.z < 0) {
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