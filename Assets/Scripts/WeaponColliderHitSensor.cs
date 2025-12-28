using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to check if attack collider connects during attacks.
/// </summary>
public class WeaponColliderHitSensor : MonoBehaviour
{
    // TODO: Figure out how to pass dmg info to this.
    [SerializeField] private int weaponDamage = 1;
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

    public void CheckHits()
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

            // TODO: Use the direction from the player to the enemy instead.
            Vector3 collisionDirection = Vector3.zero;

            if (Physics.ComputePenetration(
                weaponCollider,
                weaponCollider.transform.position,
                weaponCollider.transform.rotation,
                otherCol,
                otherCol.transform.position,
                otherCol.transform.rotation,
                out collisionDirection,
                out _
            ))
            {
                // NOTE: We use negation of the collision direction here to get the direction of the attack.
                damageable.GetHit(weaponDamage, -collisionDirection);
                alreadyHit.Add(damageable);
            }
        }
    }
}
