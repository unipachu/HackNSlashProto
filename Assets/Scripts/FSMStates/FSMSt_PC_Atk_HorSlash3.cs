using UnityEngine;

public class FsmSt_Pc_Atk_HorSlash3 : MonoBehaviour, IFsmSt {
    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Impact;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotationAllowed = false;
    float recoveryMotionInterpTimer = 0;

    void OnEnable() {
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_ComboAllowed += OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_Finished += OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_RotationAllowed += OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_RotationDisallowed += OnImpact_RotationDisallowed;
        // Recovery
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished += OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed += OnRecovery_DodgeAllowed;
    }

    void OnDisable() {
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_Finished -= OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_RotationAllowed -= OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash3_Impact_RotationDisallowed -= OnImpact_RotationDisallowed;
        // Recovery
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished -= OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed -= OnRecovery_DodgeAllowed;
    }

    public void Enter(IFsmSt previousState) {
        attackPhase = AtkPhase.Impact;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotationAllowed = false;
        recoveryMotionInterpTimer = 0;
        pc.inputBuffer.Clear();
        pc.visComponents.anims.Play_Atk_HorSlash3_Impact();
    }

    public void Exit() {
        // TODO: Deactivate HitDealers.
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        switch (attackPhase) {
            case AtkPhase.Impact:
                float angSpd = 0;
                if (impactInputRotationAllowed)
                    angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.locomotion.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd);
                if (comboAllowed) {
                    //if (pc.inputBuffer.ConsumeInput("atk1"))
                    //{
                    //    pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash2);
                    //}
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotionInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotionInterpTimer / 0.2f);
                pc.locomotion.UpdateMov(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_LinAcc,
                    pc.Data.st_Walk_MaxAngSpd * interpValue);
                if (dodgeAllowed) {
                    // If input buffer has dodge, then transition to dodge state.
                }
                return;
            default:
                Debug.LogError("Switch defaulted.", this);
                return;
        }
    }

    // ----------------------
    // Recovery Animation callbacks
    // ----------------------

    void OnRecovery_DodgeAllowed() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        dodgeAllowed = true;
    }

    void OnRecovery_Finished() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        pc.fSM.SwitchSt(pc.fSMStates.idle);
    }

    // ----------------------
    // Impact Animation callbacks
    // ----------------------

    void OnImpact_HitDealerDeactivated() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnImpact_HitDealerActivated() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnImpact_Finished() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        pc.visComponents.anims.Play_Atk_HorSlash1_Recovery();
        attackPhase = AtkPhase.Recovery;
    }

    void OnImpact_ComboDisallowed() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        comboAllowed = false;
    }

    void OnImpact_ComboAllowed(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        comboAllowed = true;
    }

    void OnImpact_RotationAllowed() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        impactInputRotationAllowed = true;
    }

    void OnImpact_RotationDisallowed() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        impactInputRotationAllowed = false;
    }
}
