using UnityEngine;

/// <summary>
/// Used as a memory managed handle to the entity id.
/// </summary>
public class AiCtrlHandle : ICpCtrlInputter {
    /// <summary>
    /// Index to the corresponding entity data <see cref="AiCtrlMgr"/>.
    /// </summary>
    public int I { get; set; } = -1;

    public ref AiCtrlData Data => ref AiCtrlMgr.inst.aos[I];
    public Vector2 Input_Look_Gamepad => AiCtrlMgr.GetData(I).ctrlInputData.input_Look_Gamepad;
    /// <summary>
    /// Gives mouse delta.
    /// </summary>
    public Vector2 Input_Look_Pointer => AiCtrlMgr.GetData(I).ctrlInputData.input_Look_Pointer;
    public Vector2 Input_Mov => AiCtrlMgr.GetData(I).ctrlInputData.input_Mov;
    
    public bool TryConsume_Atk_Light()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_Atk_Light);

    public bool TryConsume_Atk_Heavy()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_Atk_Heavy);

    public bool TryConsume_Atk_Ult()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_Atk_Ult);

    public bool TryConsume_Dodge()
        => CtrlUtils.TryConsume(ref AiCtrlMgr.GetData(I).ctrlInputData.input_Dodge);

    public void OnCpMarkedForUnregister() {
        AiCtrlMgr.inst.Unregister(I);
    }
}
