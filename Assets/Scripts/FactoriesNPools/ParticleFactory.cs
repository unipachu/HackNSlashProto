using UnityEngine;
using UnityEngine.Pool;

public class ParticleFactory : Singleton<ParticleFactory> {
    [Header("Prefabs")]
    [SerializeField] ParticleSystem sparksParPrefab;
    [SerializeField] ParticleSystem bloodParPrefab;
    [SerializeField] ParticleSystem smokeParPrefab;

    [Header("Pool Sizes")]
    [SerializeField, Min(0)] int sparksPoolSz = 8;
    [SerializeField, Min(0)] int bloodPoolSz = 8;
    [SerializeField, Min(0)] int smokePoolSz = 16;
    [SerializeField, Min(1)] int maxSparksPoolSz = 64;
    [SerializeField, Min(1)] int maxBloodPoolSz = 64;
    [SerializeField, Min(1)] int maxSmokePoolSz = 64;

    ObjectPool<ParticleSystem> sparksPool;
    ObjectPool<ParticleSystem> bloodPool;
    ObjectPool<ParticleSystem> smokePool;

    public void Init() {
        sparksPool = CreatePool(sparksParPrefab, sparksPoolSz, maxSparksPoolSz);
        bloodPool = CreatePool(bloodParPrefab, bloodPoolSz, maxBloodPoolSz);
        smokePool = CreatePool(smokeParPrefab, smokePoolSz, maxSmokePoolSz);
    }

    public void PlaySparks(Vector3 pos, Vector3 dir) {
        if (sparksPool == null)
            return;
        Play(sparksPool, pos, dir);
    }

    public void PlayBlood(Vector3 pos, Vector3 dir) {
        if (bloodPool == null)
            return;
        Play(bloodPool, pos, dir);
    }

    public void PlaySmoke(Vector3 pos) {
        if (smokePool == null)
            return;
        ParticleSystem par = smokePool.Get();
        par.transform.SetPositionAndRotation(pos, Quaternion.identity);
        par.Play(true);
    }

    ObjectPool<ParticleSystem> CreatePool(ParticleSystem parPrefab, int poolSz, int maxPoolSz) {
        if (parPrefab == null) {
            Debug.LogError($"{nameof(ParticleFactory)} is missing a particle prefab.", this);
            return null;
        }
        ObjectPool<ParticleSystem> pool = null;
        pool = new ObjectPool<ParticleSystem>(
            () => CreatePar(parPrefab, pool),
            par => {
                par.gameObject.SetActive(true);
                par.GetComponent<PooledPar>().IsInPool = false;
            },
            par => {
                par.GetComponent<PooledPar>().IsInPool = true;
                par.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                par.gameObject.SetActive(false);
            },
            par => {
                if (par != null)
                    Destroy(par.gameObject);
            },
            true,
            poolSz,
            Mathf.Max(poolSz, maxPoolSz)
        );
        for (int i = 0; i < poolSz; i++) {
            ParticleSystem par = pool.Get();
            pool.Release(par);
        }
        return pool;
    }

    static ParticleSystem CreatePar(ParticleSystem parPrefab, ObjectPool<ParticleSystem> pool) {
        ParticleSystem par = Instantiate(parPrefab);
        PooledPar pooledPar = par.GetComponent<PooledPar>();
        if (pooledPar == null)
            pooledPar = par.gameObject.AddComponent<PooledPar>();
        pooledPar.Init(pool);
        pooledPar.IsInPool = false;
        ParticleSystem.MainModule main = par.main;
        main.stopAction = ParticleSystemStopAction.Callback;
        par.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        par.gameObject.SetActive(false);
        return par;
    }

    static void Play(ObjectPool<ParticleSystem> pool, Vector3 pos, Vector3 dir) {
        ParticleSystem par = pool.Get();
        Quaternion rot = dir.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(dir)
            : Quaternion.identity;
        par.transform.SetPositionAndRotation(pos, rot);
        par.Play(true);
    }
}