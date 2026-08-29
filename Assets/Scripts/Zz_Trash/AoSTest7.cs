using System.Diagnostics;
using UnityEngine;

public class AoSTest7 : MonoBehaviour {
    const int entityCount = 50_000;

    enum State : byte {
        A,
        B,
        C
    }

    struct Entity {
        public State state;
        public float posX;
        public float posY;
        public float posZ;
        public float targetX;
        public float targetY;
        public float targetZ;
        public float attackRangeSq;
        public float aggroRangeSq;
    }

    Entity[] entities;

    Stopwatch stopwatch = new Stopwatch();

    void Start() {
        entities = new Entity[entityCount];

        System.Random random = new System.Random(12345);

        for (int i = 0; i < entityCount; i++) {
            entities[i].posX = (float)(random.NextDouble() * 100.0);
            entities[i].posY = (float)(random.NextDouble() * 100.0);
            entities[i].posZ = (float)(random.NextDouble() * 100.0);
            entities[i].targetX = (float)(random.NextDouble() * 100.0);
            entities[i].targetY = (float)(random.NextDouble() * 100.0);
            entities[i].targetZ = (float)(random.NextDouble() * 100.0);
            entities[i].attackRangeSq = 25.0f;
            entities[i].aggroRangeSq = 400.0f;
        }
    }

    void Update() {
        stopwatch.Restart();
        for (int i = 0; i < entities.Length; i++) {
            ref Entity e = ref entities[i];
            float dx = e.targetX - e.posX;
            float dy = e.targetY - e.posY;
            float dz = e.targetZ - e.posZ;
            float distSq = dx * dx + dy * dy + dz * dz;
            if (distSq < e.attackRangeSq)
                e.state = State.A;
            else if (distSq < e.aggroRangeSq)
                e.state = State.B;
            else
                e.state = State.C;
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(
            $"AoS Update: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
    }
}
