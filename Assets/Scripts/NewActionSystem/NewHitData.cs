using UnityEngine;

/// <summary>
/// Data passed from the hitter to the hit object.
/// </summary>
public struct NewHitData
{
    public GameObject attacker;
    public ActionDefinition sourceAction;

    public float damage;
    public float knockback;
    public Vector3 hitPoint;
    // TODO: Summary. This should likely be the negative of ComputePenetration direction.
    public Vector3 hitDirection;
}
