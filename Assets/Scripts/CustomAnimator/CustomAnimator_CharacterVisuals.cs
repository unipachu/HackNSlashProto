using UnityEngine;

/// <summary>
/// Used to control the animations of the CharacterVisuals prefab.
/// </summary>
// TODO: Meh, it would be easier to just use an enum with all the states. Or perhaps it would make sense to have basic
// TODO CONTD: animator state logic in a non monobehavior script which are all collected into an array/list,
// TODO CONTD: while the animation event callbacks and other methods were in a monobehavior referenced by the states.
// TODO CONTD: Except how would the animator states reference the monobehaviors?
// Ugh, Unity's Animator sucks.
public class CustomAnimator_CharacterVisuals : CustomAnimator
{
    [Header("CharacterVisuals States")]
    [SerializeField] private CustomAnimatorState_CharacterVisuals_Idle _idleState;
    [SerializeField] private CustomAnimatorState_CharacterVisuals_Walk _walkState;
    [SerializeField] private CustomAnimatorState_CharacterVisuals_KnockBack_Backward _knockBackBackwardState;
    [SerializeField] private CustomAnimatorState_CharacterVisuals_SwingHandR_1 _swingR1State;
    [SerializeField] private CustomAnimatorState_CharacterVisuals_SwingHandR_2 _swingR2State;

    public CustomAnimatorState_CharacterVisuals_Idle IdleState { get => _idleState; }
    public CustomAnimatorState_CharacterVisuals_Walk WalkState { get => _walkState; }
    public CustomAnimatorState_CharacterVisuals_KnockBack_Backward KnockBackBackwardState { get => _knockBackBackwardState; }
    public CustomAnimatorState_CharacterVisuals_SwingHandR_1 SwingR1State { get => _swingR1State; }
    public CustomAnimatorState_CharacterVisuals_SwingHandR_2 SwingR2State { get => _swingR2State; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        ValidateAnimationStates(
            IdleState, 
            WalkState, 
            KnockBackBackwardState, 
            SwingR1State, 
            SwingR2State);
    }
}
