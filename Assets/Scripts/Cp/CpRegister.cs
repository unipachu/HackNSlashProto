using System.Collections.Generic;

public class CpRegister : Singleton<CpRegister>{
    public HashSet<ICp> cps;

    public void Init() {
        cps = new HashSet<ICp>();
    }

    /// <summary>
    /// Tries to find any <see cref="ICp"/> considered an "enemy" to <paramref name="cp"/>. Returns
    /// null if none found.
    /// </summary>
    public static ICp TryFindEnemy(ICp cp) {
        foreach(ICp enemyCand in inst.cps) {
            if (enemyCand == cp)
                continue;
            Team candTeam = enemyCand.CpCommonConfig.team;
            if (candTeam == Team.FriendToAll)
                continue;
            if (candTeam == Team.EnemyToAll || candTeam != cp.CpCommonConfig.team) {
                //Debug.Log("Found tgt: " + i);
                return enemyCand;
            }
        }
        return null;
    }
}
