using UnityEngine;

public class FSMSt_PC_Atk_HorSlash1 : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    AttackPhase attackPhase = AttackPhase.Windup;
    bool comboAllowed = false;
    bool plrInitiatedStateSwitchAllowed = false;

    private void OnEnable()
    {
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed += OnImpact_ComboAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed += OnImpact_ComboDisallowed;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_Finished += OnImpact_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated += OnImpact_HitDealerActivated;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated += OnImpact_HitDealerDeactivated;

        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_Finished += OnRecovery_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_StateSwitchAllowed += OnRecovery_StateSwitchAllowed;

        pc.VisComponents.animEvents.Atk_HorSlash1_Windup_Finished += OnWindup_Finished;
    }

    private void OnDisable()
    {
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_ComboAllowed -= OnImpact_ComboAllowed;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_ComboDisallowed -= OnImpact_ComboDisallowed;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_Finished -= OnImpact_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_HitDealerActivated -= OnImpact_HitDealerActivated;
        pc.VisComponents.animEvents.Atk_HorSlash1_Impact_HitDealerDeactivated -= OnImpact_HitDealerDeactivated;

        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_Finished -= OnRecovery_Finished;
        pc.VisComponents.animEvents.Atk_HorSlash1_Recovery_StateSwitchAllowed -= OnRecovery_StateSwitchAllowed;

        pc.VisComponents.animEvents.Atk_HorSlash1_Windup_Finished -= OnWindup_Finished;
    }

    public void Enter(IFSMSt previousState)
    {
        comboAllowed = false;
        plrInitiatedStateSwitchAllowed = false;

        pc.inputBuffer.Clear();

        switch (previousState)
        {
            case FSMSt_PC_Atk_HorSlash2:
                pc.VisComponents.anims.Play_Atk_HorSlash1_Impact();
                attackPhase = AttackPhase.Impact;
                break;
            default:
                pc.VisComponents.anims.Play_Atk_HorSlash1_Windup();
                attackPhase = AttackPhase.Windup;
                break;
        }

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
                pc.Movement.UpdateMovement(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.baseData.St_AtkHorSlash1_MaxLinSpd,
                    pc.baseData.St_AtkHorSlash1_LinAcc,
                    pc.baseData.St_AtkHorSlash1_MaxAngSpd);
                return;
            case AttackPhase.Impact:
                pc.Movement.UpdateMovement(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.baseData.St_AtkHorSlash1_MaxLinSpd,
                    pc.baseData.St_AtkHorSlash1_LinAcc,
                    pc.baseData.St_AtkHorSlash1_MaxAngSpd);
                if (comboAllowed)
                {
                    if(pc.inputBuffer.ConsumeInput("atk1"))
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
                    // TODO: You might want to delay new combo - perhaps a cooldown timer? Currently you can just start the next attack if you transition to walk and instantly transition to attack from there.
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
    // Windup Animation callbacks
    // ----------------------

    private void OnWindup_Finished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        pc.VisComponents.anims.Play_Atk_HorSlash1_Impact();
        attackPhase = AttackPhase.Impact;
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
