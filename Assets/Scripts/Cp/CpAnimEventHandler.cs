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
                CpMgr.GetData(id).bufferedInputStSwitchAllowed = true;
                break;
            case CpAnimEventT.ComboAllowed:
                CpMgr.GetData(id).comboAllowed = true;
                break;
            case CpAnimEventT.ComboDisallowed:
                CpMgr.GetData(id).comboAllowed = false;
                break;
            case CpAnimEventT.DodgeAllowed:
                CpMgr.GetData(id).dodgeAllowed = true;
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
                CpMgr.GetData(id).invul = false;
                break;
            case CpAnimEventT.AirtimeEnded:
                CpMgr.GetData(id).isAffectedByGravity = true;
                CpMgr.GetData(id).vel_Ver = -CpMgr.GetData(id).act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                CpMgr.GetData(id).isAffectedByGravity = false;
                break;
            case CpAnimEventT.YawAllowed:
                CpMgr.GetData(id).inputRotAllowed = true;
                break;
            case CpAnimEventT.YawDisallowed:
                CpMgr.GetData(id).inputRotAllowed = false;
                break;
            default:
                Debug.Log($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
