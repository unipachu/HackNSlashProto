using UnityEngine;

public class HealthSystem : MonoBehaviour, IHittable
{
    [SerializeField] int _maxHP = 1;

    int _currentHP;

    public int CurrentHealth => _currentHP;

    public int MaxHealth => _maxHP;

    public bool IsDead => _maxHP <= 0;

    private void Awake()
    {
        _currentHP = _maxHP;
    }

    public void GetHit(NewHitData hitData)
    {
        // TODO
        throw new System.NotImplementedException();
    }
}
