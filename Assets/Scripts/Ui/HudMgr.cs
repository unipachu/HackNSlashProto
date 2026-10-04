using UnityEngine;
using UnityEngine.UI;

public class HudMgr : Singleton<HudMgr> {
    [SerializeField] Image imgHp;
    [SerializeField] Image imgHpYellow;
    [SerializeField] float yellowBarSpd = 1;
    [SerializeField] float yellowWaitUntilTrail = 0.5f;

    CpHandle plrCp;
    HpBarTrailData hpTrail;

    // -------------------------------------------------------------------------------
    // Tick Methods
    // -------------------------------------------------------------------------------

    public void LateTick() {
        if(plrCp != null)
            UiUtils.UpdateYellowTrail(
                imgHp,
                imgHpYellow,
                ref hpTrail,
                Time.time,
                yellowBarSpd,
                yellowWaitUntilTrail
            );
    }

    // -------------------------------------------------------------------------------
    // Other Methods
    // -------------------------------------------------------------------------------

    public void ClearPlr() {
        Debug.Assert(plrCp != null, "No player was set!");
        plrCp.Data.action_curHpChanged -= OnPlrCurHpChanged;
        plrCp.Data.action_maxHpChanged -= OnPlrMaxHpChanged;
        plrCp.Data.action_markedForPendingUnregister -= OnPlrMarkedForPendingUnregister;
        plrCp = null;
        UiUtils.ResetYellowTrail(imgHp, imgHpYellow, ref hpTrail);
    }

    void OnPlrCurHpChanged(int curHp, int maxHp) {
        UiUtils.SetHp(imgHp, curHp, maxHp);
    }

    void OnPlrMarkedForPendingUnregister() {
        ClearPlr();
    }

    void OnPlrMaxHpChanged(int curHp, int maxHp) {
        UiUtils.SetHp(imgHp, curHp, maxHp);
    }

    public void SetPlr(CpHandle cp) {
        Debug.Assert(cp != null, "Player CpHandle was null!");
        Debug.Assert(plrCp == null, "Player already set!");
        Debug.Assert(imgHp != null, "HUD HP image was null!");
        Debug.Assert(imgHpYellow != null, "HUD HP yellow image was null!");
        plrCp = cp;
        plrCp.Data.action_curHpChanged += OnPlrCurHpChanged;
        plrCp.Data.action_maxHpChanged += OnPlrMaxHpChanged;
        plrCp.Data.action_markedForPendingUnregister += OnPlrMarkedForPendingUnregister;
        UiUtils.SetHp(imgHp, plrCp.Data.hp_Cur, plrCp.so_cpData.hp_Max);
        UiUtils.ResetYellowTrail(imgHp, imgHpYellow, ref hpTrail);
    }
}