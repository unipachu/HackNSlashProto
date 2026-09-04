using UnityEngine;

/// <summary>
/// Way to reference a particular melee weapon handle.
/// </summary>
public class MeleeWeaponHandle : MonoBehaviour, IHandItemHandle{
    [SerializeField] MeleeWeaponCompRefs compRefs;

    public int Id { get; set; } = -1;
    public HandItemData HandItemData => MeleeWeaponDatabase.inst.HandItemData[Id];

    public HandItemDataT HandItemDataT => HandItemDataT.MeleeWeapon;

    void OnDestroy() {
        if (Id != -1) {
            // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
            // NOTE C: scene is being changed.
            if (MeleeWeaponDatabase.inst != null)
                MeleeWeaponDatabase.inst.Unregister(Id);
            Id = -1;
        }
    }

    /// <summary>
    /// This should be called right after the game object has been instantiated!
    /// </summary>
    public void InitNRegister(HandItemData handItemData, MeleeWeaponData meleeWeaponData) {
        Debug.Assert(
            MeleeWeaponDatabase.inst != null,
            $"{typeof(MeleeWeaponDatabase).Name} inst was null!",
            this
        );
        Id = MeleeWeaponDatabase.inst.Register(handItemData, meleeWeaponData, compRefs);
    }
}
