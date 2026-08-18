/// <summary>
/// Owner of the hit reciever which decides behavior when hit reciever is hit.
/// </summary>
public interface IHitReceiverOwner {
    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData);
}
