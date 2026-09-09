using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : MonoBehaviour, IHandItem_Comboer, IHandItem_Hitter {
    [SerializeField] HitEffects hitEffects = new HitEffects(1, KnockbackT.Weak, 1);
    [SerializeField] HitDealer hitDealer;
    [SerializeField] So_ComboGraph comboGraphConfig;

    List<IComboNode> comboGraph;

    public HitEffects HitEffects => hitEffects;
    public HitDealer HitDealer => hitDealer;
    public IComboNode LShldrComboStart => null;
    public IComboNode RShldrComboStart => comboGraph[0];
    public IComboNode RTrgComboStart => null;
    public Transform Trf => transform;

    void Awake() {
        comboGraph = comboGraphConfig.GenerateComboGraph(this);
    }
}
