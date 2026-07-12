using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Uses input to command CustomCharacterController
/// NOTE: This class is set to run before default time, just
/// NOTE C: after UnityEngine.InputSystem.PlayerInput in Project Settings -> Script Execution Order.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Input Related Refs")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] InputActionProperty moveInputAction;
    [SerializeField] InputActionProperty atkInputAction;
    [SerializeField] InputActionProperty atk2InputAction;
    [SerializeField] InputActionProperty atk3InputAction;
    [SerializeField] InputActionProperty dodgeInputAction;

    [Header("Refs")]
    [SerializeField] private PC pc;

    Vector2 moveInput = Vector2.zero;
    bool attackInput = false;
    bool attack2Input = false;
    bool attack3Input = false;
    bool dodgeInput = false;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInputs();
        // TODO: If the player can control menus, etc. you could mark the inputs as "consumed" here.
        pc.UpdateInput(moveInput, attackInput, attack2Input, attack3Input, dodgeInput);
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void ReadInputs()
    {
        moveInput = moveInputAction.action.ReadValue<Vector2>();
        attackInput = atkInputAction.action.WasPressedThisFrame();
        attack2Input = atk2InputAction.action.WasPressedThisFrame();
        attack3Input = atk3InputAction.action.WasPressedThisFrame();
        dodgeInput = dodgeInputAction.action.WasPressedThisFrame();
    }
}
