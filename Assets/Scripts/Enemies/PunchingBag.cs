using System.Collections;
using UnityEngine;

public class PunchingBag : MonoBehaviour, IHitReceiver {
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material grey;
    [SerializeField] Material red;
    Coroutine turnRedProcess;

    public PawnTeam GetTeam => PawnTeam.EnemyToAll;
    bool IHitReceiver.IgnoreAllHits => false;

    public HitResult ReceiveHit(HitData hitData) {
        if (turnRedProcess != null)
            StopCoroutine(turnRedProcess);
        turnRedProcess = StartCoroutine(TurnRed());
        return new HitResult(this, false, false);
    }

    IEnumerator TurnRed() {
        meshRenderer.material = red;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material = grey;
        turnRedProcess = null;
    }
}
