using UnityEngine;

public class HitReceiver : MonoBehaviour {
    public IHitReceiverOwner owner;

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        //Debug.Log($"{gameObject.name} was hit by {hitDealer.gameObject.name}", this);
        Debug.Assert(owner != null, "HitReciever had no owner! Owner should register to Hit Reciever's "
            + "owner when the Hit Reciever is created!", this);
        return owner.ReceiveHit(hitDealer, hitData);
    }
}
