using UnityEngine;

public class CapsuleCharRegisterer : MonoBehaviour {
    [SerializeField] SO_CapsuleCharData configSo;

    public CapsuleCharacterData Data {
        get => CapsuleCharMgr.inst.GetData(Id);
        set => CapsuleCharMgr.inst.SetData(Id, value);
    }
    public int Id { get; private set; } = -1;

    void OnEnable() {
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
