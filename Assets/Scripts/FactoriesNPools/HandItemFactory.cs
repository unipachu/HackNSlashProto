using System;
using UnityEngine;

public class HandItemFactory : Singleton<HandItemFactory> {
    [SerializeField] So_HandItemCatalog catalog;

    protected override void Awake() {
        base.Awake();
        Debug.Assert(AssertCatalog(), "Catalogue assert failed!", this);
    }

    public bool AssertCatalog() {
        if(catalog.Items.Count != Enum.GetValues(typeof(HandItemT)).Length) {
            Debug.LogError("catalogue items count didn't match enum member count.", this);
            return false;
        }
        for (int i = 0; i < catalog.Items.Count; i++) {
            string enumId = Enum.GetValues(typeof(HandItemT)).GetValue(i).ToString();
            if (catalog.Items[i].name != enumId) {
                Debug.LogError($"catalogue id {catalog.Items[i].name} name didn't match enum "
                    + $"member {enumId}.", this);
                return false;
            }
        }
        return true;
    }

    public static IHandItem InstantiateHandItem(HandItemT handItemT){
        IHandItem item = Instantiate(inst.catalog.Items[(int)handItemT]).GetComponent<IHandItem>();
        Debug.Assert(item != null, $"Failed to instantiate item of type {handItemT}.", inst);
        return item;
    }
}
