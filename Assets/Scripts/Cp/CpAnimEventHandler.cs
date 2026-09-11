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
                CpMgr.GetAos(id).bufferedInputStSwitchAllowed = true;
                break;
            case CpAnimEventT.ComboAllowed:
                CpMgr.GetAos(id).comboAllowed = true;
                break;
            case CpAnimEventT.ComboDisallowed:
                CpMgr.GetAos(id).comboAllowed = false;
                break;
            case CpAnimEventT.DodgeAllowed:
                CpMgr.GetAos(id).dodgeAllowed = true;
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
                CpMgr.GetAos(id).invul = false;
                break;
            case CpAnimEventT.AirtimeEnded:
                CpMgr.GetAos(id).isAffectedByGravity = true;
                CpMgr.GetAos(id).vel_Ver = -CpMgr.GetAos(id).act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                CpMgr.GetAos(id).isAffectedByGravity = false;
                break;
            case CpAnimEventT.YawAllowed:
                CpMgr.GetAos(id).inputRotAllowed = true;
                break;
            case CpAnimEventT.YawDisallowed:
                CpMgr.GetAos(id).inputRotAllowed = false;
                break;
            default:
                Debug.Log($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
