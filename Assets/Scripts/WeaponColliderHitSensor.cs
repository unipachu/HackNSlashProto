using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to check if attack collider connects during attacks.
/// </summary>
public class WeaponColliderHitSensor : MonoBehaviour
{
    [Tooltip("Currently does not ignore collisons, unless set here")]
    [SerializeField] private LayerMask hitMask;

    [Header("Refs")]
    [SerializeField] private Collider weaponCollider;

    /// <summary>
    /// Enemies that have already been hit during the current attack.
    /// </summary>
    private readonly HashSet<IHittable> alreadyHit = new();

    // TODO: Make a better and more foolproof system for starting and ending active attack frames.
    public void BeginAttack()
    {
        alreadyHit.Clear();
    }

    public void CheckHits(int damage, Transform attacker)
    {
        Collider[] candidates = Physics.OverlapSphere(
            weaponCollider.bounds.center,
            weaponCollider.bounds.extents.magnitude,
            hitMask,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider otherCol in candidates)
        {
            IHittable damageable = otherCol.GetComponentInParent<IHittable>();
            if (damageable == null) continue;

            if (alreadyHit.Contains(damageable)) continue;

            if (Physics.ComputePenetration(
                weaponCollider,
                weaponCollider.transform.position,
                weaponCollider.transform.rotation,
                otherCol,
                otherCol.transform.position,
                otherCol.transform.rotation,
                out _,
                out _
            ))
            {
                // NOTE: We use negation of the collision direction here to get the direction of the attack.
                damageable.GetHit(damage, attacker.position);
                alreadyHit.Add(damageable);
            }
        }
    }
}
