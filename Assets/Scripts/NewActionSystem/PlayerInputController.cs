using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Uses input to command CustomCharacterController
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    [Header("Input Related Refs")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] InputActionProperty _moveInputAction;
    [SerializeField] InputActionProperty _attackInputAction;

    [Header("Refs")]
    [SerializeField] private PlayerController _customCC;

    Vector2 moveInput = Vector2.zero;
    bool attackInput = false;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInputs();
        _customCC.UpdateInput(moveInput, attackInput);
        _customCC.UpdateActionControllers();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void ReadInputs()
    {
        moveInput = _moveInputAction.action.ReadValue<Vector2>();
        attackInput = _attackInputAction.action.WasPressedThisFrame();
    }
}
