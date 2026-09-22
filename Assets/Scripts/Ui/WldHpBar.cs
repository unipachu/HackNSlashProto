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
        Debug.Log($"Set hp bar: {curHp} {maxHp}");
        float nrmHp = maxHp > 0f
            ? Mathf.Clamp01((float)curHp / maxHp)
            : 0f;
        imgDmgRed.fillAmount = nrmHp;
    }

    // TODO: Make this trail after the red bar
    public void SetYellowHp(int hp, int maxHp) {
        float nrmHp = maxHp > 0f
            ? Mathf.Clamp01((float)hp / maxHp)
            : 0f;
        imgDmgYellow.fillAmount = nrmHp;
    }

    public void SetDmgText(int damage) {
        bool show = damage > 0f;
        textDmg.gameObject.SetActive(show);
        if (show)
            textDmg.text = damage.ToString();
    }

    public void OnCpMarkedForUnregister() {
        WldHpBarMgr.inst.bars[I].pendingUnregister = true;
    }

    public void OnCurHpChanged(int newCurHp, int maxHp) {
        SetHp(newCurHp, maxHp);
        GetData().visibleUntil = Time.time + WldHpBarMgr.inst.visibleAfterDamageTime;
    }

    public void OnDmgTaken(int dmgTaken) {
        Debug.Log("Went here");
        SetDmgText(dmgTaken);
        GetData().visibleUntil = Time.time + WldHpBarMgr.inst.visibleAfterDamageTime;
    }

    public void OnMaxHpChanged(int curHp, int newMaxHp) {
        SetHp(curHp, newMaxHp);
        GetData().visibleUntil = Time.time + WldHpBarMgr.inst.visibleAfterDamageTime;
    }

    public void OnPlrLockedOnEnded() {
        ref var data = ref GetData();
        data.isLocked = false;
        data.visibleUntil = Time.time + WldHpBarMgr.inst.visibleAfterDamageTime;
    }

    public void OnPlrLockedOnStarted() {
        ref var data = ref GetData();
        data.isLocked = true;
        data.visibleUntil = float.PositiveInfinity;
    }
}