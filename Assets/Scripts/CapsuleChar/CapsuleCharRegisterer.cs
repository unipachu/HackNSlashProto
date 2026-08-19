using UnityEngine;

/// <summary>
/// NOTE: This should be before the capsule characters in the execution order!
/// </summary>
public class CapsuleCharRegisterer : MonoBehaviour {
    [SerializeField] So_CapsuleCharData configSo;

    public CapsuleCharData Data {
        get => CapsuleCharMgr.inst.GetData(Id);
        set => CapsuleCharMgr.inst.SetData(Id, value);
    }
    public int Id { get; private set; } = -1;

    void OnEnable() {
        Debug.Assert(CapsuleCharMgr.inst != null, "CapsuleCharMgr inst was null!", this);
        Id = CapsuleCharMgr.inst.Register(gameObject, configSo);
    }

    void OnDisable() {
        if (Id != -1 ) {
            // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
            // NOTE C: scene is being changed.
            if(CapsuleCharMgr.inst != null)
                CapsuleCharMgr.inst.Unregister(gameObject);
            Id = -1;
        }
    }
}
