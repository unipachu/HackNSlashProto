using UnityEngine;

/// <summary>
/// Used for a basic character that can do actions.
/// </summary>
public interface IActionCharacter
{
    CharacterLocomotion Movement { get; }
    EquipmentController Equipment { get; }
    AN_CharacterVisuals CustomAnimator { get; }
    float InputBufferTime { get; }
    bool AttackInput { get; }
    Vector2 MoveInput { get; }
    GameObject ThisObject { get; }
    Vector3 AnimationDeltaMovement { get; }

    ActionStateRequestResult RequestFullBodyAction(ACS_FullBody newAction);

    void UpdateActionControllers();
}
