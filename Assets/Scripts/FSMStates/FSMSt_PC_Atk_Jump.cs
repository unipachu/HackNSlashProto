using System;
using UnityEngine;

public class FsmSt_Pc_Atk_Jump : MonoBehaviour, IFsmSt{
    event Action<string> animEvent;
    
    [SerializeField] Pc pc;

    // Animation event ids
    const string Finished = "Finished";
    const string HitboxActivated = "HitboxActivated";
    const string HitboxDeactivated = "HitboxDeactivated";
    const string JumpFinished = "JumpFinished";
    const string JumpStarted = "JumpStarted";

    ActAnimEvent[] animEvents_Jump = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_JumpVerSlam,
        (40, JumpStarted),
        (69, HitboxActivated),
        (81, JumpFinished),
        (88, HitboxDeactivated),
        (120, Finished)
    );

    void OnEnable(){
        animEvent += OnAnimEvent;
    }

    void OnDisable(){
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState){
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_JumpVerSlam,
            animEvents_Jump,
            animEvent
        );
    }

    public void Exit(){
        pc.charCtrlMov.IsAffectedByGravity = true;
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        pc.charCtrlMov.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    // ----------------------
    // Animation Event
    // ----------------------

    private void OnAnimEvent(string id) {
        switch (id) {
            case Finished:
                if (pc.MoveInput != Vector2.zero)
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
            case HitboxActivated:
                // TODO: Activate hitbox.
                break;
            case HitboxDeactivated:
                // TODO: Deactivate hitbox.
                break;
            case JumpFinished:
                pc.charCtrlMov.IsAffectedByGravity = true;
                pc.charCtrlMov.verVel =
                    -pc.Data.st_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case JumpStarted:
                pc.charCtrlMov.IsAffectedByGravity = false;
                break;
        }
    }
}
