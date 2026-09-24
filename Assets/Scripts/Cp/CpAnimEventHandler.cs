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

    void OnAnimEvent(int cpI, CpAnimEventT animEvent) {
        //Debug.Log($"Anim event {animEvent} for {id} called!", this);
        var classRefs = CpMgr.inst.aos[cpI].classRefs;
        switch (animEvent) {
            case CpAnimEventT.BufferedInputStSwitchAllowed:
                CpMgr.GetData(cpI).bufferedInputStSwitchAllowed = true;
                break;
            case CpAnimEventT.ComboAllowed:
                CpMgr.GetData(cpI).comboAllowed = true;
                break;
            case CpAnimEventT.ComboDisallowed:
                CpMgr.GetData(cpI).comboAllowed = false;
                break;
            case CpAnimEventT.DodgeAllowed:
                CpMgr.GetData(cpI).dodgeAllowed = true;
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
            case CpAnimEventT.InputMovAllowed:
                CpMgr.GetData(cpI).inputMovAllowed = true;
                break;
            case CpAnimEventT.InvulEnd:
                CpMgr.GetData(cpI).ignoreHits = false;
                break;
            case CpAnimEventT.AirtimeEnded:
                CpMgr.GetData(cpI).isAffectedByGravity = true;
                CpMgr.GetData(cpI).vel_Ver = -CpMgr.GetData(cpI).act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                CpMgr.GetData(cpI).isAffectedByGravity = false;
                break;
            case CpAnimEventT.YawAllowed:
                CpMgr.GetData(cpI).yawAllowed = true;
                break;
            case CpAnimEventT.YawDisallowed:
                CpMgr.GetData(cpI).yawAllowed = false;
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
