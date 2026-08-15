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
        pc.locomotion.IsAffectedByGravity = false;
        pc.visComponents.anims.Play_Atk_FlyingAtk_Windup();
    }

    public void Exit(){
        pc.locomotion.IsAffectedByGravity = true;
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        switch (attackPhase){
            case AtkPhase.Windup:
                pc.locomotion.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    2,
                    0
                );
                break;
            case AtkPhase.Impact:
                // TODO: Set values in base data.
                pc.locomotion.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
                if(pc.locomotion.IsGrounded() && impactFinished){
                    attackPhase = AtkPhase.Recovery;
                    pc.visComponents.anims.Play_Atk_FlyingAtk_Recovery();
                }
                break;
            case AtkPhase.Recovery:
                // TODO: Set values in base data.
                pc.locomotion.UpdateMov(
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

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    void OnAtk_FlyingAtk_Impact_Finished(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        pc.locomotion.IsAffectedByGravity = true;
        impactFinished = true;
        // TODO: Set values in base data.
        pc.locomotion.verVel = -40;
    }

    void OnAtk_FlyingAtk_Recovery_Finished(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        pc.fSM.SwitchSt(pc.fSMStates.idle);
    }

    void OnAtk_FlyingAtk_Windup_Finished(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        attackPhase = AtkPhase.Impact;
        pc.visComponents.anims.Play_Atk_FlyingAtk_Impact();
    }
}
