using System.Diagnostics;
using UnityEngine;

public class AoSTest5 : MonoBehaviour {
    const int entityCount = 50_000;

    enum State : byte {
        A, B, C, D, E, F, G, H, I, J, K, L, M,
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }

    struct Entity {
        public State a;
        public byte b;
        public byte c;
        public byte d;
        public byte e;
        public byte f;
        public byte g;
        public byte h;
        public byte i;
        public byte j;
        public byte k;
        public byte l;
        public byte m;
        public byte n;
        public byte o;
        public byte p;
        public byte q;
        public byte r;
        public byte s;
        public byte t;
        public byte u;
        public byte v;
        public byte w;
        public byte x;
        public byte y;
        public byte z;
        public byte aa;
        public byte ab;
        public byte ac;
    }

    Entity[] entities;

    Stopwatch stopwatch = new Stopwatch();

    void Start() {
        entities = new Entity[entityCount];
        System.Random random = new System.Random(12345);
        // Randomize the FSM states.
        for (int i = 0; i < entityCount; i++)
            entities[i].a = (State)random.Next(26);
    }

    void Update() {
        stopwatch.Restart();

        for (int i = 0; i < entities.Length; i++) {
            ref Entity e = ref entities[i];
            switch (e.a) {
                case State.A:
                    e.b++;
                    e.c++;
                    e.d++;
                    break;
                case State.B:
                    e.c++;
                    e.d++;
                    e.e++;
                    break;
                case State.C:
                    e.d++;
                    e.e++;
                    e.f++;
                    break;
                case State.D:
                    e.e++;
                    e.f++;
                    e.g++;
                    break;
                case State.E:
                    e.f++;
                    e.g++;
                    e.h++;
                    break;
                case State.F:
                    e.g++;
                    e.h++;
                    e.i++;
                    break;
                case State.G:
                    e.h++;
                    e.i++;
                    e.j++;
                    break;
                case State.H:
                    e.i++;
                    e.j++;
                    e.k++;
                    break;
                case State.I:
                    e.j++;
                    e.k++;
                    e.l++;
                    break;
                case State.J:
                    e.k++;
                    e.l++;
                    e.m++;
                    break;
                case State.K:
                    e.l++;
                    e.m++;
                    e.n++;
                    break;
                case State.L:
                    e.m++;
                    e.n++;
                    e.o++;
                    break;
                case State.M:
                    e.n++;
                    e.o++;
                    e.p++;
                    break;
                case State.N:
                    e.o++;
                    e.p++;
                    e.q++;
                    break;
                case State.O:
                    e.p++;
                    e.q++;
                    e.r++;
                    break;
                case State.P:
                    e.q++;
                    e.r++;
                    e.s++;
                    break;
                case State.Q:
                    e.r++;
                    e.s++;
                    e.t++;
                    break;
                case State.R:
                    e.s++;
                    e.t++;
                    e.u++;
                    break;
                case State.S:
                    e.t++;
                    e.u++;
                    e.v++;
                    break;
                case State.T:
                    e.u++;
                    e.v++;
                    e.w++;
                    break;
                case State.U:
                    e.v++;
                    e.w++;
                    e.x++;
                    break;
                case State.V:
                    e.w++;
                    e.x++;
                    e.y++;
                    break;
                case State.W:
                    e.x++;
                    e.y++;
                    e.z++;
                    break;
                case State.X:
                    e.y++;
                    e.z++;
                    e.aa++;
                    break;
                case State.Y:
                    e.z++;
                    e.aa++;
                    e.ab++;
                    break;
                case State.Z:
                    e.aa++;
                    e.ab++;
                    e.ac++;
                    break;
            }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(
            $"AoS Update: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
    }
}