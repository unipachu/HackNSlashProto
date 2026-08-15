using UnityEngine;

public class FsmSt_Pc_Dodge : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    bool yawAllowed = false;
    bool bufferedInputStateSwitchAllowed = false;

    void OnEnable(){
        pc.visComponents.animEvents.Dodge_BufferedInputStateSwitchAllowed += OnBufferedInputStateSwitchAllowed;
        pc.visComponents.animEvents.Dodge_Finished += OnFinished;
        pc.visComponents.animEvents.Dodge_InvulnerabilityEnd += OnInvulnerabilityEnd;
        pc.visComponents.animEvents.Dodge_YawAllowed += OnYawAllowed;
    }

    void OnDisable(){
        
    }

    public void Enter(IFsmSt previousState){
        yawAllowed = false;
        bufferedInputStateSwitchAllowed = false;
        // TODO: Turn on invulnerability.
        pc.visComponents.anims.Play_Dodge();
    }

    public void Exit(){
        // TODO: Turn off invulnerability
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        float angSpd = 0;
        if (yawAllowed) angSpd = pc.Data.st_Dodge_YawAngSpd;
        pc.locomotion.UpdateMov(pc.MoveInput, pc.AnimationDeltaMovement, 0, angSpd);
        if(bufferedInputStateSwitchAllowed){
            if (pc.inputBuffer.TryConsumeInput("atk1"))
                pc.fSM.SwitchSt(pc.fSMStates.atk_HorSlash1);
            else if (pc.inputBuffer.TryConsumeInput("atk2"))
                pc.fSM.SwitchSt(pc.fSMStates.atk_Jump);
            // TODO: The problem this: switching to same state is currently not allowed because of the bs animation event behavior.
            //else if (pc.inputBuffer.TryConsumeInput("dodge"))
            //    pc.fSM.SwitchState(pc.fSMStates.dodge);
        }
    }

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    void OnBufferedInputStateSwitchAllowed(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        bufferedInputStateSwitchAllowed = true;
    }

    void OnFinished(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        if (pc.MoveInput != Vector2.zero)
            pc.fSM.SwitchSt(pc.fSMStates.walk);
        else
            pc.fSM.SwitchSt(pc.fSMStates.idle);
    }

    void OnInvulnerabilityEnd(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO: Turn off invulnerability.
    }

    void OnYawAllowed() {
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        yawAllowed = true;
    }
}
