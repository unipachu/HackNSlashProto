using System;
using UnityEngine;

/// <summary>
/// Used to control the animations of the CharacterVisuals prefab.
/// </summary>
public class CharacterVisualsAnimationController : MonoBehaviour
{
    [SerializeField] private CustomAnimator _customAnimator;
    // TODO: This in here?
    [SerializeField] private WeaponColliderHitSensor _weaponSensor;

    // Make each animation state its own class.
    private readonly CustomAnimatorStateInfo Animation_Idle = new CustomAnimatorStateInfo("CharacterVisuals_Idle");
    private readonly CustomAnimatorStateInfo Animation_Walk = new CustomAnimatorStateInfo("CharacterVisuals_Walk");
    private readonly CustomAnimatorStateInfo Animation_KnockBackBackward =
        new CustomAnimatorStateInfo(
            "CharacterVisuals_KnockBack_Backward",
            0.1f,
            0,
            new CustomAnimatorStateInfo("CharacterVisuals_Walk"),
            0.9f);
    private readonly CustomAnimatorStateInfo Animation_Swing1 = new CustomAnimatorStateInfo(
        "CharacterVisuals_SwingHandR_1",
        0.1f,
        0.9f,
        new CustomAnimatorStateInfo("CharacterVisuals_Idle"),
        0.9f);
    private readonly CustomAnimatorStateInfo Animation_Swing2 = new CustomAnimatorStateInfo(
    "CharacterVisuals_SwingHandR_2",
    0.1f,
    0.9f,
    new CustomAnimatorStateInfo("CharacterVisuals_Idle"),
    0.9f);

    // TODO: Remove this and make a better system.
    private bool _attackActive = false;
    private bool _newAttackCanBeBuffered = false;
    private bool _bufferedAttackInput = false;
    private bool _comboWindowEnded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ValidateAnimationState(Animation_Idle);
        ValidateAnimationState(Animation_Walk);
        ValidateAnimationState(Animation_KnockBackBackward);
    }

    private void Update()
    {
        if(_attackActive)
        {
            // TODO: Find correct damage value.
            _weaponSensor.CheckHits(1, transform);
        }
    }

    /// <summary>
    /// Checks that animation state actually exists in the animator.
    /// </summary>
    /// <param name="animState"></param>
    private void ValidateAnimationState(CustomAnimatorStateInfo animState)
    {
        string stateName = animState.ThisAnimationName;

        if (string.IsNullOrWhiteSpace(stateName))
        {
            Debug.LogError("Animator state name is empty.", this);
        }

        int stateHash = Animator.StringToHash(stateName);

        // NOTE: Only checks layer 0. You might want to loop through all layers.
        Debug.Assert(_customAnimator.HasState(0, stateHash), "Animator didn't have state: " + stateName, this);
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
    private void EnterNewStateAndInitialize(CustomAnimatorStateInfo newState)
    {
        _attackActive = false;
        _newAttackCanBeBuffered = false;
        _bufferedAttackInput = false;
        _comboWindowEnded = false;
        _customAnimator.InterruptAnimationQueue(newState);
    }

    // ------------------------ IsPlaying Methods

    public bool IsPlaying_Idle()
    {
        return _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Idle.ThisAnimationHash);
    }

    public bool IsPlaying_Walk()
    {
        return _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Walk.ThisAnimationHash);
    }

    public bool IsPlaying_KnockBackBackward()
    {
        return _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_KnockBackBackward.ThisAnimationHash);
    }

    public bool IsPlaying_SwingAttack()
    {
        return _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Swing1.ThisAnimationHash)
            || _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Swing2.ThisAnimationHash);
    }

    /// <summary>
    /// After this can transition to other states (than new attack).
    /// </summary>
    /// <returns></returns>
    public bool ComboWindowEnded()
    {
        return _comboWindowEnded;
    }

    // ------------------------ Play Methods

    public void Play_KnockBackBackward()
    {
        EnterNewStateAndInitialize(Animation_KnockBackBackward);
    }

    public void Play_Walk()
    {
        EnterNewStateAndInitialize(Animation_Walk);
    }

    public void Play_Idle()
    {
        EnterNewStateAndInitialize(Animation_Idle);
    }

    public void Play_SwingAttack()
    {
        EnterNewStateAndInitialize(Animation_Swing1);
    }

    //public void Play_SwingAttacks()
    //{
    //    EnterNewStateInitializations();

    //    // TODO: You'll want on enter and on exit functionality to the animations.
    //    if (!_customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Swing1.ThisAnimationHash)
    //        && !_customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Swing2.ThisAnimationHash))
    //        _customAnimator.InterruptAnimationQueue(Animation_Swing1);
    //    else if (_customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Swing1.ThisAnimationHash))
    //        _customAnimator.InterruptAnimationQueue(Animation_Swing2);
    //}

    // ---------------------------- ANIMATION EVENTS

    public void OnSwing1Start()
    {
        // TODO: Why is this here?
    }

    public void OnSwing1ActiveStart()
    {
        _weaponSensor.BeginAttack();
        _attackActive = true;
    }

    public void OnSwing1ActiveEnd()
    {
        _attackActive = false;
    }

    public void OnSwing1Halfway()
    {
        _newAttackCanBeBuffered = true;
    }

    public void OnSwing1ComboWindowEnd()
    {
        if (_bufferedAttackInput)
        {
            // TODO: Calling this causes an error, likely because it is called in some other time than Update().
            // TODO: Animator seems to only register transition to a new state at a certain point during frame cycle. This is weird so you should write it down.
            EnterNewStateAndInitialize(Animation_Swing2);
        }
        else
        {
            _newAttackCanBeBuffered = false;
            _comboWindowEnded = true;
        }
    }

}
