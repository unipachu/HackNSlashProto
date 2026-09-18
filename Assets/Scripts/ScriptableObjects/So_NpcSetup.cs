using UnityEngine;

[CreateAssetMenu(fileName = "AiCpConfig_", menuName = "Scriptable Object Data/AiCpConfig")]
// TODO: Rename to AiCpConfig
public class So_NpcSetup : ScriptableObject{
    public CpRegisterer cpPrefab;
    public BtNodeConfig btNodeConfig;
}
