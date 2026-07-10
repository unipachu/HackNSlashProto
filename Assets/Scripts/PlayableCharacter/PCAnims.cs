using UnityEngine;

// TODO: Use animation hashes instead?

/// <summary>
/// USe this to play capsule character animations.
/// </summary>
public class PCAnims : MonoBehaviour
{
    [SerializeField] private Animator animator;

    //[HideInInspector] public string idle = "ANS_CapsuleCharacter_Idle";
    //[HideInInspector] public string attack_RHandVerticalSlam = "ANS_CapsuleCharacter_Attack_RHandVerticalSlam";
    //[HideInInspector] public string attack_RHandJumpVerticalSlam = "ANS_CapsuleCharacter_Attack_RHandJumpVerticalSlam";
    //[HideInInspector] public string walk = "ANS_CapsuleCharacter_Walk";

    public void CrossFade(string animName, float nrmTransitionDur, int layerI, float nrmTimeOffset = 0)
    {
        // NOTE: We assert short name hash (hash without layer name)!
        Debug.Assert(
            !CurrentAnimEquals(animName),
            "Tried to transition to the animation we're already playing: " + animName,
            this
        );

        animator.CrossFade(
        animName,
        nrmTransitionDur,
        layerI,
        nrmTimeOffset
        );
    }

    public void PlayAnim(string animName, int layerI, float nrmTimeOffset = 0)
    {
        // NOTE: We assert short name hash (hash without layer name)!
        Debug.Assert(
            !CurrentAnimEquals(animName),
            "Tried to transition to the animation we're already playing: " + animName,
            this
        );

        animator.Play(
            animName,
            layerI,
            nrmTimeOffset
        );
    }

    /// <summary>
    /// NOTE: Currently everything is expected to work on animator layer 0.
    /// </summary>
    private bool CurrentAnimEquals(string animShortName)
    {
        // NOTE: If we are in transition when starting a new transition, the previous current
        // NOTE C: and next animations will pause, so it's actually safe to start a transition to
        // NOTE C: either of the previous animations, but if the previous transition had finished,
        // NOTE C: then erroneous animation events could be triggered, so to catch these situations before they can happen,
        // NOTE C: we also compare the previous animation's next animation to the newest animation.
        if (animator.IsInTransition(0))
        {
            return animator.GetNextAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animShortName);
        }
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animShortName);
    }

    // -----------------------
    // Play animation methods
    // -----------------------

    public void Play_Attack_RHandVerticalSlam(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "ANS_CapsuleCharacter_Attack_RHandVerticalSlam",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Attack_RHandJumpVerticalSlam(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Atk_JumpVerSlam",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Atk_HorSlash1_Impact(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Atk_HorSlash1_Impact",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Atk_HorSlash1_Recovery(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Atk_HorSlash1_Recovery",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Atk_HorSlash1_Windup(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Atk_HorSlash1_Windup",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Atk_HorSlash2_Impact(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Atk_HorSlash2_Impact",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Atk_HorSlash2_Recovery(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Atk_HorSlash2_Recovery",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Idle(float normalizedTransitionDuration = 0.3f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Idle",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_TPose(float normalizedTransitionDuration = 0.1f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_TPose",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }

    public void Play_Walk(float normalizedTransitionDuration = 0.3f, int layerIndex = 0, float normalizedTimeOffset = 0)
    {
        CrossFade(
            "PC_Walk",
            normalizedTransitionDuration,
            layerIndex,
            normalizedTimeOffset
        );
    }


}
