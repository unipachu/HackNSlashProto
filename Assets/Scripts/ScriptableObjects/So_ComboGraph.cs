using System;
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

public List<IComboNode> GenerateComboGraph(UnityEngine.Object ctx) {
        List<IComboNode> nodeList = new();
        for (int i = 0; i < nodes.Count; i++) {
            switch (nodes[i].t) {
                case ComboNodeT.BasicImpact:
                    nodeList.Add(new ComboNode_BasicImpact(ctx, nodes[i].animInfo));
                    break;
                case ComboNodeT.BasicRecovery:
                    nodeList.Add(new ComboNode_BasicRecovery(nodes[i].animInfo));
                    break;
                case ComboNodeT.BasicWindup:
                    nodeList.Add(new ComboNode_BasicWindup(nodes[i].animInfo));
                    break;
                default:
                    Debug.LogError($"Defaulted with {nodes[i].t}.");
                    break;
            }
        }
        // Set transitions.
        for (int i = 0; i < nodes.Count; i++) {
            // TODO: Wow, does this really work like this? Have I always assigned outside of the if scope?
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

// TODO: Move to structs file after you figure out property drawers.
[Serializable]
public struct ComboNodeConfig {
    public ComboNodeT t;
    public CpAnimInfoT animInfo;
    // TODO: Not all nodes use transitions. Could I make property drawers for this?
    public byte node_BtnE;
    public byte node_LShldr;
    public byte node_NoInput;
    public byte node_RShldr;
    public byte node_RTrg;
}
