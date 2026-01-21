using UnityEngine;

/// <summary>
/// Object that can be hit with attacks.
/// </summary>
public interface IHittable
{
    void GetHit(NewHitData hitData);

    int CurrentHealth { get; }

    int MaxHealth { get; }

    bool IsDead { get; }

    // TODO: Remove.
    public bool IsAlive(float something)
    {
        return true;
    }
}
