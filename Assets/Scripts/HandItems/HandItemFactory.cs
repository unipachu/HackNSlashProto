using UnityEngine;

public class HandItemFactory : Singleton<HandItemFactory> {
    public IHandItemHandle InstantiateHandItem(HandItemT t) {
        So_HandItem item = HandItemDatabase.inst.Get(t);
        if (item == null) {
            Debug.LogError($"No HandItemSO registered for {t}.", this);
            return null;
        }
        return item.InstantiateNRegister();
    }
}
