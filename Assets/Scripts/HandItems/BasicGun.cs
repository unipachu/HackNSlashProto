using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : MonoBehaviour, IHandItem_Hitter, IHandItem_ProjectileSpawner, IHandItem_Comboer {
    public event Action<HashSet<HitResult>> hitSomething;

    [SerializeField] AimLaser aimLaser;
    [SerializeField] ComboGraphT comboGraphT;
    [SerializeField] HitDealer_CapsuleSubstepper meleeHitDealer;
    // TODO AFTER RELEASE: If you decide to start making an action rpg, you could add hit effects to combo graphs
    // C: of a weapon (and call them "base hit effects" or similar") and then to those add values from the
    // C: weapon instance stats (like added dmg, affinities etc) and finally add values/effects from the
    // C: player's stats, like bonuses from player level. Or do those in the opposite order.
    [SerializeField] HitEffects meleeHitEffects = new HitEffects(1, HitT.Blunt, KnockbackT.Weak, 1);
    [SerializeField] HitEffects projHitEffects = new HitEffects(1, HitT.Blunt, KnockbackT.Weak, 1);
    [SerializeField] HomingProjData homingProjData = new(10, 4, 0.5f);
    [SerializeField] Transform projSpawnPose;
    [SerializeField] ProjT projT;

    List<IComboNode> comboGraph;

    public AimLaser AimLaser => aimLaser;
    public IHitDealer HitDealer => meleeHitDealer;
    public HitEffects HitEffects => meleeHitEffects;
    public HomingProjData HomingProjData => homingProjData;
    public IComboNode LShldrComboStart => null;
    public IComboNode RShldrComboStart => comboGraph[0];
    public IComboNode RTrgComboStart => null;
    public HitEffects ProjHitEffects => projHitEffects;
    public Transform ProjSpawnPoseTrf => projSpawnPose;
    public ProjT ProjT => projT;
    public Transform Trf => transform;


    void Awake() {
        comboGraph = ComboGraphFactory.GenerateComboGraph(this, comboGraphT);
    }

    void OnEnable() {
        meleeHitDealer.hitSomething += hitSomething;
    }

    void OnDisable() {
        meleeHitDealer.hitSomething -= hitSomething;
    }
}
