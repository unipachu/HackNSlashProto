using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Uses input to command CustomCharacterController
/// NOTE: This class is set to run before default time, just
/// NOTE C: after UnityEngine.InputSystem.PlayerInput in Project Settings -> Script Execution Order.
/// </summary>
public class PlrCtrl : MonoBehaviour {
    [Header("Input Related Refs")]
    [SerializeField] InputActionAsset inputActs;
    [SerializeField] InputActionProperty movInputAct;
    [SerializeField] InputActionProperty atkInputAct;
    [SerializeField] InputActionProperty atk2InputAct;
    [SerializeField] InputActionProperty atk3InputAct;
    [SerializeField] InputActionProperty dodgeInputAct;

    [Header("Refs")]
    [SerializeField] Pc pc;

    Vector2 movInput = Vector2.zero;
    bool atkInput = false;
    bool atk2Input = false;
    bool atk3Input = false;
    bool dodgeInput = false;

    void OnEnable() {
        inputActs.FindActionMap("Player").Enable();
    }

    // Update is called once per frame
    void Update(){
        ReadInputs();
        // TODO: If the player can control menus, etc. you could mark the inputs as "consumed" here.
        pc.UpdateInput(movInput, atkInput, atk2Input, atk3Input, dodgeInput);
    }

    void OnDisable(){
        inputActs.FindActionMap("Player").Disable();
    }

    void ReadInputs(){
        movInput = movInputAct.action.ReadValue<Vector2>();
        atkInput = atkInputAct.action.WasPressedThisFrame();
        atk2Input = atk2InputAct.action.WasPressedThisFrame();
        atk3Input = atk3InputAct.action.WasPressedThisFrame();
        dodgeInput = dodgeInputAct.action.WasPressedThisFrame();
    }
}
