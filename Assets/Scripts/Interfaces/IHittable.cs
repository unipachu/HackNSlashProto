using UnityEngine;

/// <summary>
/// Object that can be hit with attacks.
/// </summary>
public interface IHittable
{
    void GetHit(int dmgAmount, Vector3 attackerPosition);

    int CurrentHealth { get; }

    int MaxHealth { get; }

    bool IsDead { get; }
}
