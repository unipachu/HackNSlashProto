using UnityEngine;

/// <summary>
/// Target for a navigation agent to follow.
/// </summary>
public interface IFollowTgt{
    Transform TrfToFollow { get; }

    bool IsOnNavMesh();
}
