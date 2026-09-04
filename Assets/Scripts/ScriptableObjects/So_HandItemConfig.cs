using System;
using UnityEngine;

/// <summary>
/// Item that can be held with one hand, e.g. a sword.
/// </summary>
[CreateAssetMenu(
    fileName = "HandItemConfig_",
    menuName = "Scriptable Object Data/HandItemConfig"
)]
[Obsolete("Moving functionality to an archetype manager.")]
public class So_HandItemConfig : ScriptableObject {
    //public string enumName;
    //public HandItem prefab;
    //public CpActSt lightAtkAct;
    //public CpActSt heavyAtkAct;
    //public CpActSt ultAtkAct;
}
