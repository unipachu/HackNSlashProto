using UnityEngine;

/// <summary>
/// Used as a memory managed handle to the entity id.
/// </summary>
public class AiCtrlHandle : ICpCtrl {
    /// <summary>
    /// Index to the corresponding entity data <see cref="AiCtrlMgr"/>.
    /// </summary>
    public int I { get; set; } = -1;

    public ref AiCtrlData Data => ref AiCtrlMgr.inst.aos[I];
    public Vector2 Input_RStick => AiCtrlMgr.GetData(I).ctrlInputData.input_RStick;
    /// <summary>
    /// Gives mouse delta.
    /// </summary>
    public Vector2 Input_PointerDelta => AiCtrlMgr.GetData(I).ctrlInputData.input_PointerDelta;
    public Vector2 Input_LStick => AiCtrlMgr.GetData(I).ctrlInputData.input_LStick;
    
    public bool TryConsume_Atk_Light()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_Rb);

    public bool TryConsume_Atk_Heavy()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_Rt);

    public bool TryConsume_Atk_Ult()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_Lb);

    public bool TryConsume_Dodge()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_B);

    public void OnCpMarkedForUnregister() {
        AiCtrlMgr.inst.Unregister(I);
    }
}
