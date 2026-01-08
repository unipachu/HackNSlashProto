using UnityEngine;

public class CustomAnimatorState_CharacterVisuals_KnockBack_Backward : CustomAnimatorState
{
    [SerializeField] private CustomAnimatorState_CharacterVisuals_Idle _idleState;

    protected override void OnAwake()
    {
        InitializeState(
            "CharacterVisuals_KnockBack_Backward",
            0.1f,
            _idleState);
    }

    //public CustomAnimatorState_CharacterVisuals_KnockBack_Backward()
    //    : base(
    //        animationName: "CharacterVisuals_KnockBackBackward",
    //        crossFadeDurationToThis: 0.1f,
    //        fallbackState: null,
    //        fallbackTransitionPrecent: 1f)
    //{
    //}
}
