using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    [SerializeField] Transform _attacker;
    [SerializeField] Transform _rightHandGrabPoint;
    [SerializeField] WeaponController _rightHandWeapon;
    // TODO: Should you save which input activates which action?


    public void AttackRight(Transform attacker)
    {
        _rightHandWeapon.ActivateAttack(attacker);
    }

    public void ReadyAttackRight()
    {
        _rightHandWeapon.BeginAttack();
    }

}
