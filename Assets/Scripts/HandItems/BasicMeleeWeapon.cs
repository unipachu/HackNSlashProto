using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : MonoBehaviour, IHandItem_Comboer, IHandItem_HitDealer {
    [SerializeField] HitDealer hitDealer;
    [SerializeField] So_ComboGraph comboGraphConfig;

    List<IComboNode> comboGraph;

    public HitDealer HitDealer => hitDealer;
    public IComboNode LShldrComboStart => null;
    public IComboNode RShldrComboStart => comboGraph[0];
    public IComboNode RTrgComboStart => null;
    public Transform Trf => transform;

    void Awake() {
        // TODO: Not sure if So_ComboGraph has been initialized as of yet.
        comboGraph = comboGraphConfig.GenerateComboGraph(this);
        //comboGraph.Add(
        //    new ComboNode_BasicWindup(
        //        CpAnimInfo.atk_HorSlash1_Windup,
        //        comboGraph,
        //        -1,
        //        -1,
        //        1,
        //        -1,
        //        -1
        //    )
        //);
        //comboGraph.Add(
        //    new ComboNode_BasicImpact(
        //        CpAnimInfo.atk_HorSlash1_Impact,
        //        comboGraph,
        //        hitDealer,
        //        -1,
        //        -1,
        //        2,
        //        -1,
        //        -1
        //    )
        //);
        //comboGraph.Add(new ComboNode_BasicRecovery(CpAnimInfo.atk_HorSlash1_Recovery));
    }

}
