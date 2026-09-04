using UnityEngine;

[CreateAssetMenu(
    fileName = "MeleeWeapon",
    menuName = "Hand Item/Melee Weapon"
)]
public class MeleeWeaponSO : So_HandItem {
    [SerializeField] MeleeWeaponHandle prefab;
    [SerializeField] HandItemData handItemData;
    [SerializeField] MeleeWeaponData meleeWeaponData;

    public override IHandItemHandle InstantiateNRegister() {
        MeleeWeaponHandle handle = Instantiate(prefab);
        handle.InitNRegister(handItemData, meleeWeaponData);
        return handle;
    }
}