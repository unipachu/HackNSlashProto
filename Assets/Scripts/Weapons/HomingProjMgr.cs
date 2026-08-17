using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Overengineered homing projectile manager using BurstCompiled jobs.
/// </summary>
public class HomingProjMgr : Singleton<HomingProjMgr> {
    [SerializeField] HomingProj homingProjPrefab;
    [SerializeField] int numOfPooledProj = 1000;

    HomingProj[] pooledProj;
    NativeArray<float3> positions;
    NativeArray<float3> directions;
    NativeArray<float> speeds;
    NativeArray<float> homingStrengths;
    NativeArray<float> curLifetimes;
    NativeArray<float> maxLifetimes;
    NativeArray<float3> tgtPositions;
    NativeArray<bool> hasTargets;
    NativeArray<bool> isActive;
    JobHandle movJobHandle;

    override protected void Awake() {
        base.Awake();
        pooledProj = new HomingProj[numOfPooledProj];
        positions = new NativeArray<float3>(numOfPooledProj, Allocator.Persistent);
        directions = new NativeArray<float3>(numOfPooledProj, Allocator.Persistent);
        speeds = new NativeArray<float>(numOfPooledProj, Allocator.Persistent);
        homingStrengths = new NativeArray<float>(numOfPooledProj, Allocator.Persistent);
        curLifetimes = new NativeArray<float>(numOfPooledProj, Allocator.Persistent);
        maxLifetimes = new NativeArray<float>(numOfPooledProj, Allocator.Persistent);
        tgtPositions = new NativeArray<float3>(numOfPooledProj, Allocator.Persistent);
        hasTargets = new NativeArray<bool>(numOfPooledProj, Allocator.Persistent);
        isActive = new NativeArray<bool>(numOfPooledProj, Allocator.Persistent);
        for (int i = 0; i < numOfPooledProj; i++) {
            HomingProj proj = Instantiate(homingProjPrefab, transform);
            proj.poolI = i;
            proj.gameObject.SetActive(false);
            pooledProj[i] = proj;
        }
    }

    void Update() {
        // Update target positions.
        for (int i = 0; i < numOfPooledProj; i++) {
            if (isActive[i] == false)
                continue;
            if (pooledProj[i].TryGetTgtPos(out Vector3 targetPosition)) {
                tgtPositions[i] = targetPosition;
                hasTargets[i] = true;
            }
            else
                hasTargets[i] = false;
        }
        // Move projectiles with jobs.
        movJobHandle = new HomingProjMovementJob {
            dt = Time.deltaTime,
            positions = positions,
            directions = directions,
            speeds = speeds,
            homingStrengths = homingStrengths,
            curLifetimes = curLifetimes,
            maxLifetimes = maxLifetimes,
            tgtPositions = tgtPositions,
            hasTargets = hasTargets,
            isActive = isActive
        }.Schedule(numOfPooledProj, 64);
        // TODO: For more parallelism we could actually apply the homing projectile movement to the
        // TODO C: Transforms later but what ever.
        movJobHandle.Complete();
        // Set projectile pose or deactivate if max lifetime reached.
        for (int i = 0; i < numOfPooledProj; i++) {
            if (isActive[i] == false)
                continue;
            if (curLifetimes[i] >= maxLifetimes[i]) {
                DeactivateProj(i);
                continue;
            }
            HomingProj proj = pooledProj[i];
            proj.transform.SetPositionAndRotation(
                positions[i],
                Quaternion.LookRotation(directions[i])
            );
        }
    }

    /// <summary>
    /// Shoot a homing projectile.
    /// </summary>
    public void ShootProj(
        HomingProjData projData,
        Vector3 wldStartPos,
        Vector3 wldStartDir,
        Transform tgt
    ) {
        int projIndex = GetInactivePooledProj();
        if (projIndex < 0)
            return;
        float3 direction = math.normalize((float3)wldStartDir);
        positions[projIndex] = wldStartPos;
        directions[projIndex] = direction;
        speeds[projIndex] = projData.spd;
        homingStrengths[projIndex] = projData.homingStr;
        curLifetimes[projIndex] = 0;
        maxLifetimes[projIndex] = projData.maxLifetime;
        isActive[projIndex] = true;
        HomingProj proj = pooledProj[projIndex];
        proj.SetTgt(tgt);
        proj.hitData = new HitData(projData.dmg, wldStartDir);
        proj.transform.SetPositionAndRotation(
            wldStartPos,
            Quaternion.LookRotation(wldStartDir)
        );
        proj.gameObject.SetActive(true);
        proj.hitDealer.Activate();
    }

    public void DeactivateProj(int projIndex) {
        isActive[projIndex] = false;
        pooledProj[projIndex].hitDealer.Deactivate();
        pooledProj[projIndex].gameObject.SetActive(false);
    }

    int GetInactivePooledProj() {
        for (int i = 0; i < numOfPooledProj; i++) {
            if (isActive[i] == false)
                return i;
        }
        Debug.LogWarning("Not enough pooled projectiles! Pool more!", this);
        return -1;
    }

    void OnDestroy() {
        movJobHandle.Complete();
        positions.Dispose();
        directions.Dispose();
        speeds.Dispose();
        homingStrengths.Dispose();
        curLifetimes.Dispose();
        maxLifetimes.Dispose();
        tgtPositions.Dispose();
        hasTargets.Dispose();
        isActive.Dispose();
    }

    [BurstCompile]
    struct HomingProjMovementJob : IJobParallelFor {
        public float dt;
        public NativeArray<float3> positions;
        public NativeArray<float3> directions;
        [ReadOnly] public NativeArray<float> speeds;
        [ReadOnly] public NativeArray<float> homingStrengths;
        public NativeArray<float> curLifetimes;
        [ReadOnly] public NativeArray<float> maxLifetimes;
        [ReadOnly] public NativeArray<float3> tgtPositions;
        [ReadOnly] public NativeArray<bool> hasTargets;
        [ReadOnly] public NativeArray<bool> isActive;

        public void Execute(int i) {
            if (!isActive[i])
                return;
            curLifetimes[i] += dt;
            if (curLifetimes[i] >= maxLifetimes[i])
                return;
            float3 dir = directions[i];
            // If proj has no target or 0 homing str, it will continue into its current movement dir.
            if (hasTargets[i] && homingStrengths[i] > 0) {
                float3 toTgt = tgtPositions[i] - positions[i];
                float tgtDistSq = math.lengthsq(toTgt);
                // Normalization could cause trouble if dist to tgt is zero (which it pretty
                // much will never be but what ever).
                if (tgtDistSq > 0.000001f) {
                    float3 tgtDir = math.normalize(toTgt);
                    // Saturate clamps to 0-1.
                    float homingAmt = math.saturate(homingStrengths[i] * dt);
                    dir = math.normalize(math.lerp(dir, tgtDir, homingAmt));
                }
            }
            directions[i] = dir;
            positions[i] += dir * speeds[i] * dt;
        }
    }
}
