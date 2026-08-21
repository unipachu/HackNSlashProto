using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule character.
/// </summary>
// TODO: Rename to Cc.
public class Pc : MonoBehaviour{
    [Header("Data Refs")]
    [SerializeField] CapsuleCharInitializer capsuleCharRegisterer;

    [Header("Component Refs")]
    public CapsuleCharCtrl ctrl;
    public CharCtrlMov charCtrlMov;
    public Fsm fsm;
    public Fsm_PcSts fsmSts;
    public PcInputBuffer inputBuffer;
    public AnimRootMovBroadcaster capsuleCharRootMvmtBroadcaster;
    public Animator capsuleCharAnim;
    public CapsuleCharHitRecieveHandler hitRecieverHandler;
    public CharacterController charCtrl;
    public Transform tgt;
    public NavMeshAgent agent;
    public Transform rHand;
    
    public const float movInputDeadzone = 0.2f;
    [HideInInspector]public HandEquippable rHandEquippable;

    /// <summary>
    /// Used to create animation events decoupled from the Animator.
    /// </summary>
    [HideInInspector] public AnimEventPlr animEventPlr;

    public CapsuleCharData Data {
        get => capsuleCharRegisterer.Data;
        set => capsuleCharRegisterer.Data = value;
    }
    public Vector3 AnimationDeltaMovement { get; private set; }

    void OnEnable(){
        capsuleCharRootMvmtBroadcaster.OnRootMove += OnAnimatorRootMove;
        fsm.StSwitched += OnStSwitched;
    }

    void Start(){
        // Enter initial state:
        fsm.SwitchSt(fsmSts.idle);
    }

    void FixedUpdate(){
        UpdateGroundCheck(this);
        fsm.CurSt.PhysicsTick();
    }

    void Update() {
        var data = Data;
        UpdateData_Sensing(ref data, transform, tgt);
        UpdateData_FromNonNative(ref data, transform, charCtrl);
        if(ctrl != null)
            UpdateData_Input(ref data, ctrl, inputBuffer);
        if(agent != null)
            UpdateData_AgentMovInput(ref data, agent, tgt);
        Data = data;
        UpdateInputBuffer(Data, inputBuffer);
        fsm.CurSt.Tick();
    }

    void LateUpdate() {
        fsm.CurSt.LateTick();
    }

    void OnDisable(){
        capsuleCharRootMvmtBroadcaster.OnRootMove -= OnAnimatorRootMove;
        fsm.StSwitched -= OnStSwitched;
    }

    public static void UpdateData_AgentMovInput(
        ref CapsuleCharData data,
        NavMeshAgent agent,
        Transform tgt
    ) {
        if(tgt == null) {
            data.brain_AgentDesiredVel = float3.zero;
            return;
        }
        agent.SetDestination(tgt.position);
        data.brain_AgentDesiredVel = agent.desiredVelocity;
        //Debug.Log($"Agent desired vel: {data.brain_AgentDesiredVel}", agent);
    }

    /// <summary>
    /// Update data from non native data sources, e.g. from Monobehavior components.
    /// </summary>
    public static void UpdateData_FromNonNative(
        ref CapsuleCharData data,
        Transform thisTrf,
        CharacterController charCtrl
    ) {
        data.trf_pos = thisTrf.position;
        data.trf_rot = thisTrf.rotation;
        data.trf_lossyScl = thisTrf.lossyScale;
        data.lastCharCtrlVel = charCtrl.velocity;
        data.curStDur += Time.deltaTime;
    }

    public static void UpdateGroundCheck(Pc pc) {
        var data = pc.Data;
        data.isGrounded = CharCtrlMov.IsGrounded(
            pc.charCtrl,
            out data.groundCastHitSomething,
            out RaycastHit groundCastResult
        );
        data.groundCastNrm = groundCastResult.normal;
        pc.Data = data;
    }

    public static void UpdateData_Input(
        ref CapsuleCharData data,
        CapsuleCharCtrl ctrl,
        PcInputBuffer inputBuffer
    ){
        data.input_atk_Light = ctrl.TryConsume_Atk_Light();
        data.input_atk_Heavy = ctrl.TryConsume_Atk_Heavy();
        data.input_atk_Ult = ctrl.TryConsume_Atk_Ult();
        data.input_dodge = ctrl.TryConsume_Dodge();
        if (ctrl.Input_Mov.sqrMagnitude > movInputDeadzone) {
            data.input_mov = ctrl.Input_Mov;
            data.input_mov_LastNonZero = data.input_mov;
        } else {
            data.input_mov = Vector2.zero;
        }
        // Buffer certain inputs

        //Debug.Log("mov input mag: " + data.mov.magnitude);
    }

    public static void UpdateData_Sensing(
        ref CapsuleCharData data,
        Transform thisTrf,
        Transform tgt
    ) {
        if (tgt != null) {
            data.brain_DistToTgt = Vector3.Distance(thisTrf.position, tgt.position);
            data.brain_HasTgt = true;
            data.brain_InAggroRange
                = Vector3.Distance(thisTrf.position, tgt.position) < data.brain_AggroRange;
            data.brain_InAtkRange
                = Vector3.Distance(thisTrf.position, tgt.position) < data.brain_AtkRange;
            data.brain_TgtPos = tgt.position;
        }
        else
            data.brain_HasTgt = false;
    }

    public static void UpdateInputBuffer(CapsuleCharData data, PcInputBuffer inputBuffer) {
        if (data.input_atk_Light)
            inputBuffer.BufferInput(BufferableInput.Atk_Light);
        else if (data.input_atk_Heavy)
            inputBuffer.BufferInput(BufferableInput.Atk_Heavy);
        else if (data.input_atk_Ult)
            inputBuffer.BufferInput(BufferableInput.Atk_Ult);
        else if (data.input_dodge)
            inputBuffer.BufferInput(BufferableInput.Dodge);
    }

    // ---------------------------------------------------------------
    // Event Callbacks
    // ---------------------------------------------------------------

    /// <summary>
    /// Used to save latest animation delta movement. Makes y component 0.
    /// </summary>
    /// <param name="deltaLinearMovement">
    /// Delta movement of animation root.
    /// </param>
    void OnAnimatorRootMove(Vector3 deltaLinearMovement){
        AnimationDeltaMovement = deltaLinearMovement;
    }

    void OnStSwitched() {
        //Debug.Log($"Mov input when st switched: {inputData.mov}.");
        CapsuleCharData data = Data;
        data.curStDur = 0;
        if(ctrl == null) {
            data.input_mov_WhenLastSwitchedSt = data.input_mov;
        } else {
            if (ctrl.Input_Mov.sqrMagnitude > movInputDeadzone)
                data.input_mov_WhenLastSwitchedSt = data.input_mov;
            else
                data.input_mov_WhenLastSwitchedSt = Vector2.zero;
        }
        Data = data;
    }
}
