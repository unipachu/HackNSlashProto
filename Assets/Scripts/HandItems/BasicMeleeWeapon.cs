using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : MonoBehaviour, IHandItem_Comboer, IHandItem_Hitter {
    public event Action<HashSet<HitResult>> hitSomething;

    [SerializeField] ComboGraphT comboGraphT;
    [SerializeField] HitDealer_CapsuleSubstepper hitDealer;
    [SerializeField] string hitEffectsConfigId = "";

    List<IComboNode_CpHumanoid> comboGraph;

    public IHitDealer HitDealer => hitDealer;
    public HitEffects HitEffects
        => new(
            Db.GetHitEffectsConfig(hitEffectsConfigId).dmg,
            Db.GetHitEffectsConfig(hitEffectsConfigId).hitT,
            Db.GetHitEffectsConfig(hitEffectsConfigId).knockbackT,
            Db.GetHitEffectsConfig(hitEffectsConfigId).knockbackStr
        );
    public IComboNode_CpHumanoid LShldrComboStart => null;
    public IComboNode_CpHumanoid RShldrComboStart => comboGraph[0];
    public IComboNode_CpHumanoid RTrgComboStart => null;
    public Transform Trf => transform;


    void Awake() {
        comboGraph = ComboGraphFactory.GenerateComboGraph(this, comboGraphT);
    }

    void OnEnable() {
        hitDealer.hitSomething += HitSomething;
    }

    void OnDisable() {
        hitDealer.hitSomething -= HitSomething;
    }

    void HitSomething(HashSet<HitResult> hits) {
        hitSomething?.Invoke(hits);
    }
}
