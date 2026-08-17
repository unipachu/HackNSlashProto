using System;
using UnityEngine;

public class FsmSt_Pc_Dodge : MonoBehaviour, IFsmSt{
    event Action<CapsuleCharAnimEvent> animEvent;

    [SerializeField] Pc pc;

    bool yawAllowed = false;
    bool bufferedInputStateSwitchAllowed = false;

    void OnEnable(){
        animEvent += OnAnimEvent;
    }

    void OnDisable(){
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState){
        yawAllowed = false;
        bufferedInputStateSwitchAllowed = false;
        var data = pc.Data;
        data.invul = true;
        pc.Data = data;
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.dodge,
            animEvent,
            0.1f
        );
    }

    public void Exit(){
        var data = pc.Data;
        data.invul = false;
        pc.Data = data;
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        float angSpd = 0;
        if (yawAllowed)
            angSpd = pc.Data.st_Dodge_YawAngSpd;
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

    public bool CanSwitchStTo(IFsmSt newSt) => true;

    // ----------------------
    // Animation event
    // ----------------------

    private void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            case CapsuleCharAnimEvent.Dodge_YawAllowed:
                yawAllowed = true;
                break;
            case CapsuleCharAnimEvent.Dodge_InvulEnd:
                var data = pc.Data;
                data.invul = false;
                pc.Data = data;
                break;
            case CapsuleCharAnimEvent.Dodge_BufferedInputStSwitchAllowed:
                bufferedInputStateSwitchAllowed = true;
                break;
            case CapsuleCharAnimEvent.Dodge_Finished:
                if (pc.MoveInput != Vector2.zero)
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
