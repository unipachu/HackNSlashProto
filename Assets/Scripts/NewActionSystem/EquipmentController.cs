using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    [SerializeField] Transform _attacker;
    [SerializeField] Transform _rightHandGrabPoint;
    [SerializeField] WeaponController _rightHandWeapon;
    // TODO: Should you save which input activates which action?


    public void AttackRight(Transform attacker)
    {
        _rightHandWeapon.Attack(attacker);
    }

    public void ReadyAttackRight()
    {
        _rightHandWeapon.BeginAttack();
    }

}
