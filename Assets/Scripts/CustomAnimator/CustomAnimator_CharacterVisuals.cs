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

    //// Make each animation state its own class.
    //private readonly CustomAnimatorStateInfo Animation_Idle = new CustomAnimatorStateInfo("CharacterVisuals_Idle");
    //private readonly CustomAnimatorStateInfo Animation_Walk = new CustomAnimatorStateInfo("CharacterVisuals_Walk");
    //private readonly CustomAnimatorStateInfo Animation_KnockBackBackward =
    //    new CustomAnimatorStateInfo(
    //        "CharacterVisuals_KnockBack_Backward",
    //        0.1f,
    //        0,
    //        new CustomAnimatorStateInfo("CharacterVisuals_Walk"),
    //        0.9f);
    //private readonly CustomAnimatorStateInfo Animation_Swing1 = new CustomAnimatorStateInfo(
    //    "CharacterVisuals_SwingHandR_1",
    //    0.1f,
    //    0.9f,
    //    new CustomAnimatorStateInfo("CharacterVisuals_Idle"),
    //    0.9f);
    //private readonly CustomAnimatorStateInfo Animation_Swing2 = new CustomAnimatorStateInfo(
    //"CharacterVisuals_SwingHandR_2",
    //0.1f,
    //0.9f,
    //new CustomAnimatorStateInfo("CharacterVisuals_Idle"),
    //0.9f);
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


    //public CustomAnimatorState_CharacterVisuals_Idle IdleState { get; private set; }
    //public CustomAnimatorState_CharacterVisuals_Walk WalkState { get; private set; }
    //public CustomAnimatorState_CharacterVisuals_KnockBack_Backward KnockBackBackwardState { get; private set; }
    //public CustomAnimatorState_CharacterVisuals_SwingHandR_1 SwingR1State { get; private set; }
    //public CustomAnimatorState_CharacterVisuals_SwingHandR_2 SwingR2State { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ValidateAnimationStates(
            IdleState, 
            WalkState, 
            KnockBackBackwardState, 
            SwingR1State, 
            SwingR2State);
    }


    // ------------------------ IsPlaying Methods

    //public bool IsPlaying_Idle()
    //{
    //    return IsInOrIsTransitioningToState(0, IdleState.ThisAnimationHash);
    //}

    //public bool IsPlaying_Walk()
    //{
    //    return IsInOrIsTransitioningToState(0, WalkState.ThisAnimationHash);
    //}

    //public bool IsPlaying_KnockBackBackward()
    //{
    //    return IsInOrIsTransitioningToState(0, KnockBackBackwardState.ThisAnimationHash);
    //}

    //public bool IsPlaying_SwingAttack()
    //{
    //    return IsInOrIsTransitioningToState(0, SwingR1State.ThisAnimationHash)
    //        || IsInOrIsTransitioningToState(0, SwingR2State.ThisAnimationHash);
    //}

    // ------------------------ Play Methods

    //public void Play_KnockBackBackward()
    //{
    //    EnterNewStateAndInitialize(Animation_KnockBackBackward);
    //}

    //public void Play_Walk()
    //{
    //    EnterNewStateAndInitialize(Animation_Walk);
    //}

    //public void Play_Idle()
    //{
    //    EnterNewStateAndInitialize(Animation_Idle);
    //}

    //public void Play_SwingAttack()
    //{
    //    EnterNewStateAndInitialize(Animation_Swing1);
    //}
}
