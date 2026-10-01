using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Overengineered homing projectile manager using BurstCompiled jobs.
/// </summary>
// TODO MAYBE: Projectile collisions can also be burst jobbed: https://youtu.be/WrzrtMxLgpc?si=SZRfdWvTdvntndu5
public class HomingProjMgr : Singleton<HomingProjMgr> {
    [SerializeField] int initCapacity = 10;

    NativeList<float> curLifetime;
    NativeList<float3> dir;
    int entityCount = 0;
    NativeList<bool> hasTgt;
    NativeList<float> homingStr;
    NativeList<float> maxLifetime;
    JobHandle movJobHandle;
    NativeList<float3> pos;
    IProj[] projObjs;
    NativeList<float> spd;
    NativeList<float3> tgtPos;

    override protected void Awake() {
        base.Awake();
        curLifetime = new NativeList<float>(initCapacity, Allocator.Persistent);
        dir = new NativeList<float3>(initCapacity, Allocator.Persistent);
        hasTgt = new NativeList<bool>(initCapacity, Allocator.Persistent);
        homingStr = new NativeList<float>(initCapacity, Allocator.Persistent);
        maxLifetime = new NativeList<float>(initCapacity, Allocator.Persistent);
        pos = new NativeList<float3>(initCapacity, Allocator.Persistent);
        projObjs = new IProj[initCapacity];
        spd = new NativeList<float>(initCapacity, Allocator.Persistent);
        tgtPos = new NativeList<float3>(initCapacity, Allocator.Persistent);
    }

    void Update() {
        // Update target positions.
        for (int i = 0; i < entityCount; i++) {
            if (projObjs[i].Tgt != null) {
                tgtPos[i] = projObjs[i].Tgt.position;
                hasTgt[i] = true;
            }
            else
                hasTgt[i] = false;
        }
        // Move projectiles with jobs.
        movJobHandle = new HomingProjMovementJob {
            dt = Time.deltaTime,
            // NOTE: Job needs the collections as native arrays. Apparently casting to array with AsArray() will
            // NOTE C: still use the same memory the NativeList uses.
            dir = dir.AsArray(),
            pos = pos.AsArray(),
            spd = spd.AsArray(),
            homingStr = homingStr.AsArray(),
            curLifetime = curLifetime.AsArray(),
            maxLifetimes = maxLifetime.AsArray(),
            tgtPos = tgtPos.AsArray(),
            hasTgt = hasTgt.AsArray(),
        }.Schedule(entityCount, 64);
        movJobHandle.Complete();
        // Update projectile Monobehaviour.
        int j = 0;
        while (j < entityCount) {
            if (curLifetime[j] >= maxLifetime[j]) {
                DeactivateProj(j);
                continue;
            }
            IProj proj = projObjs[j];
            proj.HitDealer.HitDealerMovDir = dir[j];
            proj.Trf.SetPositionAndRotation(
                pos[j],
                Quaternion.LookRotation(dir[j])
            );
            j++;
        }
    }

    void OnDestroy() {
        curLifetime.Dispose();
        dir.Dispose();
        hasTgt.Dispose();
        homingStr.Dispose();
        maxLifetime.Dispose();
        pos.Dispose();
        spd.Dispose();
        tgtPos.Dispose();
    }

    /// <summary>
    /// Shoot a homing projectile.
    /// </summary>
    public void ShootProj(
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HomingProjData projData,
        ProjT projT,
        HashSet<IHitReceiver> shooterHitRecievers,
        Transform tgt,
        Team team,
        Vector3 wldStartPos,
        Vector3 wldStartDir
    ) {
        int newI = entityCount;
        IProj proj = null;
        switch (projT) {
            case ProjT.ReyProj:
                proj = ProjPools.inst.Get(ProjT.ReyProj);
                break;
            case ProjT.SphereProj:
                proj = ProjPools.inst.Get(ProjT.SphereProj);
                break;
            default:
                Debug.LogError($"Switch defaulted with: {projT}");
                break;
        }
        curLifetime.Add(0);
        dir.Add(wldStartDir.normalized);
        hasTgt.Add(false);
        homingStr.Add(projData.homingStr);
        maxLifetime.Add(projData.maxLifetime);
        pos.Add(wldStartPos);
        ArrayUtils.Add(ref projObjs, entityCount, proj);
        spd.Add(projData.spd);
        tgtPos.Add(float3.zero);
        proj.IInMgr = entityCount;
        proj.Tgt = tgt;
        proj.Trf.SetPositionAndRotation(wldStartPos, Quaternion.LookRotation(wldStartDir));
        proj.TrailRenderer.Clear();
        proj.HitDealer.ResetNActivate(
            proj.Trf,
            hitDirMode,
            hitEffects,
            shooterHitRecievers,
            team,
            wldStartDir
        );
        entityCount++;
    }

    public void DeactivateProj(int projI) {
        int lastId = entityCount - 1;
        IProj swappedProj = projI != lastId
            ? projObjs[lastId]
            : null;
        projObjs[projI].HitDealer.Deactivate();
        projObjs[projI].Go.SetActive(false);
        projObjs[projI].IInMgr = -1;
        // REmove at swap back
        curLifetime.RemoveAtSwapBack(projI);
        dir.RemoveAtSwapBack(projI);
        hasTgt.RemoveAtSwapBack(projI);
        homingStr.RemoveAtSwapBack(projI);
        maxLifetime.RemoveAtSwapBack(projI);
        pos.RemoveAtSwapBack(projI);
        ArrayUtils.RemoveAtSwapBack(projObjs, entityCount, projI);
        spd.RemoveAtSwapBack(projI);
        tgtPos.RemoveAtSwapBack(projI);
        entityCount--;
        if (swappedProj != null)
            swappedProj.IInMgr = projI;
    }

    [BurstCompile]
    struct HomingProjMovementJob : IJobParallelFor {
        public NativeArray<float> curLifetime;
        public NativeArray<float3> dir;
        public float dt;
        [ReadOnly] public NativeArray<bool> hasTgt;
        [ReadOnly] public NativeArray<float> homingStr;
        [ReadOnly] public NativeArray<float> maxLifetimes;
        public NativeArray<float3> pos;
        [ReadOnly] public NativeArray<float> spd;
        [ReadOnly] public NativeArray<float3> tgtPos;

        public void Execute(int i) {
            curLifetime[i] += dt;
            float movDur = dt;
            if (curLifetime[i] >= maxLifetimes[i]) {
                // If the proj would die this update, we still move it the amount it would've moved
                // otherwise. This is probably useless but what ever.
                movDur = dt - (curLifetime[i] - maxLifetimes[i]);
            }
            float3 newDir = dir[i];
            // If proj has no target or 0 homing str, it will continue into its current movement dir.
            if (hasTgt[i] && homingStr[i] > 0) {
                float3 toTgt = tgtPos[i] - pos[i];
                float tgtDistSq = math.lengthsq(toTgt);
                // Normalization could cause trouble if dist to tgt is zero (which it pretty
                // much will never be but what ever).
                if (tgtDistSq > 0.000001f) {
                    float3 tgtDir = math.normalize(toTgt);
                    // Saturate clamps to 0-1.
                    float homingAmt = math.saturate(homingStr[i] * movDur);
                    newDir = math.normalize(math.lerp(newDir, tgtDir, homingAmt));
                }
            }
            dir[i] = newDir;
            pos[i] += newDir * spd[i] * movDur;
        }
    }
}
