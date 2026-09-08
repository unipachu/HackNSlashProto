// TODO: Delete
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboNode_BasicRecovery_",
    menuName = "Scriptable Object Data/ComboNode_BasicRecovery")]
[Obsolete("Not used anymore")]
public class So_ComboNode_BasicRecovery : So_ComboNode {
    public CpAnimInfoT animInfoT;

    public override IComboNode GenerateNode<THandItem>(THandItem ctx)
        => new ComboNode_BasicRecovery(animInfoT);
}
