using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WldHpBar : MonoBehaviour {
    [SerializeField] Image imgDmgYellow;
    [SerializeField] Image imgDmgRed;
    [SerializeField] TMP_Text textName;
    [SerializeField] TMP_Text textDmg;

    public int I { get; set; } = -1;

    public ref WldHpBarData GetData()
        => ref WldHpBarMgr.inst.bars[I];

    public void SetName(string name) {
        bool hasName = !string.IsNullOrEmpty(name);
        textName.gameObject.SetActive(hasName);
        if (hasName)
            textName.text = name;
    }

    public void SetHp(int curHp, int maxHp) {
        //Debug.Log($"Set hp bar curhp: {curHp}, maxhp: {maxHp}");
        float nrmHp = maxHp > 0
            ? Mathf.Clamp01((float)curHp / maxHp)
            : 0;
        imgDmgRed.fillAmount = nrmHp;
    }

    // TODO: Make this trail after the red bar
    public void SetYellowHp(int hp, int maxHp) {
        float nrmHp = maxHp > 0
            ? Mathf.Clamp01((float)hp / maxHp)
            : 0;
        imgDmgYellow.fillAmount = nrmHp;
    }

    public void SetDmgText(int damage) {
        bool show = damage > 0;
        textDmg.gameObject.SetActive(show);
        if (show)
            textDmg.text = damage.ToString();
    }

    public void OnCpMarkedForUnregister() {
        WldHpBarMgr.inst.bars[I].pendingUnregister = true;
    }

    public void OnCurHpChanged(int newCurHp, int maxHp) {
        SetHp(newCurHp, maxHp);
        GetData().barVisibleUntil = Time.time + WldHpBarMgr.inst.barVisibleDur;
    }

    public void OnDmgTaken(int dmgTaken) {
        ref var data = ref GetData();
        var now = Time.time;
        SetDmgText(dmgTaken + data.accumulatedDmg);
        data.accumulatedDmg = data.accumulatedDmg + dmgTaken;
        data.barVisibleUntil = now + WldHpBarMgr.inst.barVisibleDur;
        data.dmgNumberVisibleUntil = now + PlrMgr.inst.successiveAtkWindow;
    }

    public void OnMaxHpChanged(int curHp, int newMaxHp) {
        SetHp(curHp, newMaxHp);
        GetData().barVisibleUntil = Time.time + WldHpBarMgr.inst.barVisibleDur;
    }

    public void OnPlrLockedOnEnded() {
        ref var data = ref GetData();
        data.isLocked = false;
        data.barVisibleUntil = Time.time + WldHpBarMgr.inst.barVisibleDur;
    }

    public void OnPlrLockedOnStarted() {
        ref var data = ref GetData();
        data.isLocked = true;
        data.barVisibleUntil = float.PositiveInfinity;
    }
}