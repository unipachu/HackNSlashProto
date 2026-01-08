using UnityEngine;
using System;

public class CustomAnimatorState_CharacterVisuals_SwingHandR_1 : CustomAnimatorState
{
    [SerializeField] private CustomAnimatorState_CharacterVisuals_Idle _idleState;

    public event Action OnWeaponActiveStart;
    public event Action OnWeaponActiveEnd;
    public event Action OnWeaponAttackInputBufferingEnabled;
    public event Action OnWeaponAttackInputBufferingDisabled;

    protected override void OnAwake()
    {
        InitializeState(
            "CharacterVisuals_SwingHandR_1",
            0.1f,
            _idleState);
    }

    /// <summary>
    /// Meant for enabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_1_OnWeaponActiveStart()
    {
        OnWeaponActiveStart?.Invoke();
    }

    /// <summary>
    /// Meant for disabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_1_OnWeaponActiveEnd()
    {
        OnWeaponActiveEnd?.Invoke();
    }

    /// <summary>
    /// Meant for starting combo input window.
    /// </summary>
    public void SwingHandR_1_OnAttackInputBufferingEnabled()
    {
        OnWeaponAttackInputBufferingEnabled?.Invoke();
    }

    /// <summary>
    /// Meant for ending combo input window.
    /// </summary>
    public void SwingHandR_1_OnAttackInputBufferingDisabled()
    {
        OnWeaponAttackInputBufferingDisabled?.Invoke();
    }

    //public CustomAnimatorState_CharacterVisuals_SwingHandR_1(
    //    CustomAnimatorState_CharacterVisuals_Idle idleState)
    //    : base(
    //        animationName: "CharacterVisuals_SwingHandR_1",
    //        crossFadeDurationToThis: 0.1f,
    //        fallbackState: idleState)
    //{
    //}
}
