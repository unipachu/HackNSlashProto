using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// SoA storage for playable capsule character runtime data.
/// </summary>
public class EntityData : Singleton<EntityData> {
    [SerializeField] int capacity = 2;

    public NativeArray<CapsuleCharacterData> configs;
    // Structs can't be null, so we need a way to keep track of structs that are actually used.
    private NativeArray<bool> occupied;
    
    // structs can't be null, so track occupancy explicitly
    // Tracks which GameObject owns which slot, so a caller can't register twice
    // and so we know a given id actually belongs to this EntityData.
    private Dictionary<GameObject, int> ownerToId = new Dictionary<GameObject, int>();

    protected override void Awake() {
        base.Awake();
        configs = new NativeArray<CapsuleCharacterData>(capacity, Allocator.Persistent);
        occupied = new NativeArray<bool>(capacity, Allocator.Persistent);
    }

    void OnDestroy() {
        if (configs.IsCreated) configs.Dispose();
        if (occupied.IsCreated) occupied.Dispose();
    }

    // ------------------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------------------

    public CapsuleCharacterData GetData(int id) => configs[id];

    public void SetData(int id, CapsuleCharacterData value) => configs[id] = value;

    /// <summary>
    /// Registers data mapped to a game object.<br/>
    /// Returns the index of the game object data, or -1 on failure.
    /// </summary>
    public int Register(GameObject owner, SO_CapsuleCharacterData configSo) {
        if (ownerToId.ContainsKey(owner)) {
            Debug.LogError($"EntityData: {owner.name} is already registered "
                + $"(id {ownerToId[owner]}).", owner);
            return -1;
        }
        int freeIndex = -1;
        for (int i = 0; i < occupied.Length; i++) {
            if (!occupied[i]) {
                freeIndex = i;
                break;
            }
        }
        if (freeIndex == -1) {
            Debug.LogError($"EntityData: at capacity ({capacity}), "
                + $"cannot register {owner.name}.", owner);
            return -1;
        }
        configs[freeIndex] = configSo.ToStruct();
        occupied[freeIndex] = true;
        ownerToId.Add(owner, freeIndex);
        return freeIndex;
    }

    /// <summary>
    /// Returns true and outputs the id if owner is currently registered.
    /// </summary>
    public bool TryGetId(GameObject owner, out int id) => ownerToId.TryGetValue(owner, out id);

    public void Unregister(GameObject owner) {
        if (!ownerToId.TryGetValue(owner, out int id)) {
            Debug.LogError($"EntityData: {owner.name} has not been registered!", owner);
            return;
        }
        occupied[id] = false;
        ownerToId.Remove(owner);
    }
}