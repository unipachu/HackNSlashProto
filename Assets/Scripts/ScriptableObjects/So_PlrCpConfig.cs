using UnityEngine;

/// <summary>
/// Readonly config data for player.
/// </summary>
[CreateAssetMenu(fileName = "PlrCpConfig_", menuName = "Scriptable Object Data/PlrCpConfig")]
public class So_PlrCpConfig : ScriptableObject {
    public int maxUlt = 5;
}
