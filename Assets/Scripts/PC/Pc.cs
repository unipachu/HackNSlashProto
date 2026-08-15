using UnityEngine;

/// <summary>
/// Playable character.
/// </summary>
public class Pc : MonoBehaviour{
    [Header("Data Refs")]
    [SerializeField] EntityRegisterer entityRegisterer;

    [Header("Component Refs")]
    public CharCtrlMov locomotion;
    public PcVisComponents visComponents;
    public Fsm fSM;
    public Fsm_PcSts fSMStates;
    public PcInputBuffer inputBuffer;

    public Vector2 MoveInput { get; private set; }
    public bool Atk1Input { get; private set; }
    public bool Atk2Input { get; private set; }
    public bool Atk3Input { get; private set; }
    public CapsuleCharacterConfig Data {
        get => entityRegisterer.Data;
        set => entityRegisterer.Data = value;
    }
    public bool DodgeInput { get; private set; }
    public Vector3 AnimationDeltaMovement { get; private set; }

    void OnEnable(){
        visComponents.rootMvmtBroadcaster.OnRootMove += OnAnimatorRootMove;
    }

    void Start(){
        // Enter initial state:
        fSM.SwitchSt(fSMStates.idle);
    }

    void FixedUpdate(){
        fSM.CurSt.PhysicsTick();
    }

    void Update() {
        fSM.CurSt.Tick();
    }

    void OnDisable(){
        visComponents.rootMvmtBroadcaster.OnRootMove -= OnAnimatorRootMove;
    }

    // TODO: Maybe create a PC_ControllerInput class with IPawn that can consume input from Controllers.
    public void UpdateInput(
        Vector2 newMoveInput,
        bool newAtk1Input,
        bool newAtk2Input,
        bool newAtk3Input,
        bool newDodgeInput
    ){
        MoveInput = newMoveInput;
        Atk1Input = newAtk1Input;
        Atk2Input = newAtk2Input;
        Atk3Input = newAtk3Input;
        DodgeInput = newDodgeInput;
        if (newAtk1Input) inputBuffer.BufferInput("atk1");
        else if (newAtk2Input) inputBuffer.BufferInput("atk2");
        else if (newAtk3Input) inputBuffer.BufferInput("atk3");
        else if (newDodgeInput) inputBuffer.BufferInput("dodge");
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
