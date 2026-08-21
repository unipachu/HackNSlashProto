using Unity.Mathematics;
using UnityEngine;

public class FsmSt_Cc_Idle : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    public void Enter(IFsmSt previousState){
        VisUtils.CrossfadeAnim(pc.capsuleCharAnim, CapsuleCharAnimInfo.idle, 0.1f);
    }

    public void Exit(){
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        if (CapsuleCharFsmUtils.SwitchToFallingStIfNotGrounded(pc))
            return;
        if (pc.fsm.PrevSt == (IFsmSt)pc.fsmSts.walk)
            pc.charCtrlMov.UpdateMov(
                pc.Data.input_mov_LastNonZero,
                Vector3.zero,
                0,
                pc.Data.st_Walk_YawSpd
            );
        else {
            pc.charCtrlMov.UpdateMov(
                Vector2.zero,
                Vector3.zero,
                0,
                0
            );
        }
        // Try consume input
        if (pc.inputBuffer.TryConsumeInput(BufferableInput.Dodge))
            pc.fsm.SwitchSt(pc.fsmSts.dodge);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Light))
            CapsuleCharFsmUtils.SwitchToLightAtkSt(pc);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Heavy))
            pc.fsm.SwitchSt(pc.fsmSts.atk_Jump);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Ult))
            pc.fsm.SwitchSt(pc.fsmSts.atk_FlyingAtk);
        else if (!pc.Data.input_mov.Equals(float2.zero))
            pc.fsm.SwitchSt(pc.fsmSts.walk);
    }

    public void LateTick() {
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;
}
