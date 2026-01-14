using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents the hitbox and stats of the weapon.
/// </summary>
public class WeaponHitbox : MonoBehaviour
{
    public bool GizmosEnabled = true;
    public ActionController ActionController;
    public Collider HitCollider;
    HashSet<Collider> HitTargets = new();
    private float CurrentDamage;
    private float CurrentKnockback;

    void Awake()
    {
        HitCollider.enabled = false;
    }

    public void Activate()
    {
        HitTargets.Clear();
        HitCollider.enabled = true;
    }

    public void Deactivate()
    {
        HitCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (HitTargets.Contains(other))
            return;

        HitTargets.Add(other);

        if (other.TryGetComponent(out NewHurtbox hurtbox))
        {
            var hit = BuildHitData(hurtbox);
            hurtbox.ReceiveHit(hit);
        }
    }

    NewHitData BuildHitData(NewHurtbox hurtbox)
    {
        return new NewHitData
        {
            attacker = gameObject,
            sourceAction = ActionController.CurrentAction,
            damage = CurrentDamage,
            knockback = CurrentKnockback,
            hitPoint = hurtbox.transform.position,
            hitDirection = (hurtbox.transform.position - transform.position).normalized
        };
    }

    void OnDrawGizmos()
    {
        if (!GizmosEnabled || HitCollider == null)
            return;

        Gizmos.color = HitCollider.enabled ? Color.red : Color.gray;
        Gizmos.matrix = transform.localToWorldMatrix;

        // NOTE: Only works with BoxCollider.
        if (HitCollider is BoxCollider box)
            Gizmos.DrawWireCube(box.center, box.size);
    }
}