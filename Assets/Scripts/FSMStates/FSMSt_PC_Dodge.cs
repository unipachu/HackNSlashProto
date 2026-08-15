using System;
using UnityEngine;

public class FsmSt_Pc_Dodge : MonoBehaviour, IFsmSt{
    event Action<string> animEvent;

    [SerializeField] Pc pc;

    bool yawAllowed = false;
    bool bufferedInputStateSwitchAllowed = false;

    // Animation event ids
    const string YawAllowed = "YawAllowed";
    const string InvulnerabilityEnd = "InvulnerabilityEnd";
    const string BufferedInputStateSwitchAllowed =
        "BufferedInputStateSwitchAllowed";
    const string Finished = "Finished";

    // TODO: Set the correct frame count / event frames.
    ActAnimEvent[] animEvents_Dodge = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.dodge,
        (6, YawAllowed),
        (18, InvulnerabilityEnd),
        (24, BufferedInputStateSwitchAllowed),
        (30, Finished)
    );

    void OnEnable(){
        animEvent += OnAnimEvent;
    }

    void OnDisable(){
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState){
        yawAllowed = false;
        bufferedInputStateSwitchAllowed = false;
        // TODO: Turn on invulnerability.
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.dodge,
            animEvents_Dodge,
            animEvent,
            0.1f
        );
    }

    public void Exit(){
        // TODO: Turn off invulnerability
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        float angSpd = 0;
        if (yawAllowed) angSpd = pc.Data.st_Dodge_YawAngSpd;
        pc.charCtrlMov.UpdateMov(pc.MoveInput, pc.AnimationDeltaMovement, 0, angSpd);
        if(bufferedInputStateSwitchAllowed){
            if (pc.inputBuffer.TryConsumeInput("atk1"))
                pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash1);
            else if (pc.inputBuffer.TryConsumeInput("atk2"))
                pc.fsm.SwitchSt(pc.fsmSts.atk_Jump);
            else if (pc.inputBuffer.TryConsumeInput("dodge"))
                pc.fsm.SwitchSt(pc.fsmSts.dodge);
        }
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    // ----------------------
    // Animation event
    // ----------------------

    private void OnAnimEvent(string id) {
        switch (id) {
            case YawAllowed:
                yawAllowed = true;
                break;
            case InvulnerabilityEnd:
                // TODO: Turn off invulnerability.
                break;
            case BufferedInputStateSwitchAllowed:
                bufferedInputStateSwitchAllowed = true;
                break;
            case Finished:
                if (pc.MoveInput != Vector2.zero)
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
