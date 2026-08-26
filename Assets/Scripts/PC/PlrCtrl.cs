using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Uses input to command CustomCharacterController
/// NOTE: This class is (or should be) set to run before default time, just
/// NOTE C: after UnityEngine.InputSystem.PlayerInput in Project Settings -> Script Execution Order.
/// </summary>
public class PlrCtrl : MonoBehaviour, ICapsuleCharCtrlInputter {
    [Tooltip("Mouse (or joystick hatswitch) sensitivity for look input.")]
    [SerializeField] float lookPointerSensitivity = 1;

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

    bool input_Atk_Light = false;
    bool input_Atk_Heavy = false;
    bool input_Atk_Ult = false;
    bool input_Dodge = false;
    Vector2 input_Look_Gamepad = Vector2.zero;
    Vector2 input_Look_Pointer = Vector2.zero;
    Vector2 input_Mov = Vector2.zero;

    public Vector2 Input_Look_Gamepad => input_Look_Gamepad;
    /// <summary>
    /// Gives mouse delta.
    /// </summary>
    public Vector2 Input_Look_Pointer => input_Look_Pointer;
    public Vector2 Input_Mov => input_Mov;

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
        input_Atk_Light = inputAct_Atk_Light.action.WasPressedThisFrame();
        input_Atk_Heavy = inputAct_Atk_Heavy.action.WasPressedThisFrame();
        input_Atk_Ult = inputAct_Atk_Ult.action.WasPressedThisFrame();
        input_Dodge = inputAct_Dodge.action.WasPressedThisFrame();
        input_Look_Gamepad = inputAct_Look_Gamepad.action.ReadValue<Vector2>();
        input_Look_Pointer = inputAct_Look_Pointer.action.ReadValue<Vector2>() * lookPointerSensitivity;
        // NOTE: We use camera relative movement input.
        input_Mov = MathUtils.TrfInputByBasis(
            inputAct_Mov.action.ReadValue<Vector2>(),
            camMgr.CamFwdDir
        );
        //Debug.Log($"input_Mov: {input_Mov}.");
    }

    // -----------------------------------------------------------------
    // Try Consume Methods
    // -----------------------------------------------------------------

    public static bool TryConsume(ref bool input) {
        if (!input)
            return false;
        input = false;
        return true;
    }

    public bool TryConsume_Atk_Light() => TryConsume(ref input_Atk_Light);

    public bool TryConsume_Atk_Heavy() => TryConsume(ref input_Atk_Heavy);

    public bool TryConsume_Atk_Ult() => TryConsume(ref input_Atk_Ult);

    public bool TryConsume_Dodge() => TryConsume(ref input_Dodge);
}
