using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] int baseDmg;
    [SerializeField] int baseKnockback;
    //[SerializeField] WeaponHitbox _hitBox;

    //public WeaponHitbox HitBox => _hitBox;

    WeaponColliderHitSensor _hitSensor;

    public void Attack(Transform attacker)
    {
        _hitSensor.CheckHits(attacker, baseDmg, baseKnockback);
    }

    public void BeginAttack()
    {
        _hitSensor.BeginAttack();
    }
}
