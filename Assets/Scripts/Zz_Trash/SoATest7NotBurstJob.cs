using System.Diagnostics;
using UnityEngine;

/// <summary>
/// SoA Test7 without burst job
/// </summary>
public class SoATest7NotBurstJob : MonoBehaviour {
    const int entityCount = 50_000;

    enum State : byte {
        A,
        B,
        C
    }

    byte[] state;
    float[] posX;
    float[] posY;
    float[] posZ;
    float[] targetX;
    float[] targetY;
    float[] targetZ;
    float[] attackRangeSq;
    float[] aggroRangeSq;

    Stopwatch stopwatch = new Stopwatch();

    void Start() {
        state = new byte[entityCount];
        posX = new float[entityCount];
        posY = new float[entityCount];
        posZ = new float[entityCount];
        targetX = new float[entityCount];
        targetY = new float[entityCount];
        targetZ = new float[entityCount];
        attackRangeSq = new float[entityCount];
        aggroRangeSq = new float[entityCount];
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

    void Update() {
        stopwatch.Restart();
        for (int i = 0; i < entityCount; i++) {
            float dx = targetX[i] - posX[i];
            float dy = targetY[i] - posY[i];
            float dz = targetZ[i] - posZ[i];
            float distSq = dx * dx + dy * dy + dz * dz;
            if (distSq < attackRangeSq[i])
                state[i] = (byte)State.A;
            else if (distSq < aggroRangeSq[i])
                state[i] = (byte)State.B;
            else
                state[i] = (byte)State.C;
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(
            $"SoA Update: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
    }
}
