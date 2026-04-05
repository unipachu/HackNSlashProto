using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Obsolete("Use " + nameof(PlayerController) + " instead.")]
public class OldPlayerController : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("In seconds.")]
    [SerializeField] int swordSwingDmg = 1;

    [Header("Refs")]
    [SerializeField] WeaponColliderHitSensor _weaponSensor;
    [SerializeField] CharacterLocomotion _movement;

    [Header("Input Related Refs")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] InputActionProperty _moveInputAction;
    [SerializeField] InputActionProperty _attackInputAction;

    [Header("Animaton Related Refs")]
    [SerializeField] OldCustomAnimator_CharacterVisuals _customAnimator;

    Vector2 moveInput = Vector2.zero;
    bool attackInput = false;

    bool _attackActive;
    bool _newAttackCanBeBuffered;
    bool _bufferedAttackInput;
    bool _comboWindowEnded;


    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        SubscribeToAnimationEvents();
    }

    private void Update()
    {
        ReadInputs();

        string animatorState = _customAnimator.ActiveState.StateName;

        switch (animatorState)
        {
            case "CharacterVisuals_Idle":
                if (attackInput)
                {
                    ChangeAnimatorState(_customAnimator.SwingR0State);
                }
                else if (moveInput != Vector2.zero)
                {
                    ChangeAnimatorState(_customAnimator.WalkState);
                }
                break;
            case "CharacterVisuals_Walk":
                _movement.UpdateMovement(LocomotionType.VelocityByDirectionalInput, moveInput);
                if (attackInput)
                {
                    ChangeAnimatorState(_customAnimator.SwingR0State);
                }
                else if (moveInput == Vector2.zero)
                {
                    ChangeAnimatorState(_customAnimator.IdleState);
                }
                break;
            case "CharacterVisuals_SwingHandR_0":
                // TODO: Create input buffer for dodge/attack, choosing the last one.
                if (attackInput) TryBufferAttack();
                if (_attackActive)
                {
                    //_weaponSensor.CheckHits(swordSwingDmg, transform);
                }
                if (_comboWindowEnded)
                {
                    if (moveInput != Vector2.zero
                        && !_customAnimator.IsActiveState(_customAnimator.WalkState))
                    {
                        ChangeAnimatorState(_customAnimator.WalkState);
                    }
                }
                break;
            case "CharacterVisuals_SwingHandR_1":
                // TODO: Create input buffer for dodge/attack, choosing the last one.
                if (attackInput) TryBufferAttack();
                if (_attackActive)
                {
                    _weaponSensor.CheckHits(transform, swordSwingDmg, 1);
                }
                if (_comboWindowEnded)
                {
                    // Set correct animations.
                    if (moveInput != Vector2.zero
                        && !_customAnimator.IsActiveState(_customAnimator.WalkState))
                    {
                        ChangeAnimatorState(_customAnimator.WalkState);
                    }
                }
                break;
            default:
                // TODO: Since the custom animator first initializes its initial state at the end of its first Update,
                // TODO CONTD: this switch will likely default during first frame.
                Debug.LogError("Switch defaulted.", this);
                break;
        }
    }

    public void EquipWeapon(WeaponDefinition weapon)
    {
        // TODO:
        //animator.runtimeAnimatorController = weapon.animatorOverride;
        //actionDatabase.SetActions(weapon.availableActions);
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
        _customAnimator.OnWeaponActiveStart += OnWeaponActiveStart;
        _customAnimator.OnWeaponActiveEnd += OnWeaponActiveEnd;
        _customAnimator.OnWeaponAttackInputBufferingEnabled += OnAttackInputBufferingAllowed;
        _customAnimator.OnSwing0WeaponAttackInputBufferingDisabled += OnSwing0AttackBufferingEnd;
        _customAnimator.OnSwing1WeaponAttackInputBufferingDisabled += OnSwing1AttackBufferingEnd;
    }

    private void UnsubscribeToAnimationEvents()
    {
        _customAnimator.OnWeaponActiveStart -= OnWeaponActiveStart;
        _customAnimator.OnWeaponActiveEnd -= OnWeaponActiveEnd;
        _customAnimator.OnWeaponAttackInputBufferingEnabled -= OnAttackInputBufferingAllowed;
        _customAnimator.OnSwing0WeaponAttackInputBufferingDisabled -= OnSwing0AttackBufferingEnd;
        _customAnimator.OnSwing1WeaponAttackInputBufferingDisabled -= OnSwing1AttackBufferingEnd;
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
    private void ChangeAnimatorState(OldCustomAnimatorState newState)
    {
        _attackActive = false;
        _newAttackCanBeBuffered = false;
        _bufferedAttackInput = false;
        _comboWindowEnded = false;
        _customAnimator.RequestFixedTimeCrossfadeTo(newState);
    }

    //--------------------------------------------- Animation Event Callbacks

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

    public void OnSwing0AttackBufferingEnd()
    {
        if (_bufferedAttackInput)
        {
            // NOTE: Animator seems to only register transition to a new state during the internal animation update.
            ChangeAnimatorState(_customAnimator.SwingR1State);
        }
        else
        {
            _newAttackCanBeBuffered = false;
            _comboWindowEnded = true;
        }
    }

    public void OnSwing1AttackBufferingEnd()
    {
        if (_bufferedAttackInput)
        {
            // TODO: Calling this causes an error, likely because it is called in some other time than Update().
            // TODO: Animator seems to only register transition to a new state at a certain point during frame cycle. This is weird so you should write it down.
            ChangeAnimatorState(_customAnimator.SwingR0State);
        }
        else
        {
            _newAttackCanBeBuffered = false;
            _comboWindowEnded = true;
        }
    }
}
