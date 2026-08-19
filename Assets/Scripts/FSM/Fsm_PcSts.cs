using UnityEngine;

/// <summary>
/// Playable character FSM states.
/// </summary>
// TODO: Instead of setting references in the inspector, you could make these non-Monobehavior and just create new.
public class Fsm_PcSts : MonoBehaviour{
    public FsmSt_Cc_Atk_FlyingAtk atk_FlyingAtk;
    public FsmSt_Cc_Atk_HorSlash1 atk_HorSlash1;
    public FsmSt_Cc_Atk_HorSlash2 atk_HorSlash2;
    public FsmSt_Cc_Atk_HorSlash3 atk_HorSlash3;
    public FsmSt_Cc_Atk_Jump atk_Jump;
    public FsmSt_Cc_Dodge dodge;
    public FsmSt_Cc_Falling falling;
    public FsmSt_Cc_FallLanding fallLanding;
    public FsmSt_Cc_Idle idle;
    public FsmSt_Cc_Knockback_Weak knockback_Weak;
    public FsmSt_Cc_Walk walk;
}
