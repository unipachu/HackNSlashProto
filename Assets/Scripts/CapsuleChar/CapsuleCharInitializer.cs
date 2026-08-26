// TODO: Delete

using UnityEngine;

/// <summary>
/// NOTE: This should be before the capsule characters in the execution order!
/// </summary>
public class CapsuleCharInitializer : MonoBehaviour {
    //[SerializeField] Pc cc;
    //[SerializeField] So_BtRootNode so_BtRootNode;

    //public int Id { get; private set; } = -1;

    //void OnEnable() {
    //    Debug.Assert(CapsuleCharMgr.inst != null, "CapsuleCharMgr inst was null!", this);
    //    Debug.Assert(so_capsuleCharData != null, "No data ref set!", this);
    //    Id = CapsuleCharMgr.inst.Register(so_capsuleCharData, so_BtRootNode);
    //    cc.rHandEquippable = HandEquippableMgr.inst.InstantiateHandEquippable(
    //        CapsuleCharMgr.inst.equip_RHandEquippable[Id]
    //    );
    //    cc.rHandEquippable.gameObject.transform.SetPositionAndRotation(
    //        cc.rHand.position,
    //        cc.rHand.rotation
    //    );
    //    cc.rHandEquippable.transform.parent = cc.rHand;
    //}

    //void OnDisable() {
    //    if (Id != -1 ) {
    //        // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
    //        // NOTE C: scene is being changed.
    //        if(CapsuleCharMgr.inst != null) {
    //            CapsuleCharMgr.inst.Unregister(Id);
    //            BtMgr.inst.Unregister(Id);
    //        }
    //        Id = -1;
    //    }
    //}
}
