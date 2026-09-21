using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WldHpBar : MonoBehaviour {
    [SerializeField] Image imgDmgYellow;
    [SerializeField] Image imgDmgRed;
    [SerializeField] TMP_Text textName;
    [SerializeField] TMP_Text textDmg;

    public void SetName(string name) {
        bool hasName = !string.IsNullOrEmpty(name);
        textName.gameObject.SetActive(hasName);
        if (hasName)
            textName.text = name;
    }

    public void SetHp(float curHp, float maxHp) {
        float nrmHp = maxHp > 0f
            ? Mathf.Clamp01(curHp / maxHp)
            : 0f;
        imgDmgRed.fillAmount = nrmHp;
    }

    public void SetYellowHp(float hp, float maxHp) {
        float nrmHp = maxHp > 0f
            ? Mathf.Clamp01(hp / maxHp)
            : 0f;
        imgDmgYellow.fillAmount = nrmHp;
    }

    public void SetDamageText(float damage) {
        bool show = damage > 0f;
        textDmg.gameObject.SetActive(show);
        if (show)
            textDmg.text = Mathf.RoundToInt(damage).ToString();
    }
}