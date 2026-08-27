using UnityEngine;

/// <summary>
/// Capsule character.
/// </summary>
// TODO MINOR: Rename to Ca.
public class Pc : MonoBehaviour{
    [Header("Scriptable Object Data")]
    [SerializeField] So_CapsuleCharData so_capsuleCharData;
    [SerializeField] So_BtRootNode so_BtRootNode;
    
    [Header("Unity Comp Refs")]
    public CapsuleChar_UnityComps unityComps;

    public int Id { get; private set; } = -1;

    void OnEnable(){
        Debug.Assert(CapsuleCharMgr.inst != null, "CapsuleCharMgr inst was null!", this);
        Debug.Assert(so_capsuleCharData != null, "No data ref set!", this);
        unityComps.capsuleCharRootMvmtBroadcaster.OnRootMove += OnAnimatorRootMove;
        unityComps.rHandEquippable = HandEquippableMgr.inst.InstantiateHandEquippable(
            so_capsuleCharData.equip_RHandEquippable
        );
        unityComps.rHandEquippable.gameObject.transform.SetPositionAndRotation(
            unityComps.rHand.position,
            unityComps.rHand.rotation
        );
        unityComps.rHandEquippable.transform.parent = unityComps.rHand;
        Id = CapsuleCharMgr.inst.Register(so_capsuleCharData, unityComps, so_BtRootNode);
    }

    void OnDisable(){
        unityComps.capsuleCharRootMvmtBroadcaster.OnRootMove -= OnAnimatorRootMove;
        if (Id != -1) {
            // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
            // NOTE C: scene is being changed.
            if (CapsuleCharMgr.inst != null) {
                CapsuleCharMgr.inst.Unregister(Id);
                BtMgr.inst.Unregister(Id);
            }
            Id = -1;
        }
    }

    // TODO MINOR: It's maybe a little random that this class handles this but whatever.
    /// <summary>
    /// Used to save latest animation delta movement. Makes y component 0.
    /// </summary>
    /// <param name="dPos">
    /// Delta movement of animation root.
    /// </param>
    void OnAnimatorRootMove(Vector3 dPos, Quaternion dRot){
        CapsuleCharMgr.inst.data.animDPos[Id] = dPos;
    }
}
