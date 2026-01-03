using UnityEngine;

/// <summary>
/// Used to control the animations of the CharacterVisuals prefab.
/// </summary>
public class CharacterVisualsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CustomAnimator _customAnimator;

    private readonly CustomAnimatorStateInfo Animation_Idle = new CustomAnimatorStateInfo("CharacterVisuals_Idle");
    private readonly CustomAnimatorStateInfo Animation_Walk = new CustomAnimatorStateInfo("CharacterVisuals_Walk");
    private readonly CustomAnimatorStateInfo Animation_KnockBackBackward =
        new CustomAnimatorStateInfo(
            "CharacterVisuals_KnockBack_Backward",
            0.1f,
            0,
            new CustomAnimatorStateInfo("CharacterVisuals_Walk"),
            0.9f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ValidateAnimationState(Animation_Idle);
        ValidateAnimationState(Animation_Walk);
        ValidateAnimationState(Animation_KnockBackBackward);
    }


    private void ValidateAnimationState(CustomAnimatorStateInfo animState)
    {
        string stateName = animState.ThisAnimationName;

        if (string.IsNullOrWhiteSpace(stateName))
        {
            Debug.LogError("Animator state name is empty.", this);
        }

        int stateHash = Animator.StringToHash(stateName);

        // NOTE: Only checks layer 0. You might want to loop through all layers.
        Debug.Assert(_animator.HasState(0, stateHash), "Animator didn't have state: " + stateName, this);
    }

    // ------------------------ IsPlaying Methods

    internal bool IsPlaying_Idle()
    {
        return _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Idle.ThisAnimationHash);
    }

    internal bool IsPlaying_Walk()
    {
        return _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_Walk.ThisAnimationHash);
    }

    public bool IsPlaying_KnockBackBackward()
    {
        return _customAnimator.IsInOrIsTransitioningToAnimatorState(0, Animation_KnockBackBackward.ThisAnimationHash);
    }

    // ------------------------ Play Methods

    public void Play_KnockBackBackward()
    {
        _customAnimator.InterruptAnimationQueue(Animation_KnockBackBackward);
    }


    internal void Play_Walk()
    {
        _customAnimator.InterruptAnimationQueue(Animation_Walk);
    }



    internal void Play_Idle()
    {
        _customAnimator.InterruptAnimationQueue(Animation_Idle);
    }
}
