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
        Cp_SoaData data = CpMgr.inst.soaData;
        switch (animEvent) {
            case CpAnimEventT.BufferedInputStSwitchAllowed:
                data.actStSt_BufferedInputStSwitchAllowed[id] = true;
                break;
            case CpAnimEventT.ComboAllowed:
                data.actStSt_ComboAllowed[id] = true;
                break;
            case CpAnimEventT.ComboDisallowed:
                data.actStSt_ComboAllowed[id] = false;
                break;
            case CpAnimEventT.DodgeAllowed:
                data.actStSt_DodgeAllowed[id] = true;
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
                data.invul[id] = false;
                break;
            case CpAnimEventT.AirtimeEnded:
                data.isAffectedByGravity[id] = true;
                data.vel_Ver[id] = -data.st_AtkJump_DownSpeedAfterJumpFinished[id];
                break;
            case CpAnimEventT.AirtimeStarted:
                data.isAffectedByGravity[id] = false;
                break;
            case CpAnimEventT.YawAllowed:
                data.actStSt_InputRotAllowed[id] = true;
                break;
            case CpAnimEventT.YawDisallowed:
                data.actStSt_InputRotAllowed[id] = false;
                break;
            default:
                Debug.Log($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
