using UnityEngine;

public class EntityRegisterer : MonoBehaviour {
    [SerializeField] SO_CapsuleCharData configSo;

    public CapsuleCharacterData Data {
        get => EntityData.inst.GetData(Id);
        set => EntityData.inst.SetData(Id, value);
    }
    public int Id { get; private set; } = -1;

    void OnEnable() {
        Id = EntityData.inst.Register(gameObject, configSo);
    }

    void OnDisable() {
        if (Id != -1 ) {
            // NOTE EntityData might have been destroyed before this OnDisable, e.g. if
            // NOTE C: scene is being changed.
            if(EntityData.inst != null)
                EntityData.inst.Unregister(gameObject);
            Id = -1;
        }
    }
}
