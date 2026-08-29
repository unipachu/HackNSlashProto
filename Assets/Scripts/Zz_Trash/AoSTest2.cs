using System.Diagnostics;
using UnityEngine;

public class AoSTest2 : MonoBehaviour{
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
        public byte ad;
        public byte ae;
        public byte af;
        public byte ag;
        public byte ah;
        public byte ai;
        public byte aj;
        public byte ak;
        public byte al;
        public byte am;
        public byte an;
        public byte ao;
        public byte ap;
        public byte aq;
        public byte ar;
        public byte @as;
        public byte at;
        public byte au;
        public byte av;
        public byte aw;
        public byte ax;
        public byte ay;
        public byte az;
        public byte ba;
        public byte bb;
        public byte bc;
        public byte bd;
        public byte be;
        public byte bf;
        public byte bg;
        public byte bh;
        public byte bi;
        public byte bj;
        public byte bk;
        public byte bl;
        public byte bm;
        public byte bn;
        public byte bo;
        public byte bp;
        public byte bq;
        public byte br;
        public byte bs;
        public byte bt;
        public byte bu;
        public byte bv;
        public byte bw;
        public byte bx;
        public byte by;
        public byte bz;
        public byte ca;
        public byte cb;
        public byte cc;
        public byte cd;
        public byte ce;
        public byte cf;
        public byte cg;
        public byte ch;
        public byte ci;
        public byte cj;
        public byte ck;
        public byte cl;
        public byte cm;
        public byte cn;
        public byte co;
        public byte cp;
        public byte cq;
        public byte cr;
        public byte cs;
        public byte ct;
        public byte cu;
        public byte cv;
        public byte cw;
        public byte cx;
        public byte cy;
        public byte cz;
        public byte da;
        public byte db;
        public byte dc;
        public byte dd;
        public byte de;
        public byte df;
        public byte dg;
        public byte dh;
        public byte di;
        public byte dj;
        public byte dk;
        public byte dl;
        public byte dm;
        public byte dn;
        public byte doo;
        public byte dp;
        public byte dq;
        public byte dr;
        public byte ds;
        public byte dt;
        public byte du;
        public byte dv;
        public byte dw;
        public byte dx;
        public byte dy;
        public byte dz;
        public byte ea;
        public byte eb;
        public byte ec;
        public byte ed;
        public byte ee;
        public byte ef;
        public byte eg;
        public byte eh;
        public byte ei;
        public byte ej;
        public byte ek;
        public byte el;
        public byte em;
        public byte en;
        public byte eo;
        public byte ep;
        public byte eq;
        public byte er;
        public byte es;
        public byte et;
        public byte eu;
        public byte ev;
        public byte ew;
        public byte ex;
        public byte ey;
        public byte ez;
        public byte fa;
        public byte fb;
        public byte fc;
        public byte fd;
        public byte fe;
        public byte ff;
        public byte fg;
        public byte fh;
        public byte fi;
        public byte fj;
        public byte fk;
        public byte fl;
        public byte fm;
        public byte fn;
        public byte fo;
        public byte fp;
        public byte fq;
        public byte fr;
        public byte fs;
        public byte ft;
        public byte fu;
        public byte fv;
        public byte fw;
        public byte fx;
        public byte fy;
        public byte fz;
        public byte ga;
        public byte gb;
        public byte gc;
        public byte gd;
        public byte ge;
        public byte gf;
        public byte gg;
        public byte gh;
        public byte gi;
        public byte gj;
        public byte gk;
        public byte gl;
    }

    Entity[] entities;

    Stopwatch stopwatch = new Stopwatch();

    void Start() {
        entities = new Entity[entityCount];
        System.Random random = new System.Random(12345);
        // Randomize the FSM states.
        for (int i = 0; i < entityCount; i++)
            entities[i].a = State.A;
    }

    void Update() {
        stopwatch.Restart();
        for (int i = 0; i < entities.Length; i++) {
            ref Entity e = ref entities[i];
            switch (e.a) {
                case State.A:
                    e.b++;
                    e.co++;
                    e.gl++;
                    break;
                case State.B:
                    e.d++;
                    e.bq++;
                    e.ee++;
                    break;
                case State.C:
                    e.g++;
                    e.bs++;
                    e.eg++;
                    break;
                case State.D:
                    e.j++;
                    e.bu++;
                    e.ej++;
                    break;
                case State.E:
                    e.m++;
                    e.bw++;
                    e.el++;
                    break;
                case State.F:
                    e.p++;
                    e.by++;
                    e.en++;
                    break;
                case State.G:
                    e.s++;
                    e.ca++;
                    e.ep++;
                    break;
                case State.H:
                    e.v++;
                    e.cc++;
                    e.er++;
                    break;
                case State.I:
                    e.y++;
                    e.ce++;
                    e.et++;
                    break;
                case State.J:
                    e.ab++;
                    e.cg++;
                    e.ev++;
                    break;
                case State.K:
                    e.ad++;
                    e.ci++;
                    e.ex++;
                    break;
                case State.L:
                    e.ag++;
                    e.ck++;
                    e.ez++;
                    break;
                case State.M:
                    e.aj++;
                    e.cm++;
                    e.fb++;
                    break;
                case State.N:
                    e.al++;
                    e.co++;
                    e.fd++;
                    break;
                case State.O:
                    e.ao++;
                    e.cq++;
                    e.ff++;
                    break;
                case State.P:
                    e.ar++;
                    e.cs++;
                    e.fh++;
                    break;
                case State.Q:
                    e.at++;
                    e.cu++;
                    e.fj++;
                    break;
                case State.R:
                    e.aw++;
                    e.cw++;
                    e.fl++;
                    break;
                case State.S:
                    e.az++;
                    e.cy++;
                    e.fn++;
                    break;
                case State.T:
                    e.bb++;
                    e.da++;
                    e.fp++;
                    break;
                case State.U:
                    e.be++;
                    e.dc++;
                    e.fr++;
                    break;
                case State.V:
                    e.bg++;
                    e.de++;
                    e.ft++;
                    break;
                case State.W:
                    e.bj++;
                    e.dg++;
                    e.fv++;
                    break;
                case State.X:
                    e.bl++;
                    e.di++;
                    e.fx++;
                    break;
                case State.Y:
                    e.bn++;
                    e.dk++;
                    e.fz++;
                    break;
                case State.Z:
                    e.bq++;
                    e.dm++;
                    e.gl++;
                    break;
            }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(
            $"AoS Update: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
    }
}
