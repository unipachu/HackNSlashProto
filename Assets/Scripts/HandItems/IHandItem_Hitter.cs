public interface IHandItem_Hitter : IHandItem{
    IHitDealer HitDealer { get; }
    HitEffects HitEffects { get; }
}
