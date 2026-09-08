using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandItemCatalog", menuName = "Scriptable Object Data/HandItemCatalog")]
// TODO: Rename to So_HandItemCatalog. Honestly might be easier if this was just a static/singleton class.
public class HandItemCatalog : ScriptableObject {
    [SerializeField] List<GameObject> items = new();

    public IReadOnlyList<GameObject> Items => items;

#if UNITY_EDITOR
    /// <summary>
    /// Used for catalogue asset generation.
    /// </summary>
    public void SetItems(List<GameObject> newItems) {
        items.Clear();
        items.AddRange(newItems);
    }
#endif
}
