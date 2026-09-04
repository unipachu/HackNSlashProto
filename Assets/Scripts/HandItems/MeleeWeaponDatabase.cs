using System.Collections.Generic;

/// <summary>
/// Instantiated melee weapons.
/// </summary>
public class MeleeWeaponDatabase : Singleton<MeleeWeaponDatabase>, IHandItemDatabase{
    public List<MeleeWeaponData> meleeWeaponData;
    public List<MeleeWeaponCompRefs> meleeWeaponCompRefs;

    public List<HandItemData> HandItemData { get; set; }

    protected override void Awake() {
        base.Awake();
        meleeWeaponData = new List<MeleeWeaponData>();
        HandItemData = new List<HandItemData>();
    }

    /// <summary>
    /// Returns the index of the registered data.
    /// </summary>
    public int Register(
        HandItemData handItemData,
        MeleeWeaponData meleeWeaponData,
        MeleeWeaponCompRefs meleeWeaponCompRefs
    ) {
        HandItemData.Add(handItemData);
        this.meleeWeaponData.Add(meleeWeaponData);
        this.meleeWeaponCompRefs.Add(meleeWeaponCompRefs);
        // If this previously had no hand item data, add it to hand item registry.
        if(HandItemData.Count == 1)
            HandItemReg.inst.handItemDatabases.Add(this);
        return HandItemData.Count - 1;
    }

    public void Unregister(int id) {
        int lastId = HandItemData.Count - 1;
        if (id != lastId) {
            // Move last entry to removed slot.
            HandItemData[id] = HandItemData[lastId];
            meleeWeaponData[id] = meleeWeaponData[lastId];
            meleeWeaponCompRefs[id] = meleeWeaponCompRefs[lastId];
            // Change id in handle.
            meleeWeaponCompRefs[id].meleeWeaponHandle.Id = id;
        }
        // Remove the (duplicate) last items.
        HandItemData.RemoveAt(lastId);
        meleeWeaponData.RemoveAt(lastId);
        meleeWeaponCompRefs.RemoveAt(lastId);
        // If this contains no hand item data, remove it from hand item registry.
        if (HandItemData.Count == 0)
            HandItemReg.inst.handItemDatabases.Remove(this);
    }
}
