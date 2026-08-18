using UnityEngine;

[CreateAssetMenu(fileName = "GlobalData", menuName = "Global Data")]
public class So_GlobalData : ScriptableObject {
    public LayerMask groundMask = Physics.AllLayers;
    public float groundChkDist = 0.1f;
}
