using UnityEngine;

public class FSMSt_PC_Atk_HorSlash2 : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    AttackPhase attackPhase = AttackPhase.Impact;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotationAllowed = false;
    float recoveryMotionInterpTimer = 0;

    private void OnEnable()
    {
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_ComboAllowed += OnImpact_ComboAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_Finished += OnImpact_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_RotationAllowed += OnImpact_RotationAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_RotationDisallowed += OnImpact_RotationDisallowed;

        pc.VisComponents.animEvents.Atk_HorSlash2_Recovery_Finished += OnRecovery_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash2_Recovery_DodgeAllowed += OnRecovery_DodgeAllowed;
    }

    private void OnDisable()
    {
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_Finished -= OnImpact_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_RotationAllowed -= OnImpact_RotationAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash2_Impact_RotationDisallowed -= OnImpact_RotationDisallowed;

        pc.VisComponents.animEvents.Atk_HorSlash2_Recovery_Finished -= OnRecovery_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash2_Recovery_DodgeAllowed -= OnRecovery_DodgeAllowed;
    }

    public void Enter(IFSMSt previousState)
    {
        attackPhase = AttackPhase.Impact;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotationAllowed = false;
        recoveryMotionInterpTimer = 0;

        pc.inputBuffer.Clear();

        pc.VisComponents.anims.Play_Atk_HorSlash2_Impact();
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
            case AttackPhase.Impact:
                float angSpd = 0;
                if (impactInputRotationAllowed) angSpd = pc.baseData.St_AtkHorSlash_Impact_AngSpd;
                pc.Movement.UpdateMovement(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd);
                if (comboAllowed)
                {
                    if (pc.inputBuffer.ConsumeInput("atk1"))
                    {
                        pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash3);
                    }
                }
                return;
            case AttackPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotionInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotionInterpTimer / 0.2f);
                pc.Movement.UpdateMovement(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.baseData.St_Walk_MaxLinSpd * interpValue,
                    pc.baseData.St_Walk_LinAcc,
                    pc.baseData.St_Walk_MaxAngSpd * interpValue);
                if (dodgeAllowed)
                {
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

        pc.VisComponents.anims.Play_Atk_HorSlash2_Recovery();
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
}
