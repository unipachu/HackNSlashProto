using System;
using UnityEngine;

public class FsmSt_Pc_Atk_HorSlash1 : MonoBehaviour, IFsmSt{
    /// <summary>
    /// string is the unique id of the animation event.
    /// </summary>
    event Action<CapsuleCharAnimEvent> animEvent;
    
    [SerializeField] Pc pc;

    AtkPhase atkPhase = AtkPhase.Windup;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotAllowed = false;
    float recoveryMotInterpTimer = 0;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    // ----------------------
    // IFsmSt Methods
    // ----------------------

    public void Enter(IFsmSt previousState) {
        atkPhase = AtkPhase.Windup;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotAllowed = false;
        recoveryMotInterpTimer = 0;
        pc.inputBuffer.Clear();
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_HorSlash1_Windup,
            animEvent,
            0.1f
        );
    }

    public void Exit(){
        pc.weapon.hitDealer.Deactivate();
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        if (CapsuleCharFsmUtils.SwitchToFallingStIfNotGrounded(pc))
            return;
        switch (atkPhase){
            case AtkPhase.Windup:
                pc.charCtrlMov.UpdateMov(
                    pc.inputData.mov_WhenLastSwitchedSt_CamRel,
                    pc.AnimationDeltaMovement,
                    0,
                    pc.Data.st_AtkHorSlash_Windup_MaxAngSpd
                );
                return;
            case AtkPhase.Impact:
                float angSpd = 0;
                if(impactInputRotAllowed)
                    angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.charCtrlMov.UpdateMov(
                    pc.inputData.mov_CamRel,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd
                );
                if (comboAllowed) {
                    if(pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Light)) {
                        pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash2);
                        return;
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotInterpTimer / 0.2f);
                pc.charCtrlMov.UpdateMov(
                    pc.inputData.mov_CamRel,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_YawSpd * interpValue,
                    pc.Data.st_Walk_LinAcc
                );
                if (dodgeAllowed) {
                    if (pc.inputBuffer.TryConsumeInput(BufferableInput.Dodge)) {
                        pc.fsm.SwitchSt(pc.fsmSts.dodge);
                        return;
                    }
                }
                return;
            default:
                Debug.LogError("Switch defaulted.", this);
                return;
        }
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;

    // ----------------------
    // Animation Event
    // ----------------------

    void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            case CapsuleCharAnimEvent.HorSlash1_Windup_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Impact,
                    animEvent
                );
                atkPhase = AtkPhase.Impact;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Impact_RotationAllowed:
                impactInputRotAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Impact_RotationDisallowed:
                impactInputRotAllowed = false;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Impact_HitDealerActivated:
                pc.weapon.hitDealer.Activate();
                break;
            case CapsuleCharAnimEvent.HorSlash1_Impact_HitDealerDeactivated:
                pc.weapon.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEvent.HorSlash1_Impact_ComboAllowed:
                comboAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Impact_ComboDisallowed:
                comboAllowed = false;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Impact_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
                    animEvent
                );
                atkPhase = AtkPhase.Recovery;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Recovery_DodgeAllowed:
                dodgeAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                return;
        }
    }
}
