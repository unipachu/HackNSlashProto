using System;
using UnityEngine;

public class CustomAnimatorState_CharacterVisuals_SwingHandR_2 : CustomAnimatorState
{
    [SerializeField] private CustomAnimatorState_CharacterVisuals_Idle _idleState;

    public event Action OnWeaponActiveStart;
    public event Action OnWeaponActiveEnd;
    public event Action OnWeaponAttackInputBufferingEnabled;
    public event Action OnWeaponAttackInputBufferingDisabled;

    protected override void OnAwake()
    {
        InitializeState(
            "CharacterVisuals_SwingHandR_2",
            0.1f,
            _idleState);
    }

    /// <summary>
    /// Meant for enabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_2_OnWeaponActiveStart()
    {
        OnWeaponActiveStart?.Invoke();
    }

    /// <summary>
    /// Meant for disabling hit detection of the attack.
    /// </summary>
    public void SwingHandR_2_OnWeaponActiveEnd()
    {
        OnWeaponActiveEnd?.Invoke();
    }

    /// <summary>
    /// Meant for starting combo input window.
    /// </summary>
    public void SwingHandR_2_OnAttackInputBufferingEnabled()
    {
        OnWeaponAttackInputBufferingEnabled?.Invoke();
    }

    /// <summary>
    /// Meant for ending combo input window.
    /// </summary>
    public void SwingHandR_2_OnAttackInputBufferingDisabled()
    {
        OnWeaponAttackInputBufferingDisabled?.Invoke();
    }
}
