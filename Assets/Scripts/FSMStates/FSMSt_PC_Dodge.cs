using System;
using UnityEngine;

public class FSMSt_PC_Dodge : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    bool yawAllowed = false;
    bool bufferedInputStateSwitchAllowed = false;

    private void OnEnable()
    {
        pc.visComponents.animEvents.Dodge_BufferedInputStateSwitchAllowed += OnBufferedInputStateSwitchAllowed;
        pc.visComponents.animEvents.Dodge_Finished += OnFinished;
        pc.visComponents.animEvents.Dodge_InvulnerabilityEnd += OnInvulnerabilityEnd;
        pc.visComponents.animEvents.Dodge_YawAllowed += OnYawAllowed;
    }

    private void OnDisable()
    {
        
    }

    public void Enter(IFSMSt previousState)
    {
        yawAllowed = false;
        bufferedInputStateSwitchAllowed = false;
        // TODO: Turn on invulnerability.

        pc.visComponents.anims.Play_Dodge();
    }

    public void Exit()
    {
        // TODO: Turn off invulnerability
    }

    public void PhysicsTick()
    {
    }

    public void Tick()
    {
        float angSpd = 0;
        if (yawAllowed) angSpd = pc.baseData.St_Dodge_YawAngSpd;
        pc.locomotion.UpdateMovement(pc.MoveInput, pc.AnimationDeltaMovement, 0, angSpd);

        if(bufferedInputStateSwitchAllowed)
        {
            if (pc.inputBuffer.TryConsumeInput("atk1"))
                pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash1);
            else if (pc.inputBuffer.TryConsumeInput("atk2"))
                pc.fSM.SwitchState(pc.fSMStates.atk_Jump);
            // TODO: The problem this: switching to same state is currently not allowed because of the bs animation event behavior.
            //else if (pc.inputBuffer.TryConsumeInput("dodge"))
            //    pc.fSM.SwitchState(pc.fSMStates.dodge);
        }
    }

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    private void OnBufferedInputStateSwitchAllowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        bufferedInputStateSwitchAllowed = true;
    }

    private void OnFinished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        if (pc.MoveInput != Vector2.zero) pc.fSM.SwitchState(pc.fSMStates.walk);
        else pc.fSM.SwitchState(pc.fSMStates.idle);
    }

    private void OnInvulnerabilityEnd()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO: Turn off invulnerability.
    }

    private void OnYawAllowed()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        yawAllowed = true;
    }
}
