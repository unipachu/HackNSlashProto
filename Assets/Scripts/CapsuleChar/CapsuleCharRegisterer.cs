using UnityEngine;

/// <summary>
/// NOTE: This should be before the capsule characters in the execution order!
/// </summary>
public class CapsuleCharRegisterer : MonoBehaviour {
    [SerializeField] So_CapsuleCharData so_capsuleCharData;
    [SerializeField] So_BtRootNode so_BtRootNode;

    public CapsuleCharData Data {
        get => CapsuleCharMgr.inst.GetData(Id);
        set => CapsuleCharMgr.inst.SetData(Id, value);
    }
    public int Id { get; private set; } = -1;

    void OnEnable() {
        Debug.Assert(CapsuleCharMgr.inst != null, "CapsuleCharMgr inst was null!", this);
        Debug.Assert(so_capsuleCharData != null, "No data ref set!", this);
        Id = CapsuleCharMgr.inst.Register(so_capsuleCharData, so_BtRootNode);
    }

    void OnDisable() {
        if (Id != -1 ) {
            // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
            // NOTE C: scene is being changed.
            if(CapsuleCharMgr.inst != null) {
                CapsuleCharMgr.inst.Unregister(Id);
                BtMgr.inst.Unregister(Id);
            }
            Id = -1;
        }
    }
}
