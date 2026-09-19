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

    /// <summary>
    /// NOTE: When you create new combo node classes, always add a corresponding enum and a case in this switch.
    /// </summary>
    /// <param name="ctx">
    /// Generally the hand item that uses this combo graph, but can be any object containing the data the combo
    /// node needs for initialization. Individual nodes cast this to get the initialization data they need, so
    /// check the individual nodes for data required from <paramref name="ctx"/>.
    /// </param>
    /// <returns>Ref to generated combo graph.</returns>
    public List<IComboNode> GenerateComboGraph(UnityEngine.Object ctx) {
        List<IComboNode> nodeList = new();
        for (int i = 0; i < nodes.Count; i++) {
            switch (nodes[i].t) {
                case ComboNodeConfigT.BasicImpact:
                    nodeList.Add(new ComboNode_BasicImpact(ctx, nodes[i].animInfo));
                    break;
                case ComboNodeConfigT.BasicRecovery:
                    nodeList.Add(new ComboNode_BasicRecovery(nodes[i].animInfo));
                    break;
                case ComboNodeConfigT.BasicBranch:
                    nodeList.Add(new ComboNode_BasicBranch(nodes[i].animInfo));
                    break;
                case ComboNodeConfigT.ShootProj:
                    nodeList.Add(new ComboNode_ShootProj(ctx, nodes[i].animInfo));
                    break;
                default:
                    Debug.LogError($"Defaulted with {nodes[i].t}.");
                    break;
            }
        }
        // Set transitions.
        for (int i = 0; i < nodes.Count; i++) {
            if (nodeList[i] is IComboNodeTransitionsHolder transitioner) {
                ComboNode_Transitions transitions = transitioner.Transitions;
                transitions.node_BtnE = GetNode(nodes[i].node_BtnE);
                transitions.node_LShldr = GetNode(nodes[i].node_LShldr);
                transitions.node_NoInput = GetNode(nodes[i].node_NoInput);
                transitions.node_RShldr = GetNode(nodes[i].node_RShldr);
                transitions.node_RTrg = GetNode(nodes[i].node_RTrg);
                transitioner.Transitions = transitions;
            }
        }
        return nodeList;
        // Helper
        IComboNode GetNode(byte index) => index == byte.MaxValue ? null : nodeList[index];
    }
}
