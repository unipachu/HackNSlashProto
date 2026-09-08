// TODO: Delete
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboNode_BasicWindup_",
    menuName = "Scriptable Object Data/ComboNode_BasicWindup")]
[Obsolete("Not used anymore")]
public class So_ComboNode_BasicWindup : So_ComboNode{
    public CpAnimInfoT animInfoT;
    public int node_BtnE;
    public int node_LShldr;
    public int node_NoInput;
    public int node_RShldr;
    public int node_RTrg;

    public override IComboNode GenerateNode<THandItem>(THandItem ctx) {
        return new ComboNode_BasicWindup(animInfoT);
    }
}
