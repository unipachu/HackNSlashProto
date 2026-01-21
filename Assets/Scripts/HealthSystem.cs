using UnityEngine;

public class HealthSystem : MonoBehaviour, IHittable
{
    [SerializeField] int _maxHP;

    int _currentHP;

    public int CurrentHealth => _currentHP;

    public int MaxHealth => _maxHP;

    public bool IsDead => _maxHP <= 0;

    public void GetHit(NewHitData hitData)
    {
        throw new System.NotImplementedException();
    }
}
