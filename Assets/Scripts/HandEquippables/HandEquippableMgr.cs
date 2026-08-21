using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class HandEquippableMgr : Singleton<HandEquippableMgr>{
    [SerializeField] List<So_HandEquippable> handEquippableSos;

    [HideInInspector] public HandEquippable[] prefab;
    public NativeArray<CapsuleCharActSt> lightAtkActId;
    public NativeArray<CapsuleCharActSt> heavyAtkActId;
    public NativeArray<CapsuleCharActSt> ultAtkActId;

    public void Init() {
        if (
            lightAtkActId.IsCreated
            || heavyAtkActId.IsCreated
            || ultAtkActId.IsCreated
        ) {
            Debug.LogError("Data already created before Init!", this);
            return;
        }
        lightAtkActId = new NativeArray<CapsuleCharActSt>(
            handEquippableSos.Count,
            Allocator.Persistent
        );
        heavyAtkActId = new NativeArray<CapsuleCharActSt>(
            handEquippableSos.Count,
            Allocator.Persistent
        );
        ultAtkActId = new NativeArray<CapsuleCharActSt>(
            handEquippableSos.Count,
            Allocator.Persistent
        );
        // TODO: This doesn't work, how do you allocate a normal array?
        prefab = new HandEquippable[handEquippableSos.Count];
        foreach (So_HandEquippable handEquippable in handEquippableSos) {
            prefab[(int)handEquippable.t] = handEquippable.prefab;
            lightAtkActId[(int)handEquippable.t] = handEquippable.lightAtkAct;
            heavyAtkActId[(int)handEquippable.t] = handEquippable.heavyAtkAct;
            ultAtkActId[(int)handEquippable.t] = handEquippable.ultAtkAct;
        }
        Debug.Log($"Initialized {handEquippableSos.Count} hand equippables!", this);
    }

    void OnDestroy() {
        lightAtkActId.Dispose();
        heavyAtkActId.Dispose();
        ultAtkActId.Dispose();
    }

    public HandEquippable InstantiateHandEquippable(HandEquippableT t) {
        Debug.Assert(prefab[(int)t] != null, $"No prefab for type: {t}");
        return Instantiate(prefab[(int)t]);
    }
}
