using UnityEngine;

public static class CpFlyingHeadUtils{
    public static void OnAnimEvent(CpFlyingHeadHandle cpFlyingHead, CpAnimEventT animEvent) {
        //Debug.Log($"Anim event {animEvent} for {id} called!", this);
        ref var commonData = ref cpFlyingHead.CommonData;
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
                Debug.LogError($"Switch defaulted with {animEvent}.", cpFlyingHead);
                break;
        }
    }
}
