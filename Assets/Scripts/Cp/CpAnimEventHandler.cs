using System;
using UnityEngine;

// TODO: You could make this into a singleton (as well as the other cp anim event handler) but for some
// C: reason you have this on every prefab.
// TODO: Rename to CpHumanoidAnimEventHandler
public class CpAnimEventHandler : MonoBehaviour {
    public Action<CpHandle, CpAnimEventT> animEvent;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    void OnAnimEvent(CpHandle cpHumd, CpAnimEventT animEvent) {
        //Debug.Log($"Anim event {animEvent} for {id} called!", this);
        ref var commonData = ref cpHumd.CommonData;
        ref var humdData = ref cpHumd.HumdData;
        switch (animEvent) {
            case CpAnimEventT.BufferedInputStSwitchAllowed:
                commonData.bufferedInputStSwitchAllowed = true;
                break;
            case CpAnimEventT.ComboAllowed:
                commonData.comboAllowed = true;
                break;
            case CpAnimEventT.ComboDisallowed:
                commonData.comboAllowed = false;
                break;
            case CpAnimEventT.DodgeAllowed:
                humdData.dodgeAllowed = true;
                break;
            case CpAnimEventT.Finished:
                commonData.classRefs.st_cur.HandleAnimEvent(CpAnimEventT.Finished);
                break;
            case CpAnimEventT.HitDealerActivated:
                commonData.classRefs.st_cur.HandleAnimEvent(CpAnimEventT.HitDealerActivated);
                break;
            case CpAnimEventT.HitDealerDeactivated:
                commonData.classRefs.st_cur.HandleAnimEvent(CpAnimEventT.HitDealerDeactivated);
                break;
            case CpAnimEventT.InputMovAllowed:
                commonData.inputMovAllowed = true;
                break;
            case CpAnimEventT.InvulEnd:
                commonData.ignoreHits = false;
                break;
            case CpAnimEventT.AirtimeEnded:
                commonData.isAffectedByGravity = true;
                commonData.vel_Ver = -cpHumd.so_cpHumdConfig.act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                commonData.isAffectedByGravity = false;
                break;
            case CpAnimEventT.YawAllowed:
                commonData.yawAllowed = true;
                break;
            case CpAnimEventT.YawDisallowed:
                commonData.yawAllowed = false;
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}.", this);
                break;
        }
    }
}
