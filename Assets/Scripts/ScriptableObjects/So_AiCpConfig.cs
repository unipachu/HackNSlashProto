using UnityEngine;

/// <summary>
/// Used to configure data of the ai controller.
/// </summary>
[CreateAssetMenu(fileName = "AiCpConfig_", menuName = "Scriptable Object Data/AiCpConfig")]
public class So_AiCpConfig : ScriptableObject{
    public float aggroRange = 20;
    public float atkRange = 3;
    // TODO: Serialize
    public InterfaceReference<ICp> cpPrefab;
    public BtT btT;
}
