using Mono.Cecil;
using UnityEngine;

/// <summary>
/// Data passed from the hitter to the hit object.
/// </summary>
// TODO: Consider also "HitRecieverData" passed to the hitter from the hit reciever during hit.
public struct NewHitData
{
    public Transform Attacker;
    //public ActionDefinition SourceAction;
    public int Damage;
    public float KnockBack;
    //public Vector3 HitPoint;
    // TODO: Summary. This should likely be the negative of ComputePenetration direction.
    public Vector3 HitDir;

    public NewHitData(Transform attacker,
        int damage,
        float knockBack,
        Vector3 hitDir)
    {
        Attacker = attacker;
        //SourceAction = sourceAction;
        Damage = damage;
        KnockBack = knockBack;
        //HitPoint = hitPoint;
        HitDir = hitDir;
    }
}
