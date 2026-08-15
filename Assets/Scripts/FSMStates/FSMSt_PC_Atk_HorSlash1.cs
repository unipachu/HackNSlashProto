using System;
using UnityEngine;

public class FsmSt_Pc_Atk_HorSlash1 : MonoBehaviour, IFsmSt{
    /// <summary>
    /// string is the unique id of the animation event.
    /// </summary>
    event Action<string> animEvent;
    
    [SerializeField] Pc pc;

    AtkPhase atkPhase = AtkPhase.Windup;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotAllowed = false;
    float recoveryMotInterpTimer = 0;
    int shortHash_atk_HorSlash1_Windup = Animator.StringToHash("PC_Atk_HorSlash1_Windup");
    int shortHash_atk_HorSlash1_Impact = Animator.StringToHash("PC_Atk_HorSlash1_Impact");
    int shortHash_atk_HorSlash1_Recovery = Animator.StringToHash("PC_Atk_HorSlash1_Recovery");
    // Animation event ids
    const string Windup_Finished = "Windup_Finished";
    const string Impact_RotationAllowed = "Impact_RotationAllowed";
    const string Impact_RotationDisallowed = "Impact_RotationDisallowed";
    const string Impact_HitDealerActivated = "Impact_HitDealerActivated";
    const string Impact_HitDealerDeactivated = "Impact_HitDealerDeactivated";
    const string Impact_ComboAllowed = "Impact_ComboAllowed";
    const string Impact_ComboDisallowed = "Impact_ComboDisallowed";
    const string Impact_Finished = "Impact_Finished";
    const string Recovery_DodgeAllowed = "Recovery_DodgeAllowed";
    const string Recovery_Finished = "Recovery_Finished";
    ActAnimEvent[] animEvents_Windup = new ActAnimEvent[] {
        new(26, 26, Windup_Finished)
    };
    ActAnimEvent[] animEvents_Impact = VisUtils.CreateAnimEvents(
        26,
        (0, Impact_RotationAllowed),
        (4, Impact_RotationDisallowed),
        (9, Impact_HitDealerActivated),
        (18, Impact_HitDealerDeactivated),
        (22, Impact_ComboAllowed),
        (24, Impact_ComboDisallowed),
        (26, Impact_Finished)
    );
    ActAnimEvent[] animEvents_Recovery = VisUtils.CreateAnimEvents(
        18,
        (6, Recovery_DodgeAllowed),
        (18, Recovery_Finished)
    );


    void OnEnable() {
        animEvent += OnAnimEvent;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed += OnImpact_ComboAllowed;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_Finished += OnImpact_Finished;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationAllowed += OnImpact_RotationAllowed;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationDisallowed += OnImpact_RotationDisallowed;
        //// Recovery
        //pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished += OnRecovery_Finished;
        //pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed += OnRecovery_DodgeAllowed;
        //// Windup
        //pc.visComponents.animEvents.Atk_HorSlash1_Windup_Finished += OnWindup_Finished;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_Finished -= OnImpact_Finished;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationAllowed -= OnImpact_RotationAllowed;
        //pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationDisallowed -= OnImpact_RotationDisallowed;
        //// Recovery
        //pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished -= OnRecovery_Finished;
        //pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed -= OnRecovery_DodgeAllowed;
        //// Windup
        //pc.visComponents.animEvents.Atk_HorSlash1_Windup_Finished -= OnWindup_Finished;
    }

    public void Enter(IFsmSt previousState) {
        atkPhase = AtkPhase.Windup;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotAllowed = false;
        recoveryMotInterpTimer = 0;
        pc.inputBuffer.Clear();
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.visComponents.anim,
            "PC_Atk_HorSlash1_Windup",
            shortHash_atk_HorSlash1_Windup,
            0,
            animEvents_Windup,
            false,
            animEvent,
            0.1f
        );
    }

    public void Exit(){
        // TODO: Deactivate HitDealers.
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        switch (atkPhase){
            case AtkPhase.Windup:
                pc.charCtrlMov.UpdateMov(
                    Vector2.zero,
                    pc.AnimationDeltaMovement,
                    0,
                    0
                );
                return;
            case AtkPhase.Impact:
                float angSpd = 0;
                if(impactInputRotAllowed)
                    angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd
                );
                if (comboAllowed) {
                    if(pc.inputBuffer.TryConsumeInput("atk1")) {
                        pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash2);
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotInterpTimer / 0.2f);
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_LinAcc,
                    pc.Data.st_Walk_MaxAngSpd * interpValue);
                if (dodgeAllowed) {
                    if (pc.inputBuffer.TryConsumeInput("dodge")) {
                        pc.fsm.SwitchSt(pc.fsmSts.dodge);
                    }
                }
                return;
            default:
                Debug.LogError("Switch defaulted.", this);
                return;
        }
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }


    // ----------------------
    // Animation event
    // ----------------------

    private void OnAnimEvent(string id) {
        Dbg.inst.Log($"Event called: {id}.");
        switch (id) {
            case Windup_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.visComponents.anim,
                    "PC_Atk_HorSlash1_Impact",
                    shortHash_atk_HorSlash1_Impact,
                    0,
                    animEvents_Impact,
                    false,
                    animEvent
                );
                atkPhase = AtkPhase.Impact;
                break;
            case Impact_RotationAllowed:
                impactInputRotAllowed = true;
                break;
            case Impact_RotationDisallowed:
                impactInputRotAllowed = false;
                break;
            case Impact_HitDealerActivated:
                // TODO
                break;
            case Impact_HitDealerDeactivated:
                // TODO
                break;
            case Impact_ComboAllowed:
                comboAllowed = true;
                break;
            case Impact_ComboDisallowed:
                comboAllowed = false;
                break;
            case Impact_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.visComponents.anim,
                    "PC_Atk_HorSlash1_Recovery",
                    shortHash_atk_HorSlash1_Recovery,
                    0,
                    animEvents_Recovery,
                    false,
                    animEvent
                );
                atkPhase = AtkPhase.Recovery;
                break;
            case Recovery_DodgeAllowed:
                dodgeAllowed = true;
                break;
            case Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }

    //// ----------------------
    //// Recovery Animation callbacks
    //// ----------------------

    //void OnRecovery_DodgeAllowed() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    dodgeAllowed = true;
    //}

    //void OnRecovery_Finished() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    pc.fSM.SwitchSt(pc.fSMStates.idle);
    //}

    //// ----------------------
    //// Impact Animation callbacks
    //// ----------------------

    //void OnImpact_HitDealerDeactivated() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    // TODO
    //}

    //void OnImpact_HitDealerActivated() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    // TODO
    //}

    //void OnImpact_Finished() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    pc.visComponents.anims.Play_Atk_HorSlash1_Recovery();
    //    atkPhase = AtkPhase.Recovery;
    //}

    //void OnImpact_ComboDisallowed() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    comboAllowed = false;
    //}

    //void OnImpact_ComboAllowed() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    comboAllowed = true;
    //}

    //void OnImpact_RotationAllowed() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    impactInputRotAllowed = true;
    //}

    //void OnImpact_RotationDisallowed() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    impactInputRotAllowed = false;
    //}

    //// ----------------------
    //// Windup Animation callbacks
    //// ----------------------

    //void OnWindup_Finished() {
    //    if (pc.fSM.CurSt != (IFsmSt)this)
    //        return;
    //    pc.visComponents.anims.Play_Atk_HorSlash1_Impact();
    //    atkPhase = AtkPhase.Impact;
    //}
}
