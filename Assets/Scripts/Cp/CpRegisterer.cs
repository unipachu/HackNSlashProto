using UnityEngine;

/// <summary>
/// Used to register and unregister capsule pawn to <see cref="CpMgr"/>.
/// </summary>
// Rename to just Cp.
public class CpRegisterer : MonoBehaviour, LockOnTgt{
    [Header("Scriptable Object Data")]
    [SerializeField] So_CpData so_cpData;
    [SerializeField] So_BtRootNode so_BtRootNode;
    
    [Header("Unity Comp Refs")]
    [SerializeField] Cp_UnityObjs unityComps;

    public int Id { get; set; }

    public Transform Trf => unityComps.lockOnTrf;

    void OnDestroy(){
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

    /// <summary>
    /// Sets up and registers this cp.<br/>
    /// NOTE: This should be called right after initialization.
    /// </summary>
    public void Init(ICpCtrlInputter ctrl) {
        // If these are not set to false, the nav mesh agent component will try to move the capsule pawn trf.
        // NOTE: NavMeshAgent will still move its own position and rotation which can cause problems if you don't
        // NOTE C: set the drifting navmesh position back to the transform position and rotation every time you move
        // NOTE C: the capsule pawn.
        unityComps.navMeshAgent.updatePosition = false;
        unityComps.navMeshAgent.updateRotation = false;
        Debug.Assert(CpMgr.inst != null, $"{typeof(CpMgr).Name} inst was null!", this);
        Debug.Assert(so_cpData != null, "No data ref set!", this);
        IHandItem rHandItem = HandItemFactory.inst.InstantiateHandItem(so_cpData.rHandItem);
        rHandItem.Trf.SetPositionAndRotation(
            unityComps.rHand.position,
            unityComps.rHand.rotation
        );
        rHandItem.Trf.parent = unityComps.rHand;
        CpMgr.inst.Register(ctrl, this, unityComps, rHandItem, so_cpData, so_BtRootNode);
    }
}
