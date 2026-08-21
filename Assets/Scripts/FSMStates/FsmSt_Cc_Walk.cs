using Unity.Mathematics;
using UnityEngine;

public class FsmSt_Cc_Walk : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    public void Enter(IFsmSt previousState){
        switch(previousState){
            case FsmSt_Cc_Atk_HorSlash1:
                // We use a little more time to fade to the walk animation.
                VisUtils.CrossfadeAnim(pc.capsuleCharAnim, CapsuleCharAnimInfo.walk, 1);
                break;
            case FsmSt_Cc_Atk_HorSlash2:
                // We use a little more time to fade to the walk animation.
                VisUtils.CrossfadeAnim(pc.capsuleCharAnim, CapsuleCharAnimInfo.walk, 1);
                break;
            default:
                VisUtils.CrossfadeAnim(pc.capsuleCharAnim, CapsuleCharAnimInfo.walk, 0.1f);
                break;
        }
    }

    public void Exit(){
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        if (CapsuleCharFsmUtils.SwitchToFallingStIfNotGrounded(pc))
            return;
        pc.charCtrlMov.UpdateMov(
            pc.Data.input_mov,
            Vector3.zero,
            pc.Data.st_Walk_MaxLinSpd,
            pc.Data.st_Walk_YawSpd,
            pc.Data.st_Walk_LinAcc
        );
        if (pc.inputBuffer.TryConsumeInput(BufferableInput.Dodge))
            pc.fsm.SwitchSt(pc.fsmSts.dodge);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Light))
            CapsuleCharFsmUtils.SwitchToLightAtkSt(pc);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Heavy))
            pc.fsm.SwitchSt(pc.fsmSts.atk_Jump);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Ult))
            pc.fsm.SwitchSt(pc.fsmSts.atk_FlyingAtk);
        else if (pc.Data.input_mov.Equals(float2.zero))
            pc.fsm.SwitchSt(pc.fsmSts.idle);
    }

    public void LateTick() {
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;
}
