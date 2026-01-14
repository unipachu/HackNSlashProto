using UnityEngine;

/// <summary>
/// Represents the time when an attack is active.
/// </summary>
// TODO: Think a way to add this to "animation data". You want just one script/data for one animation.
[System.Serializable]
public struct HitWindow
{
    // TODO: Tooltips explaining these are normalized.
    [Range(0f, 1f)] public float WindowStart;
    [Range(0f, 1f)] public float WindowEnd;
    // Use HitData instead.
    public float Damage;
    public float Knockback;
}
