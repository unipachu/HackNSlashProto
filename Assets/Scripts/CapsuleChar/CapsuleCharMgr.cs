using Unity.Collections;
using UnityEngine;

/// <summary>
/// Storage for playable capsule character runtime data.
/// </summary>
public class CapsuleCharMgr : Singleton<CapsuleCharMgr> {
    public int maxCapsuleChars = 2;

    public NativeArray<CapsuleCharData> capsuleCharDatas;
    // Structs can't be null, so we need a way to keep track of structs that are actually used.
    public NativeArray<bool> occupied;
 
    public void Init() {
        if (capsuleCharDatas.IsCreated) {
            Debug.LogError("Data already created before Init!", this);
            return;
        }
        capsuleCharDatas = new NativeArray<CapsuleCharData>(maxCapsuleChars, Allocator.Persistent);
        occupied = new NativeArray<bool>(maxCapsuleChars, Allocator.Persistent);
    }

    void OnDestroy() {
        capsuleCharDatas.Dispose();
        occupied.Dispose();
    }

    // ------------------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------------------

    public CapsuleCharData GetData(int id) {
        //Debug.Log($"GET id={id}, invul={configs[id].invul}");
        return capsuleCharDatas[id];
    }

    public void SetData(int id, CapsuleCharData value) {
        //Debug.Log($"SET id={id}, invul={value.invul}");
        capsuleCharDatas[id] = value;
    }

    /// <summary>
    /// Returns the index of the registered data, or -1 on failure.
    /// </summary>
    public int Register(So_CapsuleCharData so_capsuleCharData, So_BtRootNode bt) {
        int freeIndex = -1;
        for (int i = 0; i < occupied.Length; i++) {
            if (!occupied[i]) {
                freeIndex = i;
                break;
            }
        }
        if (freeIndex == -1) {
            Debug.LogError($"EntityData: at capacity ({maxCapsuleChars})");
            return -1;
        }
        capsuleCharDatas[freeIndex] = so_capsuleCharData.ToStruct();
        if (bt != null) {
            BtMgr.inst.Register(freeIndex, bt);
        }
        occupied[freeIndex] = true;
        return freeIndex;
    }

    public void Unregister(int id) {
        if (!occupied[id]) {
            Debug.LogError($"Capsule character with id {id} has not been registered!");
            return;
        }
        occupied[id] = false;
    }
}