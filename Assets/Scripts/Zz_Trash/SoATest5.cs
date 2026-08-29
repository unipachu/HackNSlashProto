using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Use managed arrays instead of native arrays and fit AoS struct to one cache line and randomize states.
/// </summary>
public class SoATest5 : MonoBehaviour {
    const int entityCount = 50_000;

    enum State : byte {
        A, B, C, D, E, F, G, H, I, J, K, L, M,
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }

    byte[] a;
    byte[] b;
    byte[] c;
    byte[] d;
    byte[] e;
    byte[] f;
    byte[] g;
    byte[] h;
    byte[] i;
    byte[] j;
    byte[] k;
    byte[] l;
    byte[] m;
    byte[] n;
    byte[] o;
    byte[] p;
    byte[] q;
    byte[] r;
    byte[] s;
    byte[] t;
    byte[] u;
    byte[] v;
    byte[] w;
    byte[] x;
    byte[] y;
    byte[] z;
    byte[] aa;
    byte[] ab;
    byte[] ac;

    Stopwatch stopwatch = new Stopwatch();

    void Start() {
        a = NewArray();
        b = NewArray();
        c = NewArray();
        d = NewArray();
        e = NewArray();
        f = NewArray();
        g = NewArray();
        h = NewArray();
        i = NewArray();
        j = NewArray();
        k = NewArray();
        l = NewArray();
        m = NewArray();
        n = NewArray();
        o = NewArray();
        p = NewArray();
        q = NewArray();
        r = NewArray();
        s = NewArray();
        t = NewArray();
        u = NewArray();
        v = NewArray();
        w = NewArray();
        x = NewArray();
        y = NewArray();
        z = NewArray();
        aa = NewArray();
        ab = NewArray();
        ac = NewArray();

        System.Random random = new System.Random(12345);
        // Randomize the FSM states.
        for (int i = 0; i < entityCount; i++)
            a[i] = (byte)random.Next(26);
    }

    byte[] NewArray() {
        return new byte[entityCount];
    }

    void Update() {
        stopwatch.Restart();
        for (int ii = 0; ii < entityCount; ii++) {
            switch ((State)a[ii]) {
                case State.A:
                    b[ii]++;
                    c[ii]++;
                    d[ii]++;
                    break;
                case State.B:
                    c[ii]++;
                    d[ii]++;
                    e[ii]++;
                    break;
                case State.C:
                    d[ii]++;
                    e[ii]++;
                    f[ii]++;
                    break;
                case State.D:
                    e[ii]++;
                    f[ii]++;
                    g[ii]++;
                    break;
                case State.E:
                    f[ii]++;
                    g[ii]++;
                    h[ii]++;
                    break;
                case State.F:
                    g[ii]++;
                    h[ii]++;
                    i[ii]++;
                    break;
                case State.G:
                    h[ii]++;
                    i[ii]++;
                    j[ii]++;
                    break;
                case State.H:
                    i[ii]++;
                    j[ii]++;
                    k[ii]++;
                    break;
                case State.I:
                    j[ii]++;
                    k[ii]++;
                    l[ii]++;
                    break;
                case State.J:
                    k[ii]++;
                    l[ii]++;
                    m[ii]++;
                    break;
                case State.K:
                    l[ii]++;
                    m[ii]++;
                    n[ii]++;
                    break;
                case State.L:
                    m[ii]++;
                    n[ii]++;
                    o[ii]++;
                    break;
                case State.M:
                    n[ii]++;
                    o[ii]++;
                    p[ii]++;
                    break;
                case State.N:
                    o[ii]++;
                    p[ii]++;
                    q[ii]++;
                    break;
                case State.O:
                    p[ii]++;
                    q[ii]++;
                    r[ii]++;
                    break;
                case State.P:
                    q[ii]++;
                    r[ii]++;
                    s[ii]++;
                    break;
                case State.Q:
                    r[ii]++;
                    s[ii]++;
                    t[ii]++;
                    break;
                case State.R:
                    s[ii]++;
                    t[ii]++;
                    u[ii]++;
                    break;
                case State.S:
                    t[ii]++;
                    u[ii]++;
                    v[ii]++;
                    break;
                case State.T:
                    u[ii]++;
                    v[ii]++;
                    w[ii]++;
                    break;
                case State.U:
                    v[ii]++;
                    w[ii]++;
                    x[ii]++;
                    break;
                case State.V:
                    w[ii]++;
                    x[ii]++;
                    y[ii]++;
                    break;
                case State.W:
                    x[ii]++;
                    y[ii]++;
                    z[ii]++;
                    break;
                case State.X:
                    y[ii]++;
                    z[ii]++;
                    aa[ii]++;
                    break;
                case State.Y:
                    z[ii]++;
                    aa[ii]++;
                    ab[ii]++;
                    break;
                case State.Z:
                    aa[ii]++;
                    ab[ii]++;
                    ac[ii]++;
                    break;
            }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log($"SoA Update: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
    }
}