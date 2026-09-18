using UnityEngine;

/// <summary>
/// Used as a memory managed reference to the capsule pawn's id. Also contains capsule pawn initialization data.
/// </summary>
// Rename to just Cp.
public class CpRegisterer : MonoBehaviour, LockOnTgt, IPawn{
    [Header("Scriptable Object Data")]
    public So_CpData so_cpData;
    
    [Header("Unity Comp Refs")]
    public Cp_UnityObjs unityObjs;

    public int Id { get; set; }

    // NOTE: This is a little cheating but we do not need to access the manager to access the lock on transform.
    public Transform Trf => unityObjs.lockOnTrf;

    //void OnDestroy(){
    //    if (Id != -1) {
    //        // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
    //        // NOTE C: scene is being changed.
    //        if (CpMgr.inst != null) {
    //            CpMgr.inst.Unregister(Id);
    //            BtMgr.inst.Unregister(Id);
    //        }
    //        Id = -1;
    //    }
    //}

    ///// <summary>
    ///// Sets up and registers this cp.<br/>
    ///// NOTE: This should be called right after initialization.
    ///// </summary>
    //public void Init(ICpCtrlInputter ctrl) {

    //    CpMgr.inst.Register(ctrl, this, unityComps, so_cpData, so_BtRootNode);
    //}
}
