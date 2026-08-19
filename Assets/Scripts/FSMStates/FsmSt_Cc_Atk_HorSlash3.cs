using System;
using UnityEngine;

public class FsmSt_Cc_Atk_HorSlash3 : MonoBehaviour, IFsmSt {
    event Action<CapsuleCharAnimEvent> animEvent;

    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Impact;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotationAllowed = false;
    float recoveryMotionInterpTimer = 0;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState) {
        attackPhase = AtkPhase.Impact;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotationAllowed = false;
        recoveryMotionInterpTimer = 0;
        pc.inputBuffer.Clear();
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_HorSlash3_Impact,
            animEvent
        );
    }

    public void Exit() {
        pc.weapon.hitDealer.Deactivate();
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        if (CapsuleCharFsmUtils.SwitchToFallingStIfNotGrounded(pc))
            return;
        switch (attackPhase) {
            case AtkPhase.Impact:
                float angSpd = 0;
                if (impactInputRotationAllowed)
                    angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.charCtrlMov.UpdateMov(
                    pc.inputData.mov_CamRel,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd);
                if (comboAllowed) {
                    //if (pc.inputBuffer.ConsumeInput("atk1"))
                    //{
                    //    pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash2);
                    //}
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotionInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotionInterpTimer / 0.2f);
                pc.charCtrlMov.UpdateMov(
                    pc.inputData.mov_CamRel,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_YawSpd * interpValue,
                    pc.Data.st_Walk_LinAcc
                );
                if (dodgeAllowed) {
                    // If input buffer has dodge, then transition to dodge state.
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

    private void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            case CapsuleCharAnimEvent.HorSlash3_Impact_ComboAllowed:
                comboAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash3_Impact_ComboDisallowed:
                comboAllowed = false;
                break;
            case CapsuleCharAnimEvent.HorSlash3_Impact_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
                    animEvent
                );
                attackPhase = AtkPhase.Recovery;
                break;
            case CapsuleCharAnimEvent.HorSlash3_Impact_HitDealerActivated:
                pc.weapon.hitDealer.Activate();
                break;
            case CapsuleCharAnimEvent.HorSlash3_Impact_HitDealerDeactivated:
                pc.weapon.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEvent.HorSlash3_Impact_RotationAllowed:
                impactInputRotationAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash3_Impact_RotationDisallowed:
                impactInputRotationAllowed = false;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Recovery_DodgeAllowed:
                dodgeAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash1_Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
