using UnityEngine;

public class GunHandle : MonoBehaviour, IHandItemHandle {
    [SerializeField] GunCompRefs compRefs;

    public int Id { get; set; } = -1;
    public HandItemData HandItemData => GunDatabase.inst.HandItemData[Id];

    public HandItemDataT HandItemDataT => HandItemDataT.Gun;

    void OnDestroy() {
        if (Id != -1) {
            // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
            // NOTE C: scene is being changed.
            if (GunDatabase.inst != null)
                GunDatabase.inst.Unregister(Id);
            Id = -1;
        }
    }

    /// <summary>
    /// This should be called right after the game object has been instantiated!
    /// </summary>
    public void InitNRegister(HandItemData handItemData, GunData gunData) {
        Debug.Assert(
            GunDatabase.inst != null,
            $"{typeof(GunDatabase).Name} inst was null!",
            this
        );
        Id = GunDatabase.inst.Register(handItemData, gunData, compRefs);
    }
}
