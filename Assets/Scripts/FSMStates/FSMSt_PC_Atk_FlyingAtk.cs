using UnityEngine;

// TODO: This should probably be called FlyingSlam or something more descriptive than "Atk".
public class FsmSt_Pc_Atk_FlyingAtk : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Windup;
    bool impactFinished = false;

    void OnEnable() {
        pc.visComponents.animEvents.Atk_FlyingAtk_Impact_Finished += OnAtk_FlyingAtk_Impact_Finished;
        pc.visComponents.animEvents.Atk_FlyingAtk_Recovery_Finished += OnAtk_FlyingAtk_Recovery_Finished;
        pc.visComponents.animEvents.Atk_FlyingAtk_Windup_Finished += OnAtk_FlyingAtk_Windup_Finished;
    }

    void OnDisable(){
        pc.visComponents.animEvents.Atk_FlyingAtk_Impact_Finished -= OnAtk_FlyingAtk_Impact_Finished;
        pc.visComponents.animEvents.Atk_FlyingAtk_Recovery_Finished -= OnAtk_FlyingAtk_Recovery_Finished;
        pc.visComponents.animEvents.Atk_FlyingAtk_Windup_Finished -= OnAtk_FlyingAtk_Windup_Finished;
    }

    public void Enter(IFsmSt previousState){
        attackPhase = AtkPhase.Windup;
        impactFinished = false;
        pc.charCtrlMov.IsAffectedByGravity = false;
        pc.visComponents.anims.Play_Atk_FlyingAtk_Windup();
    }

    public void Exit(){
        pc.charCtrlMov.IsAffectedByGravity = true;
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        switch (attackPhase){
            case AtkPhase.Windup:
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    2,
                    0
                );
                break;
            case AtkPhase.Impact:
                // TODO: Set values in base data.
                pc.charCtrlMov.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
                if(pc.charCtrlMov.IsGrounded() && impactFinished){
                    attackPhase = AtkPhase.Recovery;
                    pc.visComponents.anims.Play_Atk_FlyingAtk_Recovery();
                }
                break;
            case AtkPhase.Recovery:
                // TODO: Set values in base data.
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd,
                    0,
                    0
                );
                break;
            default:
                Debug.LogError("Switch defaulted.", this);
                break;
        }
    }

    public void LateTick() {
    }

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    void OnAtk_FlyingAtk_Impact_Finished(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        pc.charCtrlMov.IsAffectedByGravity = true;
        impactFinished = true;
        // TODO: Set values in base data.
        pc.charCtrlMov.verVel = -40;
    }

    void OnAtk_FlyingAtk_Recovery_Finished(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        pc.fsm.SwitchSt(pc.fsmSts.idle);
    }

    void OnAtk_FlyingAtk_Windup_Finished(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        attackPhase = AtkPhase.Impact;
        pc.visComponents.anims.Play_Atk_FlyingAtk_Impact();
    }
}
