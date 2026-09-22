using UnityEngine;

public class EnemySpawnPt : MonoBehaviour {
    public bool IsSpawning { get; private set; }
    
    // TODO: Start some spawning animation and only after spawning has finished, allow spawning again.
    public bool TryBeginSpawning() {
        //if (IsSpawning)
        //    return false;
        //IsSpawning = true;
        return true;
    }

    public void EndSpawning() {
        IsSpawning = false;
    }
}