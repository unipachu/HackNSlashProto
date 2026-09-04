using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Obsolete("Moving away from this config thing. Each item with unique data has its own manager.")]
public class HandItemConfigs : Singleton<HandItemConfigs>{
    //[SerializeField] List<So_HandItemConfig> handItemConfigSos;

    //[HideInInspector] public HandItemConfig[] handItemConfigs;

    //protected override void Awake() {
    //    base.Awake();
    //    // fore each handItemConfigSos create a handItemConfig and add it to handItemConfigs. Also create enum where each enum corresponds to an index in handItemConfigs and where enums are named based on So_HandItemConfig.enumName.
    //}

    //public void Init() {
    //    if (
    //        lightAtkActId.IsCreated
    //        || heavyAtkActId.IsCreated
    //        || ultAtkActId.IsCreated
    //    ) {
    //        Debug.LogError("Data already created before Init!", this);
    //        return;
    //    }
    //    lightAtkActId = new NativeArray<CpActSt>(
    //        handItemConfigSos.Count,
    //        Allocator.Persistent
    //    );
    //    heavyAtkActId = new NativeArray<CpActSt>(
    //        handItemConfigSos.Count,
    //        Allocator.Persistent
    //    );
    //    ultAtkActId = new NativeArray<CpActSt>(
    //        handItemConfigSos.Count,
    //        Allocator.Persistent
    //    );
    //    // TODO: This doesn't work, how do you allocate a normal array?
    //    prefab = new HandItem[handItemConfigSos.Count];
    //    foreach (So_HandItemConfig handItemConfig in handItemConfigSos) {
    //        prefab[(int)handItemConfig.config.t] = handItemConfig.config.prefab;
    //        lightAtkActId[(int)handItemConfig.config.t] = handItemConfig.config.lightAtkAct;
    //        heavyAtkActId[(int)handItemConfig.config.t] = handItemConfig.config.heavyAtkAct;
    //        ultAtkActId[(int)handItemConfig.config.t] = handItemConfig.config.ultAtkAct;
    //    }
    //    Debug.Log($"Initialized {handItemConfigSos.Count} hand equippables!", this);
    //}

    //public HandItem InstantiateHandEquippable(HandItemT t) {
    //    Debug.Assert(handItemConfigs[(int)t].prefab != null, $"No prefab for type: {t}");
    //    return Instantiate(handItemConfigs[(int)t].prefab);
    //}
}
