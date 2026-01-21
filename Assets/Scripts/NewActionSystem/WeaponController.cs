using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] int baseDmg = 1;
    [SerializeField] int baseKnockback = 1;
    //[SerializeField] WeaponHitbox _hitBox;

    //public WeaponHitbox HitBox => _hitBox;

    [SerializeField]WeaponColliderHitSensor _hitSensor;

    public void Attack(Transform attacker)
    {
        _hitSensor.CheckHits(attacker, baseDmg, baseKnockback);
    }

    public void BeginAttack()
    {
        _hitSensor.BeginAttack();
    }
}
