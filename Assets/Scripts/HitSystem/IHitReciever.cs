/// <summary>
/// Owner of the hit reciever which decides behavior when hit reciever is hit.
/// </summary>
public interface IHitReceiver {
    /// <summary>
    /// Get the team number the reciever belongs to.
    /// </summary>
    public PawnTeam GetTeam();
    public bool IgnoreAllHits();
    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData);
}
