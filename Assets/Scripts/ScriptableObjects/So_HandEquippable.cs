using UnityEngine;

/// <summary>
/// Things a capsule character can equip to one hand, e.g. a sword.
/// </summary>
[CreateAssetMenu(
    fileName = "HandEquippable_",
    menuName = "Scriptable Object Data/HandEquippable"
)]
public class So_HandEquippable : ScriptableObject {
    public HandEquippableT t;
    public HandEquippable prefab;
    public CpActSt lightAtkAct;
    public CpActSt heavyAtkAct;
    public CpActSt ultAtkAct;
}
