using UnityEngine;

[CreateAssetMenu(fileName = "AiCpConfig_", menuName = "Scriptable Object Data/AiCpConfig")]
// TODO: Rename to AiCpConfig
public class So_NpcSetup : ScriptableObject{
    public AiCtrlConfigData data = new(20, 3);
    public CpRegisterer cpPrefab;
    public BtT btT;
}
