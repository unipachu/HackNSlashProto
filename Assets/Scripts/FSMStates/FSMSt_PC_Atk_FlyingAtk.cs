using System;
using UnityEngine;

// TODO: This should probably be called FlyingSlam or something more descriptive than "Atk".
public class FsmSt_Pc_Atk_FlyingAtk : MonoBehaviour, IFsmSt{
    event Action<string> animEvent;
    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Windup;
    bool impactFinished = false;
    // Anim event ids:
    const string Windup_Finished = "Windup_Finished";
    const string Impact_HitDealerActivated = "Impact_HitDealerActivated";
    const string Impact_HitDealerDeactivated = "Impact_HitDealerDeactivated";
    const string Impact_Finished = "Impact_Finished";
    const string Recovery_Finished = "Recovery_Finished";
    ActAnimEvent[] animEvents_Windup = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_FlyingAtk_Windup,
        (95, Windup_Finished)
    );
    ActAnimEvent[] animEvents_Impact = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_FlyingAtk_Impact,
        (30, Impact_Finished)
    );
    ActAnimEvent[] animEvents_Recovery = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_FlyingAtk_Recovery,
        (48, Recovery_Finished)
    );

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable(){
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState){
        attackPhase = AtkPhase.Windup;
        impactFinished = false;
        pc.charCtrlMov.IsAffectedByGravity = false;
        pc.inputBuffer.Clear();
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_FlyingAtk_Windup,
            animEvents_Windup,
            animEvent,
            0.1f
        );
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
                    VisUtils.CrossfadeNInitAnimEventPlr(
                        ref pc.animEventPlr,
                        pc.capsuleCharAnim,
                        CapsuleCharAnimInfo.atk_FlyingAtk_Recovery,
                        animEvents_Recovery,
                        animEvent
                    );
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
        pc.animEventPlr.Tick();
    }

    // -------------------------
    // Anim Event
    // -------------------------

    private void OnAnimEvent(string id) {
        switch (id) {
            case Windup_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_FlyingAtk_Impact,
                    animEvents_Impact,
                    animEvent
                );
                attackPhase = AtkPhase.Impact;
                break;
            //case Impact_HitDealerActivated:
            //    // TODO: Activate hit dealer.
            //    break;
            //case Impact_HitDealerDeactivated:
            //    // TODO: Deactivate hit dealer.
            //    break;
            case Impact_Finished:
                pc.charCtrlMov.IsAffectedByGravity = true;
                impactFinished = true;
                // TODO: Set this in base data.
                pc.charCtrlMov.verVel = -40;
                break;
            case Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
