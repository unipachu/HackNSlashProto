using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Uses input to command CustomCharacterController
/// NOTE: This class is (or should be) set to run before default time, just
/// NOTE C: after UnityEngine.InputSystem.PlayerInput in Project Settings -> Script Execution Order.
/// </summary>
public class PlrMgr : Singleton<PlrMgr>, ICpCtrlInputter {
    [Header("Player stats")]
    [Tooltip("Window (in seconds) after an attack during which another attack is considered 'successive'.")]
    public float successiveAtkWindow = 1.5f;

    [Header("Input Action Asset")]
    [SerializeField] InputActionAsset inputActs;
    [SerializeField] string actionMapName = "Player";

    [Header("Input Action Refs")]
    [SerializeField] InputActionProperty inputAct_Atk_Light;
    [SerializeField] InputActionProperty inputAct_Atk_Heavy;
    [SerializeField] InputActionProperty inputAct_Atk_Ult;
    [SerializeField] InputActionProperty inputAct_Dodge;
    [SerializeField] InputActionProperty inputAct_Look_Gamepad;
    [SerializeField] InputActionProperty inputAct_Look_Pointer;
    [SerializeField] InputActionProperty inputAct_Mov;

    [Header("Refs")]
    [SerializeField] CamMgr camMgr;

    CtrlInputData data;

    public Vector2 Input_Look_Gamepad => data.input_Look_Gamepad;
    /// <summary>
    /// Gives mouse delta.
    /// </summary>
    public Vector2 Input_Look_Pointer => data.input_Look_Pointer;
    public Vector2 Input_Mov => data.input_Mov;

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
        data.input_Atk_Light = inputAct_Atk_Light.action.WasPressedThisFrame();
        data.input_Atk_Heavy = inputAct_Atk_Heavy.action.WasPressedThisFrame();
        data.input_Atk_Ult = inputAct_Atk_Ult.action.WasPressedThisFrame();
        data.input_Dodge = inputAct_Dodge.action.WasPressedThisFrame();
        data.input_Look_Gamepad = inputAct_Look_Gamepad.action.ReadValue<Vector2>();
        data.input_Look_Pointer = inputAct_Look_Pointer.action.ReadValue<Vector2>()
            * GameSettings.inst.lookPointerSensitivity;
        // NOTE: We use camera relative movement input.
        data.input_Mov = MathUtils.TrfInputByBasis(
            inputAct_Mov.action.ReadValue<Vector2>(),
            camMgr.CamFwdDir
        );
        //Debug.Log($"input_Mov: {input_Mov}.");
    }

    public bool TryConsume_Atk_Light() => CtrlUtils.TryConsume(ref data.input_Atk_Light);

    public bool TryConsume_Atk_Heavy() => CtrlUtils.TryConsume(ref data.input_Atk_Heavy);

    public bool TryConsume_Atk_Ult() => CtrlUtils.TryConsume(ref data.input_Atk_Ult);

    public bool TryConsume_Dodge() => CtrlUtils.TryConsume(ref data.input_Dodge);
}
