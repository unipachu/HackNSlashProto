using System.Collections.Generic;
using UnityEngine;

public class PlrMgr : Singleton<PlrMgr>{
    [SerializeField] So_PlrCpConfig config;

    int curUlt;
    CpHumdHandle cp;

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
        ref var commonData = ref cp.CommonData;
        commonData.action_CurHpChanged -= OnPlrCurHpChanged;
        commonData.action_MaxHpChanged -= OnPlrMaxHpChanged;
        commonData.action_HitSomething -= OnPlrHitSomething;
        commonData.action_MarkedForPendingUnregister -= OnPlrMarkedForPendingUnregister;
        cp = null;
        HudMgr.inst.ResetYellowTrail();
    }

    void OnPlrCurHpChanged(int curHp, int maxHp) {
        HudMgr.inst.SetHp(curHp, maxHp);
    }

    void OnPlrHitSomething(HashSet<HitResult> hitResults) {
        // NOTE: Cannot gain ult meter when doing ult attack.
        if (cp.CommonData.classRefs.st_cur == cp.HumdData.classRefs.actSts.atk_FlyingAtk)
            return;
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

    public void SetPlr(CpHumdHandle cp) {
        Debug.Assert(this.cp == null, "Player already set!");
        this.cp = cp;
        ref var commonData = ref cp.CommonData;
        commonData.action_HitSomething += OnPlrHitSomething;
        commonData.action_CurHpChanged += OnPlrCurHpChanged;
        commonData.action_MaxHpChanged += OnPlrMaxHpChanged;
        commonData.action_MarkedForPendingUnregister += OnPlrMarkedForPendingUnregister;
        HudMgr.inst.SetHp(commonData.hp_Cur, cp.so_cpCommonData.hp_Max);
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
