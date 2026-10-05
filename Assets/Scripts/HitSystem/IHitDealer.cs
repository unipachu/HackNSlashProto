using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can deal hits to hit recievers.
/// </summary>
public interface IHitDealer{
    public event Action<HashSet<HitResult>> hitSomething;

    Vector3 HitDealerMovDir { get; set; }
    
    void Deactivate();

    void ResetNActivate(
        Transform hitSource,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        Team team,
        Vector3 wldDir
    );
}
