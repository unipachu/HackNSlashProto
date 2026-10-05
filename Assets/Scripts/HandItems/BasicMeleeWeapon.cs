using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : MonoBehaviour, IHandItem_Comboer, IHandItem_Hitter {
    public event Action<HashSet<HitResult>> hitSomething;

    [SerializeField] ComboGraphT comboGraphT;
    [SerializeField] HitDealer_CapsuleSubstepper hitDealer;
    [SerializeField] HitEffects hitEffects = new HitEffects(1, HitT.Blunt, KnockbackT.Weak, 1);

    List<IComboNode> comboGraph;

    public IHitDealer HitDealer => hitDealer;
    public HitEffects HitEffects => hitEffects;
    public IComboNode LShldrComboStart => null;
    public IComboNode RShldrComboStart => comboGraph[0];
    public IComboNode RTrgComboStart => null;
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
