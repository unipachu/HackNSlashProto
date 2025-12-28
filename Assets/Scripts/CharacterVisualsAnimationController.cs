using UnityEngine;

/// <summary>
/// Used to control the animations of the CharacterVisuals prefab.
/// </summary>
// TODO: Player and the enemies should have their own animator controllers, and so this class should eventually be removed..
public class CharacterVisualsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // NOTE: Hashed animator state names. These NEED to match the ones in the animator. Make sure to assert these.
    private static readonly int _animHash_Idle = Animator.StringToHash("CharacterVisuals_Idle");
    private static readonly int _animHash_Walk = Animator.StringToHash("CharacterVisuals_Walk");
    private static readonly int _animHash_KnockBackBackward = Animator.StringToHash("CharacterVisuals_KnockBack_Backward");

    private static readonly int _triggerHash_Idle = Animator.StringToHash("Idle");
    private static readonly int _triggerHash_Walk = Animator.StringToHash("Walk");
    private static readonly int _triggerHash_KnockBackBackward = Animator.StringToHash("KnockBack_Backward");


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Assert every hashed animation state here!
        Debug.Assert(_animator.HasState(0, _animHash_Idle), "Animator state didn't exist!");
        Debug.Assert(_animator.HasState(0, _animHash_Walk), "Animator state didn't exist!");
        Debug.Assert(_animator.HasState(0, _animHash_KnockBackBackward), "Animator state didn't exist!");

        Debug.Assert(GeneralUtils.HasTrigger(_animator, _triggerHash_Idle), "Animator trigger didn't exist!");
        Debug.Assert(GeneralUtils.HasTrigger(_animator, _triggerHash_Walk), "Animator trigger didn't exist!");
        Debug.Assert(GeneralUtils.HasTrigger(_animator, _triggerHash_KnockBackBackward), "Animator trigger didn't exist!");
    }

    public bool IsPlaying_Idle()
    {
        return GeneralUtils.IsInAnimationState(_animator, 0, _animHash_Idle);
    }

    public bool IsPlaying_Walk()
    {
        return GeneralUtils.IsInAnimationState(_animator, 0, _animHash_Walk);
    }

    public bool IsPlaying_KnockBackBackward()
    {
        return GeneralUtils.IsInAnimationState(_animator, 0, _animHash_KnockBackBackward);
    }

    public void Play_Idle()
    {
        _animator.SetTrigger(_triggerHash_Idle);
    }

    public void Play_Walk()
    {
        _animator.SetTrigger(_triggerHash_Walk);
    }

    public void Play_KnockBackBackward()
    {
        _animator.SetTrigger(_triggerHash_KnockBackBackward);
    }
}
