using UnityEngine;

public class FsmSt_Pc_Atk_HorSlash1 : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    AtkPhase atkPhase = AtkPhase.Windup;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotAllowed = false;
    float recoveryMotInterpTimer = 0;

    void OnEnable() {
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed += OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_Finished += OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationAllowed += OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationDisallowed += OnImpact_RotationDisallowed;
        // Recovery
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished += OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed += OnRecovery_DodgeAllowed;
        // Windup
        pc.visComponents.animEvents.Atk_HorSlash1_Windup_Finished += OnWindup_Finished;
    }

    void OnDisable() {
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_Finished -= OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationAllowed -= OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationDisallowed -= OnImpact_RotationDisallowed;
        // Recovery
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished -= OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed -= OnRecovery_DodgeAllowed;
        // Windup
        pc.visComponents.animEvents.Atk_HorSlash1_Windup_Finished -= OnWindup_Finished;
    }

    public void Enter(IFsmSt previousState) {
        atkPhase = AtkPhase.Windup;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotAllowed = false;
        recoveryMotInterpTimer = 0;
        pc.inputBuffer.Clear();
        pc.visComponents.anims.Play_Atk_HorSlash1_Windup();
    }

    public void Exit(){
        // TODO: Deactivate HitDealers.
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        switch (atkPhase){
            case AtkPhase.Windup:
                pc.locomotion.UpdateMov(
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
                pc.locomotion.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd
                );
                if (comboAllowed) {
                    if(pc.inputBuffer.TryConsumeInput("atk1")) {
                        pc.fSM.SwitchSt(pc.fSMStates.atk_HorSlash2);
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotInterpTimer / 0.2f);
                pc.locomotion.UpdateMov(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_LinAcc,
                    pc.Data.st_Walk_MaxAngSpd * interpValue);
                if (dodgeAllowed) {
                    if (pc.inputBuffer.TryConsumeInput("dodge")) {
                        pc.fSM.SwitchSt(pc.fSMStates.dodge);
                    }
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

    void OnRecovery_DodgeAllowed(){
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

    void OnImpact_HitDealerDeactivated(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnImpact_HitDealerActivated(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnImpact_Finished(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        pc.visComponents.anims.Play_Atk_HorSlash1_Recovery();
        atkPhase = AtkPhase.Recovery;
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

    void OnImpact_RotationAllowed(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        impactInputRotAllowed = true;
    }

    void OnImpact_RotationDisallowed(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        impactInputRotAllowed = false;
    }

    // ----------------------
    // Windup Animation callbacks
    // ----------------------

    void OnWindup_Finished(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        pc.visComponents.anims.Play_Atk_HorSlash1_Impact();
        atkPhase = AtkPhase.Impact;
    }
}
