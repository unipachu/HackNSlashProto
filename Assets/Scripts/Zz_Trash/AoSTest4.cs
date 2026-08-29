using System.Diagnostics;
using UnityEngine;

public class AoSTest4 : MonoBehaviour {
    const int entityCount = 50_000;

    enum State : byte {
        A, B, C, D, E, F, G, H, I, J, K, L, M,
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }

    struct Entity {
        public State a;
        public int b;
        public int c;
        public int d;
        public int e;
        public int f;
        public int g;
        public int h;
        public int i;
        public int j;
        public int k;
        public int l;
        public int m;
        public int n;
        public int o;
        public int p;
        public int q;
        public int r;
        public int s;
        public int t;
        public int u;
        public int v;
        public int w;
        public int x;
        public int y;
        public int z;
        public int aa;
        public int ab;
        public int ac;
        public int ad;
        public int ae;
        public int af;
        public int ag;
        public int ah;
        public int ai;
        public int aj;
        public int ak;
        public int al;
        public int am;
        public int an;
        public int ao;
        public int ap;
        public int aq;
        public int ar;
        public int @as;
        public int at;
        public int au;
        public int av;
        public int aw;
        public int ax;
        public int ay;
        public int az;
        public int ba;
        public int bb;
        public int bc;
        public int bd;
        public int be;
        public int bf;
        public int bg;
        public int bh;
        public int bi;
        public int bj;
        public int bk;
        public int bl;
        public int bm;
        public int bn;
        public int bo;
        public int bp;
        public int bq;
        public int br;
        public int bs;
        public int bt;
        public int bu;
        public int bv;
        public int bw;
        public int bx;
        public int by;
        public int bz;
        public int ca;
        public int cb;
        public int cc;
        public int cd;
        public int ce;
        public int cf;
        public int cg;
        public int ch;
        public int ci;
        public int cj;
        public int ck;
        public int cl;
        public int cm;
        public int cn;
        public int co;
        public int cp;
        public int cq;
        public int cr;
        public int cs;
        public int ct;
        public int cu;
        public int cv;
        public int cw;
        public int cx;
        public int cy;
        public int cz;
        public int da;
        public int db;
        public int dc;
        public int dd;
        public int de;
        public int df;
        public int dg;
        public int dh;
        public int di;
        public int dj;
        public int dk;
        public int dl;
        public int dm;
        public int dn;
        public int doo;
        public int dp;
        public int dq;
        public int dr;
        public int ds;
        public int dt;
        public int du;
        public int dv;
        public int dw;
        public int dx;
        public int dy;
        public int dz;
        public int ea;
        public int eb;
        public int ec;
        public int ed;
        public int ee;
        public int ef;
        public int eg;
        public int eh;
        public int ei;
        public int ej;
        public int ek;
        public int el;
        public int em;
        public int en;
        public int eo;
        public int ep;
        public int eq;
        public int er;
        public int es;
        public int et;
        public int eu;
        public int ev;
        public int ew;
        public int ex;
        public int ey;
        public int ez;
        public int fa;
        public int fb;
        public int fc;
        public int fd;
        public int fe;
        public int ff;
        public int fg;
        public int fh;
        public int fi;
        public int fj;
        public int fk;
        public int fl;
        public int fm;
        public int fn;
        public int fo;
        public int fp;
        public int fq;
        public int fr;
        public int fs;
        public int ft;
        public int fu;
        public int fv;
        public int fw;
        public int fx;
        public int fy;
        public int fz;
        public int ga;
        public int gb;
        public int gc;
        public int gd;
        public int ge;
        public int gf;
        public int gg;
        public int gh;
        public int gi;
        public int gj;
        public int gk;
        public int gl;
    }

    Entity[] entities;

    Stopwatch stopwatch = new Stopwatch();

    void Start() {
        entities = new Entity[entityCount];
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