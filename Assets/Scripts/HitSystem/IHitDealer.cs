using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can deal hits to hit recievers.
/// </summary>
public interface IHitDealer{
    Vector3 WldDir { get; set; }
    
    public void Deactivate();

    public void ResetNActivate(
        Transform hitSource,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        PawnTeam team,
        Vector3 wldDir
    );
}
