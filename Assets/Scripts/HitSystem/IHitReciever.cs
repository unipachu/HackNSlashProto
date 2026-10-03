/// <summary>
/// Owner of the hit reciever which decides behavior when hit reciever is hit.
/// </summary>
public interface IHitReceiver {
    /// <summary>
    /// Get the team number the reciever belongs to.
    /// </summary>
    public Team GetTeam { get; }
    public bool IgnoreAllHits { get; }
    /// <summary>
    /// NOTE: <see cref="HitData.normal"/> can be zero if overlap shape failed to get
    /// hit reciever surface normal.
    /// </summary>
    /// <param name="hitData"></param>
    /// <returns></returns>
    public HitResult ReceiveHit(HitData hitData);
}
