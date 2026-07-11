using UnityEngine;

public class FSMSt_PC_Atk_HorSlash3 : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    AttackPhase attackPhase = AttackPhase.Impact;
    bool comboAllowed = false;
    bool plrInitiatedStateSwitchAllowed = false;

    private void OnEnable()
    {
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_ComboAllowed += OnImpact_ComboAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_Finished += OnImpact_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;

        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_Finished += OnRecovery_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_StateSwitchAllowed += OnRecovery_StateSwitchAllowed;
    }

    private void OnDisable()
    {
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_Finished -= OnImpact_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        pc.VisComponents.animEvents.Atk_HorSlash3_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;

        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_Finished -= OnRecovery_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_StateSwitchAllowed -= OnRecovery_StateSwitchAllowed;
    }

    public void Enter(IFSMSt previousState)
    {
        attackPhase = AttackPhase.Impact;
        comboAllowed = false;
        plrInitiatedStateSwitchAllowed = false;

        pc.inputBuffer.Clear();

        pc.VisComponents.anims.Play_Atk_HorSlash3_Impact();
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
                pc.Movement.UpdateMovement(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.baseData.St_AtkHorSlash1_MaxLinSpd,
                    pc.baseData.St_AtkHorSlash1_LinAcc,
                    pc.baseData.St_AtkHorSlash1_MaxAngSpd);
                if (comboAllowed)
                {
                    if (pc.inputBuffer.ConsumeInput("atk1"))
                    {
                        pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash2);
                    }
                }
                return;
            case AttackPhase.Recovery:
                pc.Movement.UpdateMovement(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.baseData.St_AtkHorSlash1_MaxLinSpd,
                    pc.baseData.St_AtkHorSlash1_LinAcc,
                    pc.baseData.St_AtkHorSlash1_MaxAngSpd);
                if (plrInitiatedStateSwitchAllowed)
                {
                    if (pc.MoveInput != Vector2.zero)
                    {
                        pc.fSM.SwitchState(pc.fSMStates.walk);
                        return;
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

    private void OnRecovery_StateSwitchAllowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        plrInitiatedStateSwitchAllowed = true;
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

        pc.VisComponents.anims.Play_Atk_HorSlash1_Recovery();
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
}
