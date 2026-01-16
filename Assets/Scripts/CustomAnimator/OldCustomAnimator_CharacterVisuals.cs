using System;
using UnityEngine;

/// <summary>
/// Used to control the animations of the CharacterVisuals prefab.
/// </summary>
// TODO: Meh, it would be easier to just use an enum with all the states. Or perhaps it would make sense to have basic
// TODO CONTD: animator state logic in a non monobehavior script which are all collected into an array/list,
// TODO CONTD: while the animation event callbacks and other methods were in a monobehavior referenced by the states.
// TODO CONTD: Except how would the animator states reference the monobehaviors?
// Ugh, Unity's Animator sucks.
[Obsolete]
public class OldCustomAnimator_CharacterVisuals : OldCustomAnimator
{
    public event Action OnWeaponActiveStart;
    public event Action OnWeaponActiveEnd;
    public event Action OnWeaponAttackInputBufferingEnabled;
    public event Action OnSwing0WeaponAttackInputBufferingDisabled;
    public event Action OnSwing1WeaponAttackInputBufferingDisabled;
    /// <summary>
    /// Passes the delta XZ root motion as Vector2.
    /// </summary>
    public event Action<Vector2> OnRootXZMotion;

    private OldCustomAnimatorState _idleState = new();
    private OldCustomAnimatorState _walkState = new();
    private OldCustomAnimatorState _knockBackBackwardState = new();
    private OldCustomAnimatorState _swingR0State = new();
    private OldCustomAnimatorState _swingR1State = new();
    private OldCustomAnimatorState _attackJumpState = new();

    public OldCustomAnimatorState IdleState => _idleState;
    public OldCustomAnimatorState WalkState => _walkState;
    public OldCustomAnimatorState KnockBackBackwardState => _knockBackBackwardState;
    public OldCustomAnimatorState SwingR0State => _swingR0State;
    public OldCustomAnimatorState SwingR1State => _swingR1State;
    public OldCustomAnimatorState AttackJumpState => _attackJumpState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        // Initialize states:
        _idleState.InitializeState("CharacterVisuals_Idle");
        _walkState.InitializeState("CharacterVisuals_Walk");
        _knockBackBackwardState.InitializeState(
            "CharacterVisuals_KnockBack_Backward",
            0.1f,
            _idleState);
        _swingR0State.InitializeState(
            "CharacterVisuals_SwingHandR_0",
            0.1f,
            _idleState);
        _swingR1State.InitializeState(
            "CharacterVisuals_SwingHandR_1",
            0.1f,
            _idleState);
        _attackJumpState.InitializeState(
            "CharacterVisuals_Attack_Jump",
            0.1f,
            _idleState);

        ValidateAnimationStates(
            _idleState,
            _walkState,
            _knockBackBackwardState,
            _swingR0State,
            _swingR1State,
            _attackJumpState);

        _initialState = _idleState;

        base.Start();

        
    }

    //private void OnAnimatorMove()
    //{
    //    Vector3 deltaPos = _animator.deltaPosition;
    //    Quaternion deltaRot = _animator.deltaRotation;

    //    Vector2 deltaPosXZ = new Vector2(deltaPos.x, deltaPos.z);
    //    OnRootXZMotion?.Invoke(deltaPosXZ);

    //    //Debug.Log("delta pos: " + deltaPos);
    //    transform.localPosition += new Vector3(0, deltaPos.y, 0);
    //    transform.localRotation *= deltaRot;
    //}

    // ------------------------------------------------ Animation Events
    // NOTE: Events are triggered on both current and next Animator states during transitions. We can prevent it here unless
    // NOTE C: The transition happens to the same state we are already in (in which case animation events will unfortunately
    // NOTE C: trigger in both the current and next animations).

    /// <summary>
    /// Meant for enabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_0_WeaponActiveStart()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR0State))
            OnWeaponActiveStart?.Invoke();
    }

    /// <summary>
    /// Meant for disabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_0_WeaponActiveEnd()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR0State))
            OnWeaponActiveEnd?.Invoke();
    }

    /// <summary>
    /// Meant for starting combo input window.
    /// </summary>
    public void SwingHandR_0_AttackInputBufferingEnabled()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR0State))
            OnWeaponAttackInputBufferingEnabled?.Invoke();
    }

    /// <summary>
    /// Meant for ending combo input window.
    /// </summary>
    public void SwingHandR_0_Swing0AttackInputBufferingDisabled()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR0State))
            OnSwing0WeaponAttackInputBufferingDisabled?.Invoke();
    }

    /// <summary>
    /// Meant for enabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_1_WeaponActiveStart()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR1State))
            OnWeaponActiveStart?.Invoke();
    }

    /// <summary>
    /// Meant for disabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_1_WeaponActiveEnd()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR1State))
            OnWeaponActiveEnd?.Invoke();
    }

    /// <summary>
    /// Meant for starting combo input window.
    /// </summary>
    public void SwingHandR_1_AttackInputBufferingEnabled()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR1State))
            OnWeaponAttackInputBufferingEnabled?.Invoke();
    }

    /// <summary>
    /// Meant for ending combo input window.
    /// </summary>
    public void SwingHandR_1_Swing2AttackInputBufferingDisabled()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_swingR1State))
            OnSwing1WeaponAttackInputBufferingDisabled?.Invoke();
    }

    /// <summary>
    /// Meant for enabling hit detection of the attack.
    /// </summary>
    public void AttackJump_WeaponActiveStart()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_attackJumpState))
            OnWeaponActiveStart?.Invoke();
    }

    /// <summary>
    /// Meant for disabling hit detection of the attack.
    /// </summary>
    public void AttackJump_WeaponActiveEnd()
    {
        if (IsTransitioningToSameState())
        {
            Debug.LogWarning("Animation event of the previous state was likely triggered.", this);
        }

        if (IsActiveState(_attackJumpState))
            OnWeaponActiveEnd?.Invoke();
    }
}
