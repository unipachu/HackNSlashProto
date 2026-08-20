using UnityEngine;

[CreateAssetMenu(fileName = "GlobalData_", menuName = "Scriptable Object Data/Global Data")]
public class So_GlobalData : ScriptableObject {
    public LayerMask groundMask = Physics.AllLayers;
    public float isGroundedChkDist = 0.1f;
}
