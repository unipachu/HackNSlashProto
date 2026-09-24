using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can deal hits to hit recievers.
/// </summary>
public interface IHitDealer{
    public void Deactivate();

    public void ResetNActivate(
        Transform hitSource,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        PawnTeam team
    );
}
