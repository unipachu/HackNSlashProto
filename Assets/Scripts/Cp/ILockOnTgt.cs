using UnityEngine;

/// <summary>
/// Capsule pawn that has a lock on point for attacks etc.<br/>
/// </summary>
public interface ILockOnTgt {
    /// <summary>
    /// CpId of the target.
    /// </summary>
    int Id { get; }
    /// <summary>
    /// Transform of the target.
    /// </summary>
    Transform Trf { get; }
}
