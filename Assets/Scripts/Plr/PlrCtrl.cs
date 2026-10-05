using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads player input devices.
/// NOTE: This class is (or should be) set to run before default time, just
/// NOTE C: after UnityEngine.InputSystem.PlayerInput in Project Settings -> Script Execution Order.
/// </summary>
public class PlrCtrl : Singleton<PlrCtrl>, ICpCtrlInputter {
    [Header("Input Action Asset")]
    [SerializeField] InputActionAsset inputActs;
    [SerializeField] string actionMapName = "Player";

    [Header("Input Action Refs")]
    [SerializeField] InputActionProperty inputAct_B;
    [SerializeField] InputActionProperty inputAct_Lb;
    [SerializeField] InputActionProperty inputAct_LStick;
    [SerializeField] InputActionProperty inputAct_PointerDelta;
    [SerializeField] InputActionProperty inputAct_Rb;
    [SerializeField] InputActionProperty inputAct_RStick;
    [SerializeField] InputActionProperty inputAct_Rt;

    CtrlInputData data;

    public Vector2 Input_RStick => data.input_RStick;
    /// <summary>
    /// Gives mouse delta movement.
    /// </summary>
    public Vector2 Input_PointerDelta => data.input_PointerDelta;
    public Vector2 Input_LStick => data.input_LStick;

    void OnEnable() {
        inputActs.FindActionMap(actionMapName).Enable();
    }

    // Update is called once per frame
    void Update(){
        ReadInputs();
    }

    void OnDisable(){
        inputActs.FindActionMap(actionMapName).Disable();
    }

    void ReadInputs(){
        data.input_B = inputAct_B.action.WasPressedThisFrame();
        data.input_Lb = inputAct_Lb.action.WasPressedThisFrame();
        // NOTE: We use camera relative movement input.
        data.input_LStick = MathUtils.TrfInputByBasis(
            inputAct_LStick.action.ReadValue<Vector2>(),
            CamMgr.inst.CamFwdDir
        );
        data.input_PointerDelta = inputAct_PointerDelta.action.ReadValue<Vector2>()
            * GameSettings.inst.lookPointerSensitivity;
        data.input_Rb = inputAct_Rb.action.WasPressedThisFrame();
        data.input_RStick = inputAct_RStick.action.ReadValue<Vector2>();
        data.input_Rt = inputAct_Rt.action.WasPressedThisFrame();
        //Debug.Log($"input_Mov: {input_Mov}.");
    }

    public bool TryConsume_Atk_Light() => CtrlUtils.TryConsume(ref data.input_Rb);

    public bool TryConsume_Atk_Heavy() => CtrlUtils.TryConsume(ref data.input_Rt);

    public bool TryConsume_Atk_Ult() => CtrlUtils.TryConsume(ref data.input_Lb);

    public bool TryConsume_Dodge() => CtrlUtils.TryConsume(ref data.input_B);
}
