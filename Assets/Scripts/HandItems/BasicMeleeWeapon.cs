using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : MonoBehaviour, IHandItem_Comboer, IHandItem_Hitter {
    [SerializeField] So_ComboGraph comboGraphConfig;
    [SerializeField] HitDealer_CapsuleSubstepper hitDealer;
    [SerializeField] HitEffects hitEffects = new HitEffects(1, KnockbackT.Weak, 1);

    List<IComboNode> comboGraph;

    public IHitDealer HitDealer => hitDealer;
    public HitEffects HitEffects => hitEffects;
    public IComboNode LShldrComboStart => null;
    public IComboNode RShldrComboStart => comboGraph[0];
    public IComboNode RTrgComboStart => null;
    public Transform Trf => transform;

    void Awake() {
        comboGraph = ComboGraphFactory.GenerateComboGraph(this, comboGraphConfig.nodes);
    }
}
