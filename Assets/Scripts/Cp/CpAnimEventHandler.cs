using System;
using UnityEngine;

public class CpAnimEventHandler : MonoBehaviour {
    public Action<int, CpAnimEventT> animEvent;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    void OnAnimEvent(int id, CpAnimEventT animEvent) {
        //Debug.Log($"Anim event {animEvent} for {id} called!", this);
        var classRefs = CpMgr.inst.classRefs[id];
        switch (animEvent) {
            case CpAnimEventT.BufferedInputStSwitchAllowed:
                CpMgr.GetSoa(id).actStSt_BufferedInputStSwitchAllowed = true;
                break;
            case CpAnimEventT.ComboAllowed:
                CpMgr.GetSoa(id).actStSt_ComboAllowed = true;
                break;
            case CpAnimEventT.ComboDisallowed:
                CpMgr.GetSoa(id).actStSt_ComboAllowed = false;
                break;
            case CpAnimEventT.DodgeAllowed:
                CpMgr.GetSoa(id).actStSt_DodgeAllowed = true;
                break;
            case CpAnimEventT.Finished:
                classRefs.st_cur.HandleAnimEvent(CpAnimEventT.Finished);
                break;
            case CpAnimEventT.HitDealerActivated:
                classRefs.st_cur.HandleAnimEvent(CpAnimEventT.HitDealerActivated);
                break;
            case CpAnimEventT.HitDealerDeactivated:
                classRefs.st_cur.HandleAnimEvent(CpAnimEventT.HitDealerDeactivated);
                break;
            case CpAnimEventT.InvulEnd:
                CpMgr.GetSoa(id).invul = false;
                break;
            case CpAnimEventT.AirtimeEnded:
                CpMgr.GetSoa(id).isAffectedByGravity = true;
                CpMgr.GetSoa(id).vel_Ver = -CpMgr.GetSoa(id).st_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                CpMgr.GetSoa(id).isAffectedByGravity = false;
                break;
            case CpAnimEventT.YawAllowed:
                CpMgr.GetSoa(id).actStSt_InputRotAllowed = true;
                break;
            case CpAnimEventT.YawDisallowed:
                CpMgr.GetSoa(id).actStSt_InputRotAllowed = false;
                break;
            default:
                Debug.Log($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
