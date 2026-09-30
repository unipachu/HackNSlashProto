using UnityEngine;

/// <summary>
/// Pools of all projectile types. Movement logic of the projectiles should be implemented by the
/// managers/systems that use <see cref="Get"/>. Proj is returned to the pool by setting <see cref="IProj.Go"/>
/// inactive.
/// </summary>
public class ProjPools : Singleton<ProjPools> {
    [Tooltip("How many objects pooled per pool in Awake?")]
    [SerializeField] int initialPoolAmount = 1;
    [SerializeField] RayProj rayProjPrefab;
    [SerializeField] SphereProj sphereProjPrefab;

    RayProj[] rayProjPool;
    int rayProjPoolUsedLength;
    SphereProj[] sphereProjPool;
    int sphereProjPoolUsedLength;

    protected override void Awake() {
        base.Awake();
        rayProjPool = new RayProj[initialPoolAmount];
        sphereProjPool = new SphereProj[initialPoolAmount];
        for (int i = 0; i < initialPoolAmount; i++) {
            SphereProj sphereProj = Instantiate(sphereProjPrefab, transform);
            sphereProj.IInMgr = -1;
            sphereProj.gameObject.SetActive(false);
            sphereProjPool[i] = sphereProj;
            sphereProjPoolUsedLength = initialPoolAmount;
            RayProj rayProj = Instantiate(rayProjPrefab, transform);
            rayProj.IInMgr = -1;
            rayProj.gameObject.SetActive(false);
            rayProjPool[i] = rayProj;
            rayProjPoolUsedLength = initialPoolAmount;
        }
    }

    /// <summary>
    /// Finds inactive object, makes it active and returns it.
    /// </summary>
    public IProj Get(ProjT projT) {
        switch (projT) {
            case ProjT.ReyProj:
                return GetPooledObj(ref rayProjPoolUsedLength, transform, ref rayProjPool, rayProjPrefab);
            case ProjT.SphereProj:
                return GetPooledObj(
                    ref sphereProjPoolUsedLength,
                    transform,
                    ref sphereProjPool,
                    sphereProjPrefab
                );
            default:
                Debug.LogError($"Switch defaulted with {projT}");
                return null;
        }
    }

    /// <summary>
    /// Gets pooled object. If none are available, instantiates a new object and returns that.
    /// </summary>
    /// <param name="usedLength">
    /// How many elements from the pool array are used from index 0 onwards.
    /// </param>
    public static T GetPooledObj<T>(
        ref int arrayUsedLength,
        Transform parent,
        ref T[] pooledObjs,
        T prefab
    ) where T : Component {
        for (int i = 0; i < arrayUsedLength; i++) {
            T obj = pooledObjs[i];
            if (obj.gameObject.activeSelf == false) {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        T newObj = Object.Instantiate(prefab, parent);
        newObj.gameObject.SetActive(true);
        arrayUsedLength = ArrayUtils.Add(ref pooledObjs, arrayUsedLength, newObj);
        return newObj;
    }
}
