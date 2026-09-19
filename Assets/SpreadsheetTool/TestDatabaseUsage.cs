// BEFORE BUILD: This is just a test class. You can delete after you're actually using the databases elsewhere.
using UnityEngine;

/// <summary>
/// Just some testing´.
/// </summary>
public class TestDatabaseUsage : MonoBehaviour {
    [SerializeField] ExampleGameDatabase database;
    void Start(){
        //Debug.Log(database.GetItem("jee").DisplayName, this);
    }
}
