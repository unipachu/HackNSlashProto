using UnityEngine;

public class FSMSt_PC_Atk_HorSlash1 : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    AttackPhase attackPhase = AttackPhase.Windup;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotationAllowed = false;
    float recoveryMotionInterpTimer = 0;

    private void OnEnable()
    {
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed += OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_Finished += OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationAllowed += OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationDisallowed += OnImpact_RotationDisallowed;

        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished += OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed += OnRecovery_DodgeAllowed;

        pc.visComponents.animEvents.Atk_HorSlash1_Windup_Finished += OnWindup_Finished;
    }

    private void OnDisable()
    {
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_Finished -= OnImpact_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationAllowed -= OnImpact_RotationAllowed;
        pc.visComponents.animEvents.Atk_HorSlash1_Impact_RotationDisallowed -= OnImpact_RotationDisallowed;

        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_Finished -= OnRecovery_Finished;
        pc.visComponents.animEvents.Atk_HorSlash1_Recovery_DodgeAllowed -= OnRecovery_DodgeAllowed;

        pc.visComponents.animEvents.Atk_HorSlash1_Windup_Finished -= OnWindup_Finished;
    }

    public void Enter(IFSMSt previousState)
    {
        attackPhase = AttackPhase.Windup;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotationAllowed = false;
        recoveryMotionInterpTimer = 0;

        pc.inputBuffer.Clear();

        pc.visComponents.anims.Play_Atk_HorSlash1_Windup();
    }

    public void Exit()
    {
        // TODO: Deactivate HitDealers.
    }

    public void PhysicsTick()
    {
    }

    public void Tick()
    {
        switch (attackPhase)
        {
            case AttackPhase.Windup:
                pc.locomotion.UpdateMovement(
                    Vector2.zero,
                    pc.AnimationDeltaMovement,
                    0,
                    0);
                return;
            case AttackPhase.Impact:
                float angSpd = 0;
                if(impactInputRotationAllowed) angSpd = pc.baseData.St_AtkHorSlash_Impact_AngSpd;
                pc.locomotion.UpdateMovement(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd);
                if (comboAllowed)
                {
                    if(pc.inputBuffer.TryConsumeInput("atk1"))
                    {
                        pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash2);
                    }
                }
                return;
            case AttackPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotionInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotionInterpTimer / 0.2f);
                pc.locomotion.UpdateMovement(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.baseData.St_Walk_MaxLinSpd * interpValue,
                    pc.baseData.St_Walk_LinAcc,
                    pc.baseData.St_Walk_MaxAngSpd * interpValue);
                if (dodgeAllowed)
                {
                    if (pc.inputBuffer.TryConsumeInput("dodge"))
                    {
                        pc.fSM.SwitchState(pc.fSMStates.dodge);
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

    private void OnRecovery_DodgeAllowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        dodgeAllowed = true;
    }

    private void OnRecovery_Finished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        pc.fSM.SwitchState(pc.fSMStates.idle);
    }

    // ----------------------
    // Impact Animation callbacks
    // ----------------------

    private void OnImpact_HitDealerDeactivated()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO
    }

    private void OnImpact_HitDealerActivated()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO
    }

    private void OnImpact_Finished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        pc.visComponents.anims.Play_Atk_HorSlash1_Recovery();
        attackPhase = AttackPhase.Recovery;
    }

    private void OnImpact_ComboDisallowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        comboAllowed = false;
    }

    private void OnImpact_ComboAllowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        comboAllowed = true;
    }

    private void OnImpact_RotationAllowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        impactInputRotationAllowed = true;
    }

    private void OnImpact_RotationDisallowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        impactInputRotationAllowed = false;
    }

    // ----------------------
    // Windup Animation callbacks
    // ----------------------

    private void OnWindup_Finished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        pc.visComponents.anims.Play_Atk_HorSlash1_Impact();
        attackPhase = AttackPhase.Impact;
    }

}
