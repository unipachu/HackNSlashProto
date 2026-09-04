using System.Collections.Generic;
using UnityEngine;

public class GunDatabase : Singleton<GunDatabase>, IHandItemDatabase {
    public List<GunData> gunData;
    public List<GunCompRefs> gunCompRefs;

    public List<HandItemData> HandItemData { get; set; }

    protected override void Awake() {
        base.Awake();
        gunData = new List<GunData>();
        HandItemData = new List<HandItemData>();
    }

    /// <summary>
    /// Returns the index of the registered data.
    /// </summary>
    public int Register(
        HandItemData handItemData,
        GunData gunData,
        GunCompRefs gunCompRefs
    ) {
        HandItemData.Add(handItemData);
        this.gunData.Add(gunData);
        this.gunCompRefs.Add(gunCompRefs);
        // If this previously had no hand item data, add it to hand item registry.
        if (HandItemData.Count == 1)
            HandItemReg.inst.handItemDatabases.Add(this);
        return HandItemData.Count - 1;
    }

    public void Unregister(int id) {
        int lastId = HandItemData.Count - 1;
        if (id != lastId) {
            // Move last entry to removed slot.
            HandItemData[id] = HandItemData[lastId];
            gunData[id] = gunData[lastId];
            gunCompRefs[id] = gunCompRefs[lastId];
            // Change id in handle.
            gunCompRefs[id].gunHandle.Id = id;
        }
        // Remove the (duplicate) last items.
        HandItemData.RemoveAt(lastId);
        gunData.RemoveAt(lastId);
        gunCompRefs.RemoveAt(lastId);
        // If this contains no hand item data, remove it from hand item registry.
        if (HandItemData.Count == 0)
            HandItemReg.inst.handItemDatabases.Remove(this);
    }
}
