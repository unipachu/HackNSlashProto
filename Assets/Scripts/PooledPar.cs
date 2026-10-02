using UnityEngine;
using UnityEngine.Pool;

public class PooledPar : MonoBehaviour {
    ObjectPool<ParticleSystem> pool;
    public bool IsInPool { get; set; }

    public void Init(ObjectPool<ParticleSystem> pool) {
        this.pool = pool;
    }

    void OnParticleSystemStopped() {
        if (IsInPool)
            return;
        IsInPool = true;
        pool.Release(GetComponent<ParticleSystem>());
    }
}