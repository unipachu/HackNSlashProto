using System;
using UnityEngine;

// TODO: This should probably be called FlyingSlam or something more descriptive than "Atk".
public class FSMSt_PC_Atk_FlyingAtk : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    AttackPhase attackPhase = AttackPhase.Windup;
    bool impactFinished = false;

    private void OnEnable()
    {
        pc.visComponents.animEvents.Atk_FlyingAtk_Impact_Finished += OnAtk_FlyingAtk_Impact_Finished;

        pc.visComponents.animEvents.Atk_FlyingAtk_Recovery_Finished += OnAtk_FlyingAtk_Recovery_Finished;

        pc.visComponents.animEvents.Atk_FlyingAtk_Windup_Finished += OnAtk_FlyingAtk_Windup_Finished;
    }




    private void OnDisable()
    {
        pc.visComponents.animEvents.Atk_FlyingAtk_Impact_Finished -= OnAtk_FlyingAtk_Impact_Finished;

        pc.visComponents.animEvents.Atk_FlyingAtk_Recovery_Finished -= OnAtk_FlyingAtk_Recovery_Finished;

        pc.visComponents.animEvents.Atk_FlyingAtk_Windup_Finished -= OnAtk_FlyingAtk_Windup_Finished;
    }

    public void Enter(IFSMSt previousState)
    {
        attackPhase = AttackPhase.Windup;
        impactFinished = false;
        pc.locomotion.IsAffectedByGravity = false;


        pc.visComponents.anims.Play_Atk_FlyingAtk_Windup();
    }

    public void Exit()
    {
        pc.locomotion.IsAffectedByGravity = true;
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
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    2,
                    0);
                break;
            case AttackPhase.Impact:
                // TODO: Set values in base data.
                pc.locomotion.UpdateMovement(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
                if(pc.locomotion.IsGrounded() && impactFinished)
                {
                    attackPhase = AttackPhase.Recovery;
                    pc.visComponents.anims.Play_Atk_FlyingAtk_Recovery();
                }
                break;
            case AttackPhase.Recovery:
                // TODO: Set values in base data.
                pc.locomotion.UpdateMovement(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.baseData.St_Walk_MaxLinSpd,
                    0,
                    0);
                break;
            default:
                break;
        }
    }

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    private void OnAtk_FlyingAtk_Impact_Finished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        pc.locomotion.IsAffectedByGravity = true;
        impactFinished = true;
        // TODO: Set values in base data.
        pc.locomotion._verticalVelocity = -40;
    }

    private void OnAtk_FlyingAtk_Recovery_Finished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        pc.fSM.SwitchState(pc.fSMStates.idle);
    }

    private void OnAtk_FlyingAtk_Windup_Finished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        attackPhase = AttackPhase.Impact;
        pc.visComponents.anims.Play_Atk_FlyingAtk_Impact();
    }
}
