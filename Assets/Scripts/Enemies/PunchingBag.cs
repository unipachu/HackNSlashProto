using System.Collections;
using UnityEngine;

public class PunchingBag : MonoBehaviour, IHitReceiver {
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material grey;
    [SerializeField] Material red;
    Coroutine turnRedProcess;

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        if (turnRedProcess != null)
            StopCoroutine(turnRedProcess);
        turnRedProcess = StartCoroutine(TurnRed());
        return new HitResult(false, false);
    }

    IEnumerator TurnRed() {
        meshRenderer.material = red;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material = grey;
        turnRedProcess = null;
    }
}
