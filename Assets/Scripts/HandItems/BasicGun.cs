using System.Collections.Generic;
using UnityEngine;

public class BasicGun : MonoBehaviour, IHandItem_Hitter, IHandItem_ProjectileSpawner, IHandItem_Comboer {
    [SerializeField] So_ComboGraph comboGraphConfig;
    [SerializeField] HitDealer meleeHitDealer;
    [SerializeField] HitEffects hitEffects = new HitEffects(1, KnockbackT.Weak, 1);
    [SerializeField] HomingProjData homingProjData = new(10, 4, 0.5f);
    [SerializeField] Transform projSpawnPose;

    List<IComboNode> comboGraph;

    public HitDealer HitDealer => meleeHitDealer;
    public HitEffects HitEffects => hitEffects;
    public HomingProjData HomingProjData => homingProjData;
    public IComboNode LShldrComboStart => null;
    public IComboNode RShldrComboStart => comboGraph[0];
    public IComboNode RTrgComboStart => null;
    public Transform ProjSpawnPoseTrf => projSpawnPose;
    public Transform Trf => transform;

    void Awake() {
        comboGraph = comboGraphConfig.GenerateComboGraph(this);
    }
}
