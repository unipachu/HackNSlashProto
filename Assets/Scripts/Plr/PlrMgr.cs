using System.Collections.Generic;
using UnityEngine;

public class PlrMgr : Singleton<PlrMgr>{
    [SerializeField] So_PlrCpConfig config;

    int curUlt;
    CpHandle cp;

    // -------------------------------------------------------------------------------
    // Tick Methods
    // -------------------------------------------------------------------------------

    public void LateTick() {
        if (cp != null)
            HudMgr.inst.Tick();
    }

    // -------------------------------------------------------------------------------
    // Other Methods
    // -------------------------------------------------------------------------------

    void ClearPlr() {
        Debug.Assert(cp != null, "No player was set!");
        cp.Data.action_CurHpChanged -= OnPlrCurHpChanged;
        cp.Data.action_MaxHpChanged -= OnPlrMaxHpChanged;
        cp.Data.action_HitSomething -= OnPlrHitSomething;
        cp.Data.action_MarkedForPendingUnregister -= OnPlrMarkedForPendingUnregister;
        cp = null;
        HudMgr.inst.ResetYellowTrail();
    }

    void OnPlrCurHpChanged(int curHp, int maxHp) {
        HudMgr.inst.SetHp(curHp, maxHp);
    }

    void OnPlrHitSomething(HashSet<HitResult> hitResults) {
        foreach(var hitResult in hitResults) {
            // We increase ult meter for each hit that dealt damage.
            if (hitResult.dmgDealt > 0)
                curUlt = Mathf.Min(config.maxUlt, curUlt + 1);
            if(curUlt == config.maxUlt) {
                // TODO: Play some effect etc?
                break;
            }
        }
        //Debug.Log($"Updated Ult meter to: {curUlt}");
        HudMgr.inst.SetUlt(curUlt, config.maxUlt);
    }

    void OnPlrMarkedForPendingUnregister() {
        ClearPlr();
    }

    void OnPlrMaxHpChanged(int curHp, int maxHp) {
        HudMgr.inst.SetHp(curHp, maxHp);
    }

    public void SetPlr(CpHandle cp) {
        Debug.Assert(this.cp == null, "Player already set!");
        this.cp = cp;
        cp.Data.action_HitSomething += OnPlrHitSomething;
        cp.Data.action_CurHpChanged += OnPlrCurHpChanged;
        cp.Data.action_MaxHpChanged += OnPlrMaxHpChanged;
        cp.Data.action_MarkedForPendingUnregister += OnPlrMarkedForPendingUnregister;
        HudMgr.inst.SetHp(cp.Data.hp_Cur, cp.so_cpData.hp_Max);
        HudMgr.inst.ResetYellowTrail();
        HudMgr.inst.SetUlt(0, config.maxUlt);
    }

    public bool TryConsumeUltMeter() {
        //Debug.Log($"Plr tried to consume ult meter. {nameof(curUlt)}: {curUlt}, " 
        //    + $"{nameof(config.maxUlt)}: {config.maxUlt}.");
        if (curUlt == config.maxUlt) {
            curUlt = 0;
            HudMgr.inst.SetUlt(curUlt, config.maxUlt);
            return true;
        }
        return false;
    }
}
