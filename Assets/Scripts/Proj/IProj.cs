using UnityEngine;

public interface IProj {
    GameObject Go { get; }
    IHitDealer HitDealer {get; }
    /// <summary>
    /// Index used by a manager running logic on this projectile.
    /// </summary>
    int IInMgr { get; set; }
    Transform Tgt { get; set; }
    TrailRenderer TrailRenderer { get; }
    Transform Trf { get; }
}
