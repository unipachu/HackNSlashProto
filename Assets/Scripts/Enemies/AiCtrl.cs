using UnityEngine;

public class AiCtrl : MonoBehaviour, ICpCtrlInputter {
    public int Id { get; set; }

    public Vector2 Input_Look_Gamepad => AiCtrlMgr.inst.ctrlInputData[Id].input_Look_Gamepad;
    /// <summary>
    /// Gives mouse delta.
    /// </summary>
    public Vector2 Input_Look_Pointer => AiCtrlMgr.inst.ctrlInputData[Id].input_Look_Pointer;
    public Vector2 Input_Mov => AiCtrlMgr.inst.ctrlInputData[Id].input_Mov;

    public bool TryConsume_Atk_Light() => CtrlUtils.TryConsume(ref AiCtrlMgr.inst.ctrlInputData[Id].input_Atk_Light);

    public bool TryConsume_Atk_Heavy() => CtrlUtils.TryConsume(ref AiCtrlMgr.inst.ctrlInputData[Id].input_Atk_Heavy);

    public bool TryConsume_Atk_Ult() => CtrlUtils.TryConsume(ref AiCtrlMgr.inst.ctrlInputData[Id].input_Atk_Ult);

    public bool TryConsume_Dodge() => CtrlUtils.TryConsume(ref AiCtrlMgr.inst.ctrlInputData[Id].input_Dodge);

    public void Unpossess() {
        AiCtrlMgr.inst.Unregister(Id);
    }
}
