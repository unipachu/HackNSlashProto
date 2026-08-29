using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// Burst job, more complex calculations than the previous.
/// </summary>
public class SoATest7 : MonoBehaviour {
    const int entityCount = 50_000;

    enum State : byte {
        A,
        B,
        C
    }

    NativeArray<byte> state;
    NativeArray<float> posX;
    NativeArray<float> posY;
    NativeArray<float> posZ;
    NativeArray<float> targetX;
    NativeArray<float> targetY;
    NativeArray<float> targetZ;
    NativeArray<float> attackRangeSq;
    NativeArray<float> aggroRangeSq;

    Stopwatch stopwatch = new Stopwatch();

    [BurstCompile]
    struct UpdateJob : IJobParallelFor {
        public NativeArray<byte> state;
        [ReadOnly] public NativeArray<float> posX;
        [ReadOnly] public NativeArray<float> posY;
        [ReadOnly] public NativeArray<float> posZ;
        [ReadOnly] public NativeArray<float> targetX;
        [ReadOnly] public NativeArray<float> targetY;
        [ReadOnly] public NativeArray<float> targetZ;
        [ReadOnly] public NativeArray<float> attackRangeSq;
        [ReadOnly] public NativeArray<float> aggroRangeSq;

        public void Execute(int ii) {
            float dx = targetX[ii] - posX[ii];
            float dy = targetY[ii] - posY[ii];
            float dz = targetZ[ii] - posZ[ii];
            float distSq = dx * dx + dy * dy + dz * dz;
            if (distSq < attackRangeSq[ii])
                state[ii] = (byte)State.A;
            else if (distSq < aggroRangeSq[ii])
                state[ii] = (byte)State.B;
            else
                state[ii] = (byte)State.C;
        }
    }

    void Start() {
        state = NewByteArray();
        posX = NewFloatArray();
        posY = NewFloatArray();
        posZ = NewFloatArray();
        targetX = NewFloatArray();
        targetY = NewFloatArray();
        targetZ = NewFloatArray();
        attackRangeSq = NewFloatArray();
        aggroRangeSq = NewFloatArray();
        System.Random random = new System.Random(12345);
        for (int i = 0; i < entityCount; i++) {
            posX[i] = (float)(random.NextDouble() * 100.0);
            posY[i] = (float)(random.NextDouble() * 100.0);
            posZ[i] = (float)(random.NextDouble() * 100.0);
            targetX[i] = (float)(random.NextDouble() * 100.0);
            targetY[i] = (float)(random.NextDouble() * 100.0);
            targetZ[i] = (float)(random.NextDouble() * 100.0);
            attackRangeSq[i] = 25.0f;
            aggroRangeSq[i] = 400.0f;
        }
    }

    NativeArray<byte> NewByteArray() {
        return new NativeArray<byte>(
            entityCount,
            Allocator.Persistent);
    }

    NativeArray<float> NewFloatArray() {
        return new NativeArray<float>(
            entityCount,
            Allocator.Persistent);
    }

    void Update() {
        stopwatch.Restart();
        UpdateJob job = new UpdateJob {
            state = state,
            posX = posX,
            posY = posY,
            posZ = posZ,
            targetX = targetX,
            targetY = targetY,
            targetZ = targetZ,
            attackRangeSq = attackRangeSq,
            aggroRangeSq = aggroRangeSq
        };
        JobHandle handle = job.Schedule(entityCount, 64);
        handle.Complete();
        stopwatch.Stop();
        UnityEngine.Debug.Log(
            $"Burst SoA Job Update: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
    }

    void OnDestroy() {
        Dispose(ref state);
        Dispose(ref posX);
        Dispose(ref posY);
        Dispose(ref posZ);
        Dispose(ref targetX);
        Dispose(ref targetY);
        Dispose(ref targetZ);
        Dispose(ref attackRangeSq);
        Dispose(ref aggroRangeSq);
    }

    static void Dispose<T>(ref NativeArray<T> array)
        where T : unmanaged {
        if (array.IsCreated)
            array.Dispose();
    }
}
