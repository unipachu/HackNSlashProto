using UnityEngine;
using UnityEngine.UI;

public class HudMgr : Singleton<HudMgr> {
    public Image imgHp;
    public Image imgHpYellow;
    public float yellowBarSpd = 1;
    public float yellowWaitUntilTrail = 0.5f;
    public Image imgUlt;
    
    [HideInInspector] public HpBarTrailData hpTrail;

    public void ResetYellowTrail() {
        UiUtils.ResetYellowTrail(imgHp, imgHpYellow, ref hpTrail);
    }

    public void SetHp(int curHp, int maxHp) {
        UiUtils.SetBarFill(imgHp, curHp, maxHp);
    }

    public void SetUlt(int curUlt, int maxUlt) {
        UiUtils.SetBarFill(imgUlt, curUlt, maxUlt);
    }

    public void Tick() {
        UiUtils.UpdateYellowTrail(
                imgHp,
                imgHpYellow,
                ref hpTrail,
                Time.time,
                yellowBarSpd,
                yellowWaitUntilTrail
            );
    }
}