using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "HandItemCatalog",
    menuName = "Hand Item/Catalog"
)]
public class HandItemCatalog : ScriptableObject {
    [SerializeField] List<So_HandItem> items = new();

    public IReadOnlyList<So_HandItem> Items => items;

#if UNITY_EDITOR
    public void SetItems(List<So_HandItem> newItems) {
        items.Clear();
        items.AddRange(newItems);
    }
#endif
}
