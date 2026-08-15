using System;
using UnityEngine;

public class FsmSt_Pc_Atk_HorSlash1 : MonoBehaviour, IFsmSt{
    /// <summary>
    /// string is the unique id of the animation event.
    /// </summary>
    event Action<string> animEvent;
    
    [SerializeField] Pc pc;

    AtkPhase atkPhase = AtkPhase.Windup;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotAllowed = false;
    float recoveryMotInterpTimer = 0;
    // Animation event ids
    const string Windup_Finished = "Windup_Finished";
    const string Impact_RotationAllowed = "Impact_RotationAllowed";
    const string Impact_RotationDisallowed = "Impact_RotationDisallowed";
    const string Impact_HitDealerActivated = "Impact_HitDealerActivated";
    const string Impact_HitDealerDeactivated = "Impact_HitDealerDeactivated";
    const string Impact_ComboAllowed = "Impact_ComboAllowed";
    const string Impact_ComboDisallowed = "Impact_ComboDisallowed";
    const string Impact_Finished = "Impact_Finished";
    const string Recovery_DodgeAllowed = "Recovery_DodgeAllowed";
    const string Recovery_Finished = "Recovery_Finished";
    ActAnimEvent[] animEvents_Windup = new ActAnimEvent[] {
        new(CapsuleCharAnimInfo.atk_HorSlash1_Windup.lastFrame, 26, Windup_Finished)
    };
    ActAnimEvent[] animEvents_Impact = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_HorSlash1_Impact,
        (0, Impact_RotationAllowed),
        (4, Impact_RotationDisallowed),
        (9, Impact_HitDealerActivated),
        (18, Impact_HitDealerDeactivated),
        (22, Impact_ComboAllowed),
        (24, Impact_ComboDisallowed),
        (26, Impact_Finished)
    );
    ActAnimEvent[] animEvents_Recovery = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
        (6, Recovery_DodgeAllowed),
        (18, Recovery_Finished)
    );

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
            animEvents_Windup,
            animEvent,
            0.1f
        );
    }

    public void Exit(){
        // TODO: Deactivate HitDealers.
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        switch (atkPhase){
            case AtkPhase.Windup:
                pc.charCtrlMov.UpdateMov(
                    Vector2.zero,
                    pc.AnimationDeltaMovement,
                    0,
                    0
                );
                return;
            case AtkPhase.Impact:
                float angSpd = 0;
                if(impactInputRotAllowed)
                    angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd
                );
                if (comboAllowed) {
                    if(pc.inputBuffer.TryConsumeInput("atk1")) {
                        pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash2);
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotInterpTimer / 0.2f);
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_LinAcc,
                    pc.Data.st_Walk_MaxAngSpd * interpValue);
                if (dodgeAllowed) {
                    if (pc.inputBuffer.TryConsumeInput("dodge")) {
                        pc.fsm.SwitchSt(pc.fsmSts.dodge);
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

    // ----------------------
    // Animation Event
    // ----------------------

    private void OnAnimEvent(string id) {
        switch (id) {
            case Windup_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Impact,
                    animEvents_Impact,
                    animEvent
                );
                atkPhase = AtkPhase.Impact;
                break;
            case Impact_RotationAllowed:
                impactInputRotAllowed = true;
                break;
            case Impact_RotationDisallowed:
                impactInputRotAllowed = false;
                break;
            case Impact_HitDealerActivated:
                // TODO
                break;
            case Impact_HitDealerDeactivated:
                // TODO
                break;
            case Impact_ComboAllowed:
                comboAllowed = true;
                break;
            case Impact_ComboDisallowed:
                comboAllowed = false;
                break;
            case Impact_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
                    animEvents_Recovery,
                    animEvent
                );
                atkPhase = AtkPhase.Recovery;
                break;
            case Recovery_DodgeAllowed:
                dodgeAllowed = true;
                break;
            case Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
