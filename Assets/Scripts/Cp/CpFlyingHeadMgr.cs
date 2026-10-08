using UnityEngine;

public class CpFlyingHeadMgr : Singleton<CpFlyingHeadMgr> {
    [Tooltip("Initial capacity of arrays. They allocate more space if needed (but do not deallocate even" +
    "if pawns are unregistered.)")]
    [SerializeField] int initCapacity = 1;

    [HideInInspector] public CpHumd_Data[] aos;

    public void Register(CpFlyingHeadHandle cpFlyingHead) {
        Debug.LogError("Not implemented", this);
    }
}
