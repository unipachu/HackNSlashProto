using UnityEngine;

/// <summary>
/// Just some testing´.
/// </summary>
// TODO: Delete this class.
public class TestDatabaseUsage : MonoBehaviour
{
    [SerializeField] ExampleGameDatabase database;
    void Start()
    {
        if (database.TryGetItem("asdasd", out ExampleItemData sword))
        {
            Debug.Log(sword.DisplayName);
        }
        else
        {
            Debug.LogWarning("Coudl not find: " + "asdasd");
        }
    }
}
