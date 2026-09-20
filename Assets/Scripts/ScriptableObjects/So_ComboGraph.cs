using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboGraph_", menuName = "Scriptable Object Data/ComboGraph")]
public class So_ComboGraph : ScriptableObject {
    [Tooltip("These represents the combo moves and transitions. NOTE: 255 = no transition!")]
    public List<ComboNodeConfig> nodes = new () {
        new ComboNodeConfig {
            node_BtnE = byte.MaxValue,
            node_LShldr = byte.MaxValue,
            node_NoInput = byte.MaxValue,
            node_RShldr = byte.MaxValue,
            node_RTrg = byte.MaxValue
        }
    };
}
