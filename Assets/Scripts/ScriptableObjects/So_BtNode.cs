using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BtNode_", menuName = "Scriptable Object Data/BtNode")]
public class So_BtNode : ScriptableObject {
    public FixedString64Bytes nodeName;
    public BtNodeT t;
    public So_BtNode[] children;
}
