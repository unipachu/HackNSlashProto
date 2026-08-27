using UnityEngine;

/// <summary>
/// Used to register capsule pawn to <see cref="CpMgr"/>.
/// </summary>
public class CpRegisterer : MonoBehaviour{
    [Header("Scriptable Object Data")]
    [SerializeField] So_CpData so_cpData;
    [SerializeField] So_BtRootNode so_BtRootNode;
    
    [Header("Unity Comp Refs")]
    public Cp_UnityComps unityComps;

    public int Id { get; private set; } = -1;

    void OnEnable(){
        Debug.Assert(CpMgr.inst != null, $"{typeof(CpMgr).Name} inst was null!", this);
        Debug.Assert(so_cpData != null, "No data ref set!", this);
        unityComps.animRootMovBroadcaster.OnRootMove += OnAnimatorRootMove;
        unityComps.rHandEquippable = HandEquippableMgr.inst.InstantiateHandEquippable(
            so_cpData.equip_RHandEquippable
        );
        unityComps.rHandEquippable.gameObject.transform.SetPositionAndRotation(
            unityComps.rHand.position,
            unityComps.rHand.rotation
        );
        unityComps.rHandEquippable.transform.parent = unityComps.rHand;
        Id = CpMgr.inst.Register(so_cpData, unityComps, so_BtRootNode);
    }

    void OnDisable(){
        unityComps.animRootMovBroadcaster.OnRootMove -= OnAnimatorRootMove;
        if (Id != -1) {
            // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
            // NOTE C: scene is being changed.
            if (CpMgr.inst != null) {
                CpMgr.inst.Unregister(Id);
                BtMgr.inst.Unregister(Id);
            }
            Id = -1;
        }
    }

    // TODO MINOR: It's maybe a little random that this class handles this. Make separate class for it.
    /// <summary>
    /// Used to save latest animation delta movement. Makes y component 0.
    /// </summary>
    /// <param name="dPos">
    /// Delta movement of animation root.
    /// </param>
    void OnAnimatorRootMove(Vector3 dPos, Quaternion dRot){
        CpMgr.inst.data.animDPos[Id] = dPos;
    }
}
