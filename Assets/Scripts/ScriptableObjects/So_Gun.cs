using UnityEngine;

[CreateAssetMenu(
    fileName = "Gun",
    menuName = "Hand Item/Gun"
)]
public class So_Gun : So_HandItem {

    [SerializeField] GunHandle prefab;
    [SerializeField] HandItemData handItemData;
    [SerializeField] GunData gunData;

    public override IHandItemHandle InstantiateNRegister() {
        GunHandle handle = Instantiate(prefab);
        handle.InitNRegister(handItemData, gunData);
        return handle;
    }
}