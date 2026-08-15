using UnityEngine;

public class FsmSt_Pc_Atk_HorSlash2 : MonoBehaviour, IFsmSt {
    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Impact;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotationAllowed = false;
    float recoveryMotionInterpTimer = 0;

    void OnEnable() {
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_ComboAllowed += OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_Finished += OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_RotationAllowed += OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_RotationDisallowed += OnImpact_RotationDisallowed;
        // Recovery
        pc.visComponents.animEvents.Atk_HorSlash2_Recovery_Finished += OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash2_Recovery_DodgeAllowed += OnRecovery_DodgeAllowed;
    }

    void OnDisable(){
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_Finished -= OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_RotationAllowed -= OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash2_Impact_RotationDisallowed -= OnImpact_RotationDisallowed;
        //Recovery
        pc.visComponents.animEvents.Atk_HorSlash2_Recovery_Finished -= OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash2_Recovery_DodgeAllowed -= OnRecovery_DodgeAllowed;
    }

    public void Enter(IFsmSt previousState) {
        attackPhase = AtkPhase.Impact;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotationAllowed = false;
        recoveryMotionInterpTimer = 0;
        pc.inputBuffer.Clear();
        pc.visComponents.anims.Play_Atk_HorSlash2_Impact();
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
                if (impactInputRotationAllowed) angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd);
                if (comboAllowed) {
                    if (pc.inputBuffer.TryConsumeInput("atk1")) {
                        pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash3);
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotionInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotionInterpTimer / 0.2f);
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_LinAcc,
                    pc.Data.st_Walk_MaxAngSpd * interpValue
                );
                if (dodgeAllowed) {
                    // If input buffer has dodge, then transition to dodge state.
                }
                return;
            default:
                Debug.LogError("Switch defaulted.", this);
                return;
        }
    }

    public void LateTick() {
    }

    // ----------------------
    // Recovery Animation callbacks
    // ----------------------

    private void OnRecovery_DodgeAllowed() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        dodgeAllowed = true;
    }

    private void OnRecovery_Finished() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        pc.fsm.SwitchSt(pc.fsmSts.idle);
    }

    // ----------------------
    // Impact Animation callbacks
    // ----------------------

    private void OnImpact_HitDealerDeactivated(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    private void OnImpact_HitDealerActivated() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    private void OnImpact_Finished() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        pc.visComponents.anims.Play_Atk_HorSlash2_Recovery();
        attackPhase = AtkPhase.Recovery;
    }

    private void OnImpact_ComboDisallowed() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        comboAllowed = false;
    }

    private void OnImpact_ComboAllowed() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        comboAllowed = true;
    }

    private void OnImpact_RotationAllowed() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        impactInputRotationAllowed = true;
    }

    private void OnImpact_RotationDisallowed() {
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        impactInputRotationAllowed = false;
    }
}
