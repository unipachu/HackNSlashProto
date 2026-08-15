using System;
using UnityEngine;

// TODO: Use animation hashes instead?

/// <summary>
/// USe this to play capsule character animations.
/// </summary>
[Obsolete]
public class PcAnims : MonoBehaviour{
    [SerializeField] Animator animator;

    public void CrossFade(string animName, float nrmTransitionDur, int layerI, float nrmTimeOffset = 0){
        // NOTE: We assert short name hash (hash without layer name)!
        Dbg.inst.Assert(
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

    public void PlayAnim(string animName, int layerI, float nrmTimeOffset = 0){
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
    bool CurrentAnimEquals(string animShortName){
        // NOTE: If we are in transition when starting a new transition, the previous current
        // NOTE C: and next animations will pause, so it's actually safe to start a transition to
        // NOTE C: either of the previous animations, but if the previous transition had finished,
        // NOTE C: then erroneous animation events could be triggered, so to catch these situations
        // NOTE C: before they can happen, we also compare the previous animation's next animation
        // NOTE C: to the newest animation.
        if (animator.IsInTransition(0))
            return animator.GetNextAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animShortName);
        return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animShortName);
    }

    // -----------------------
    // Play animation methods
    // -----------------------

    public void Play_Atk_FlyingAtk_Impact(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_FlyingAtk_Impact",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_FlyingAtk_Recovery(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_FlyingAtk_Recovery",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_FlyingAtk_Windup(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_FlyingAtk_Windup",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_RHandVerSlam(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "ANS_CapsuleCharacter_Attack_RHandVerticalSlam",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_RHandJumpVerSlam(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_JumpVerSlam",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_HorSlash1_Impact(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_HorSlash1_Impact",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_HorSlash1_Recovery(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_HorSlash1_Recovery",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    //public void Play_Atk_HorSlash1_Windup(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
    //    CrossFade(
    //        "PC_Atk_HorSlash1_Windup",
    //        nrmTransDur,
    //        layerI,
    //        nrmTimeOfs
    //    );
    //}

    public void Play_Atk_HorSlash2_Impact(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_HorSlash2_Impact",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_HorSlash2_Recovery(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_HorSlash2_Recovery",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Atk_HorSlash3_Impact(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Atk_HorSlash3_Impact",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Dodge(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Dodge",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Idle(float nrmTransDur = 0.3f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Idle",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_TPose(float nrmTransDur = 0.1f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_TPose",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }

    public void Play_Walk(float nrmTransDur = 0.3f, int layerI = 0, float nrmTimeOfs = 0){
        CrossFade(
            "PC_Walk",
            nrmTransDur,
            layerI,
            nrmTimeOfs
        );
    }
}
