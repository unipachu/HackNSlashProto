using UnityEngine;

/// <summary>
/// Used for a basic character that can do actions.
/// TODO: This makes no sense. Pawn should simply be something that can be posessed by "IPawnController" similarly to Unreal Engine.
/// </summary>
public interface IPawn
{
    CharacterLocomotion Movement { get; }
    EquipmentController Equipment { get; }
    AN_CharacterVisuals CustomAnimator { get; }
    float InputBufferTime { get; }
    bool AttackInput { get; }
    Vector2 MoveInput { get; }
    GameObject ThisObject { get; }
    public Vector3 AnimationDeltaMovement { get; }


    ActionStateRequestResult RequestFullBodyAction(ACS_FullBody newAction);

    void UpdateActionControllers();
}
