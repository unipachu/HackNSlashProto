using UnityEngine;

/// <summary>
/// Playable character.
/// </summary>
public class Pc : MonoBehaviour{
    [Header("Data Refs")]
    [SerializeField] CapsuleCharRegisterer capsuleCharRegisterer;

    [Header("Component Refs")]
    public CharCtrlMov charCtrlMov;
    public Fsm fsm;
    public Fsm_PcSts fsmSts;
    public PcInputBuffer inputBuffer;
    public AnimRootMovBroadcaster capsuleCharRootMvmtBroadcaster;
    public Animator capsuleCharAnim;
    public CapsuleCharWeapon weapon;
    public CapsuleCharHitRecieveHandler hitRecieverHandler;
    public PlrCtrl plrCtrl;
    public CamMgr camMgr;

    /// <summary>
    /// Used to create animation events decoupled from the Animator.
    /// </summary>
    [HideInInspector] public AnimEventPlr animEventPlr;

    public Vector2 Input_Mov { get; private set; }
    public bool Input_Atk_Light { get; private set; }
    public bool Input_Atk_Heavy { get; private set; }
    public bool Input_Atk_Ult { get; private set; }
    public CapsuleCharacterData Data {
        get => capsuleCharRegisterer.Data;
        set => capsuleCharRegisterer.Data = value;
    }
    public bool Input_Dodge { get; private set; }
    public Vector3 AnimationDeltaMovement { get; private set; }

    void OnEnable(){
        capsuleCharRootMvmtBroadcaster.OnRootMove += OnAnimatorRootMove;
    }

    void Start(){
        // Enter initial state:
        fsm.SwitchSt(fsmSts.idle);
    }

    void FixedUpdate(){
        fsm.CurSt.PhysicsTick();
    }

    void Update() {
        UpdateInput();
        fsm.CurSt.Tick();
    }

    private void LateUpdate() {
        fsm.CurSt.LateTick();
    }

    void OnDisable(){
        capsuleCharRootMvmtBroadcaster.OnRootMove -= OnAnimatorRootMove;
    }

    void UpdateInput(){
        Input_Atk_Light = plrCtrl.TryConsume_Atk_Light();
        Input_Atk_Heavy = plrCtrl.TryConsume_Atk_Heavy();
        Input_Atk_Ult = plrCtrl.TryConsume_Atk_Ult();
        Input_Dodge = plrCtrl.TryConsume_Dodge();
        Input_Mov = plrCtrl.Input_Mov;
        if (Input_Atk_Light)
            inputBuffer.BufferInput(BufferableInput.Atk_Light);
        else if (Input_Atk_Heavy)
            inputBuffer.BufferInput(BufferableInput.Atk_Heavy);
        else if (Input_Atk_Ult)
            inputBuffer.BufferInput(BufferableInput.Atk_Ult);
        else if (Input_Dodge)
            inputBuffer.BufferInput(BufferableInput.Dodge);
    }

    /// <summary>
    /// Used to save latest animation delta movement. Makes y component 0.
    /// </summary>
    /// <param name="deltaLinearMovement">
    /// Delta movement of animation root.
    /// </param>
    void OnAnimatorRootMove(Vector3 deltaLinearMovement){
        AnimationDeltaMovement = deltaLinearMovement;
    }
}
