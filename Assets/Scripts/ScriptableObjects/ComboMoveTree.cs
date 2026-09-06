using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboMoveTree_", menuName = "Scriptable Object Data/ComboMoveTree")]
// TODO: Rename to So_ComboGraph
public class ComboMoveTree : ScriptableObject {
    [Tooltip("NOTE: Put the root node to index 0!")]
    // TODO: Is there a better way to create a tree structure than just having nodes referencing each other
    // TODO C: like this for this usage purpose? 
    [SerializeField] List<IComboNode> nodes;

    public IComboNode GetNode(int i)
        => nodes[i];
}
