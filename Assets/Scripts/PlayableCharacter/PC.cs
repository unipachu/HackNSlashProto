using UnityEngine;

/// <summary>
/// Playable character.
/// </summary>
public class PC : MonoBehaviour
{
    [Header("Data Refs")]
    public PC_BaseData baseData;

    [Header("Component Refs")]
    public CharacterLocomotion locomotion;
    public PCVisComponents visComponents;
    public FSM fSM;
    public FSM_PCStates fSMStates;
    public PCInputBuffer inputBuffer;

    public Vector2 MoveInput { get; private set; }
    public bool Atk1Input { get; private set; }
    public bool Atk2Input { get; private set; }
    public bool Atk3Input { get; private set; }
    public bool DodgeInput { get; private set; }
    public Vector3 AnimationDeltaMovement { get; private set; }

    private void OnEnable()
    {
        visComponents.rootMvmtBroadcaster.OnRootMove += OnAnimatorRootMove;
    }

    private void Start()
    {
        // Enter initial state:
        fSM.SwitchState(fSMStates.idle);
    }

    private void FixedUpdate()
    {
        fSM.CurrentState.PhysicsTick();
    }

    private void Update()
    {
        fSM.CurrentState.Tick();
    }

    private void OnDisable()
    {
        visComponents.rootMvmtBroadcaster.OnRootMove -= OnAnimatorRootMove;
    }

    // TODO: Maybe create a PC_ControllerInput class with IPawn that can consume input from Controllers.
    public void UpdateInput(Vector2 newMoveInput, bool newAtk1Input, bool newAtk2Input, bool newAtk3Input, bool newDodgeInput)
    {
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
    private void OnAnimatorRootMove(Vector3 deltaLinearMovement)
    {
        AnimationDeltaMovement = deltaLinearMovement;
    }
}
