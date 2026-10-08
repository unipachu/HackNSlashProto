using UnityEngine;

/// <summary>
/// Target for a navigation agent to follow.
/// </summary>
public interface IFollowTgt{
    /// <summary>
    /// Navigation target, expected to be on (very close to) the NavMesh.
    /// </summary>
    Transform TrfToFollow { get; }

    /// <summary>
    /// Is the <see cref="TrfToFollow"/> on NavMesh, i.e. is it a valid follow target?
    /// </summary>
    /// <returns></returns>
    bool IsOnNavMesh();
}
