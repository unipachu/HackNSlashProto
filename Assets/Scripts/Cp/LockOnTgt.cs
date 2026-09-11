// TODO: Rename to ILockOnTgt
using UnityEngine;

/// <summary>
/// Capsule pawn that has a lock on point for attacks etc.<br/>
/// </summary>
public interface LockOnTgt {
    int Id { get; }
    Transform Trf { get; }
}
