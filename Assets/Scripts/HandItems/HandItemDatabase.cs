using UnityEngine;

/// <summary>
/// Lookup for all hand items.
/// </summary>
public class HandItemDatabase : Singleton<HandItemDatabase> {
    [SerializeField] HandItemCatalog catalog;

    public So_HandItem Get(HandItemT t) {
        int index = (int)t;
        if (index < 0 || index >= catalog.Items.Count) {
            Debug.LogError(
                $"HandItemT {t} has no corresponding hand item.",
                this
            );

            return null;
        }
        return catalog.Items[index];
    }
}
