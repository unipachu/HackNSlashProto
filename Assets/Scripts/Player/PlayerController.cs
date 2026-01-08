using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Units per second.")]
    [SerializeField] private float maxLinearSpeed = 5;
    [Tooltip("Degrees per second.")]
    [SerializeField] private float maxAngularSpeed = 800;
    [Tooltip("Units per second squared.")]
    [SerializeField] private float acceleration = 100;

    [Header("Attack Settings")]
    [Tooltip("In seconds.")]
    [SerializeField] private int swordSwingDmg = 1;

    [Header("Refs")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private WeaponColliderHitSensor _weaponSensor;

    [Header("Input Related Refs")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionProperty _moveInputAction;
    [SerializeField] private InputActionProperty _attackInputAction;

    [Header("Animaton Related Refs")]
    [SerializeField] private CustomAnimator_CharacterVisuals _characterVisualsAnimationController;
    [SerializeField] private CustomAnimatorState_CharacterVisuals_SwingHandR_1 swingR1State;
    [SerializeField] private CustomAnimatorState_CharacterVisuals_SwingHandR_2 swingR2State;

    private Vector2 velocity = Vector2.zero;
    private Vector2 moveInput = Vector2.zero;
    private bool attackInput = false;

    private bool _attackActive = false;
    private bool _newAttackCanBeBuffered = false;
    private bool _bufferedAttackInput = false;
    private bool _comboWindowEnded = false;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        SubscribeToAnimationEvents();
    }

    private void Update()
    {
        ReadInputs();
        if (attackInput) TryBufferAttack();


        // TODO: Make better system using animator states instead, maybe.
        if (_characterVisualsAnimationController.IsActiveState(0, _characterVisualsAnimationController.IdleState.StateHash))
        {
            if(attackInput)
            {
                EnterNewStateAndInitialize(_characterVisualsAnimationController.SwingR1State);
            }
            else if (moveInput != Vector2.zero)
            {
                EnterNewStateAndInitialize(_characterVisualsAnimationController.WalkState);
            }
        }
        else if (_characterVisualsAnimationController.IsActiveState(0, _characterVisualsAnimationController.WalkState.StateHash))
        {
            SolveMovement(moveInput);

            if (attackInput)
            {
                EnterNewStateAndInitialize(_characterVisualsAnimationController.SwingR1State);
            }
            else if (moveInput == Vector2.zero)
            {
                EnterNewStateAndInitialize(_characterVisualsAnimationController.IdleState);
            }
        }
        else if(_characterVisualsAnimationController.IsActiveState(0, _characterVisualsAnimationController.SwingR1State.StateHash)
            || _characterVisualsAnimationController.IsActiveState(0, _characterVisualsAnimationController.SwingR2State.StateHash))
        {
            if(_comboWindowEnded)
            {
                // Set correct animations.
                if (moveInput != Vector2.zero
                    && !_characterVisualsAnimationController.IsActiveState(0, _characterVisualsAnimationController.WalkState.StateHash))
                {
                    EnterNewStateAndInitialize(_characterVisualsAnimationController.WalkState);
                }
            }
        }

        if (_attackActive)
        {
            // TODO: Find correct damage value.
            _weaponSensor.CheckHits(1, transform);
        }
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
        UnsubscribeToAnimationEvents();
    }

    private void ReadInputs()
    {
        moveInput = _moveInputAction.action.ReadValue<Vector2>();
        attackInput = _attackInputAction.action.WasPressedThisFrame();
    }

    private void SubscribeToAnimationEvents()
    {
        swingR1State.OnWeaponActiveStart += OnWeaponActiveStart;
        swingR1State.OnWeaponActiveEnd += OnWeaponActiveEnd;
        swingR1State.OnWeaponAttackInputBufferingEnabled += OnAttackInputBufferingAllowed;
        swingR1State.OnWeaponAttackInputBufferingDisabled += OnSwing1AttackBufferingEnd;

        swingR2State.OnWeaponActiveStart += OnWeaponActiveStart;
        swingR2State.OnWeaponActiveEnd += OnWeaponActiveEnd;
        swingR2State.OnWeaponAttackInputBufferingEnabled += OnAttackInputBufferingAllowed;
        swingR2State.OnWeaponAttackInputBufferingDisabled += OnSwing2AttackBufferingEnd;
    }

    private void UnsubscribeToAnimationEvents()
    {
        swingR1State.OnWeaponActiveStart -= OnWeaponActiveStart;
        swingR1State.OnWeaponActiveEnd -= OnWeaponActiveEnd;
        swingR1State.OnWeaponAttackInputBufferingEnabled -= OnAttackInputBufferingAllowed;
        swingR1State.OnWeaponAttackInputBufferingDisabled -= OnSwing1AttackBufferingEnd;

        swingR2State.OnWeaponActiveStart -= OnWeaponActiveStart;
        swingR2State.OnWeaponActiveEnd -= OnWeaponActiveEnd;
        swingR2State.OnWeaponAttackInputBufferingEnabled -= OnAttackInputBufferingAllowed;
        swingR2State.OnWeaponAttackInputBufferingDisabled -= OnSwing2AttackBufferingEnd;
    }

    private void SolveMovement(Vector2 movementInput)
    {
        UpdateVelocity(movementInput);
        Vector3 XYVelocity = new Vector3(velocity.x, 0, velocity.y);
        characterController.SimpleMove(XYVelocity);
        RotateForward();
    }

    private void UpdateVelocity(Vector2 movementInput)
    {
        velocity = Vector2.MoveTowards(velocity, movementInput * maxLinearSpeed, acceleration * Time.deltaTime);
    }

    public void RotateForward()
    {
        if(velocity ==  Vector2.zero) return;

        Vector3 dir3D = new(velocity.x, 0, velocity.y);

        Quaternion targetRotation = Quaternion.LookRotation(dir3D, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngularSpeed * Time.deltaTime);
    }

    public bool TryBufferAttack()
    {
        if (_newAttackCanBeBuffered)
        {
            _bufferedAttackInput = true;
            return true;
        }
        else return false;
    }

    /// <summary>
    /// NOTE: Always call this when entering new animation state.
    /// </summary>
    private void EnterNewStateAndInitialize(CustomAnimatorState newState)
    {
        _attackActive = false;
        _newAttackCanBeBuffered = false;
        _bufferedAttackInput = false;
        _comboWindowEnded = false;
        _characterVisualsAnimationController.RequestCrossfadeTo(newState);
    }

    public void OnWeaponActiveStart()
    {
        _weaponSensor.BeginAttack();
        _attackActive = true;
    }

    public void OnWeaponActiveEnd()
    {
        _attackActive = false;
    }

    public void OnAttackInputBufferingAllowed()
    {
        _newAttackCanBeBuffered = true;
    }

    public void OnSwing1AttackBufferingEnd()
    {
        if (_bufferedAttackInput)
        {
            // TODO: Calling this causes an error, likely because it is called in some other time than Update().
            // TODO: Animator seems to only register transition to a new state at a certain point during frame cycle. This is weird so you should write it down.
            EnterNewStateAndInitialize(_characterVisualsAnimationController.SwingR2State);
        }
        else
        {
            _newAttackCanBeBuffered = false;
            _comboWindowEnded = true;
        }
    }

    public void OnSwing2AttackBufferingEnd()
    {
        if (_bufferedAttackInput)
        {
            // TODO: Calling this causes an error, likely because it is called in some other time than Update().
            // TODO: Animator seems to only register transition to a new state at a certain point during frame cycle. This is weird so you should write it down.
            EnterNewStateAndInitialize(_characterVisualsAnimationController.SwingR1State);
        }
        else
        {
            _newAttackCanBeBuffered = false;
            _comboWindowEnded = true;
        }
    }
}
