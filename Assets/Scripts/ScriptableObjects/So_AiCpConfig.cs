using UnityEngine;

[CreateAssetMenu(fileName = "AiCpConfig_", menuName = "Scriptable Object Data/AiCpConfig")]
public class So_AiCpConfig : ScriptableObject{
    public AiCtrlConfigData data = new(20, 3);
    public CpHandle cpPrefab;
    public BtT btT;
}
