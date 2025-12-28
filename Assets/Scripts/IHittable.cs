using UnityEngine;

public interface IHittable
{
    void GetHit(int dmgAmount, Vector3 impactDirection);

    int CurrentHealth { get; }

    int MaxHealth { get; }

    bool IsDead { get; }
}
