// TODO: Delete
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BtNode_", menuName = "Scriptable Object Data/BtNode")]
[Obsolete]
public class So_BtNode : ScriptableObject {
    public BtNodeT t;
    public So_BtNode[] children;
}
