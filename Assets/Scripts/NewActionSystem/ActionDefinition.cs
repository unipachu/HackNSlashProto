using System;
using UnityEngine;

// TODO: Figure out better types as you need.
public enum ActionType
{
    LightAttack,
    HeavyAttack,
    Dodge,
    JumpAttack,
    HitReaction
}

/// <summary>
/// The lower the higher the priority (can interrupt lower priority actions), you could give numbers to these so that you can do checks
/// against those numbers instead of 
/// </summary>
public enum ActionPriority
{
    Locomotion,
    Attack,
    Dodge,
    HitReaction,
    Knockdown,
    Death
}

/// <summary>
/// Action is a state representing what a game object is currently doing.
/// TODO: Make this into an abstract class. Add Hyperarmor etc. to a child class. This is base class for ALL ACTIONS.
/// </summary>
[CreateAssetMenu(menuName = "Combat/Action Definition")]
public class ActionDefinition : ScriptableObject
{
    [Header("Identity")]
    private ActionType _actionType;
    [Tooltip("Animator facing id")]
    private int _actionID;

    [Header("Animation")]
    [Tooltip("Semantic name")]
    // TODO: You might want to create a "CustomAnimationData" which hold all of the info below as well as layer index, events, and whether 
    // TODO C: events should be invoked if animation is started from some different point than beginning - or maybe the CustomAnimator
    // TODO C: could handle that.
    private string _animatorStateName;
    private bool _useRootMotion = true;
    private bool _fullBody = true;

    [Header("Timing (normalized)")]
    [Range(0f, 1f)] private float _canChainFrom = 0.6f;
    [Range(0f, 1f)] private float _canCancelFrom = 0.3f;
    [Range(0f, 1f)] private float _endAt = 0.95f;

    [Header("Stamina")]
    private float _staminaCost;

    [Header("Next Actions")]
    [Tooltip("Combo attacks etc.")]
    private ActionDefinition[] _chainableActions;

    [Header("Priority")]
    private ActionPriority _priority;
    private bool _uninterruptible;

    [Header("Interruption Rules")]
    private ActionPriority _minPriorityToInterrupt = ActionPriority.HitReaction;

    // NOTE: "uninterruptible" overrides hyperarmor:
    [Header("Hyper Armor")]
    [Range(0f, 1f)] private float _hyperArmorFrom = 0.2f;
    [Range(0f, 1f)] private float _hyperArmorTo = 0.7f;

    [Header("Hit Data")]
    public HitWindow[] _hitWindows;

    [Header("Invulnerability")]
    [Range(0f, 1f)] private float _iFrameFrom;
    [Range(0f, 1f)] private float _iFrameTo;

    public ActionType ActionType { get => _actionType; }
    public int ActionID { get => _actionID; }
    public string AnimatorStateName { get => _animatorStateName; }
    public bool UseRootMotion { get => _useRootMotion; }
    public bool FullBody { get => _fullBody; }
    public float CanChainFrom { get => _canChainFrom; }
    public float CanCancelFrom { get => _canCancelFrom; }
    public float EndAt { get => _endAt; }
    public float StaminaCost { get => _staminaCost; }
    public ActionDefinition[] ChainableActions { get => _chainableActions; }
    public ActionPriority Priority { get => _priority;}
    public bool Uninterruptible { get => _uninterruptible; }
    public ActionPriority MinPriorityToInterrupt { get => _minPriorityToInterrupt; }
    public float HyperArmorFrom { get => _hyperArmorFrom; }
    public float HyperArmorTo { get => _hyperArmorTo; }
    public float IFrameFrom { get => _iFrameFrom; }
    public float IFrameTo { get => _iFrameTo; }
}
