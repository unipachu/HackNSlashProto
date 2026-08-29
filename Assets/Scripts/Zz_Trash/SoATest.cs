using System.Diagnostics;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// For AoS, use struct that fits into one cache line.
/// </summary>
public class SoATest : MonoBehaviour{
    const int entityCount = 50_000;

    enum State : byte {
        A, B, C, D, E, F, G, H, I, J, K, L, M,
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }

    NativeArray<byte> a;
    NativeArray<byte> b;
    NativeArray<byte> c;
    NativeArray<byte> d;
    NativeArray<byte> e;
    NativeArray<byte> f;
    NativeArray<byte> g;
    NativeArray<byte> h;
    NativeArray<byte> i;
    NativeArray<byte> j;
    NativeArray<byte> k;
    NativeArray<byte> l;
    NativeArray<byte> m;
    NativeArray<byte> n;
    NativeArray<byte> o;
    NativeArray<byte> p;
    NativeArray<byte> q;
    NativeArray<byte> r;
    NativeArray<byte> s;
    NativeArray<byte> t;
    NativeArray<byte> u;
    NativeArray<byte> v;
    NativeArray<byte> w;
    NativeArray<byte> x;
    NativeArray<byte> y;
    NativeArray<byte> z;
    NativeArray<byte> aa;
    NativeArray<byte> ab;
    NativeArray<byte> ac;

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

    NativeArray<byte> NewArray() {
        return new NativeArray<byte>(
            entityCount,
            Allocator.Persistent);
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

    void OnDestroy() {
        Dispose(ref a);
        Dispose(ref b);
        Dispose(ref c);
        Dispose(ref d);
        Dispose(ref e);
        Dispose(ref f);
        Dispose(ref g);
        Dispose(ref h);
        Dispose(ref i);
        Dispose(ref j);
        Dispose(ref k);
        Dispose(ref l);
        Dispose(ref m);
        Dispose(ref n);
        Dispose(ref o);
        Dispose(ref p);
        Dispose(ref q);
        Dispose(ref r);
        Dispose(ref s);
        Dispose(ref t);
        Dispose(ref u);
        Dispose(ref v);
        Dispose(ref w);
        Dispose(ref x);
        Dispose(ref y);
        Dispose(ref z);
        Dispose(ref aa);
        Dispose(ref ab);
        Dispose(ref ac);
    }

    static void Dispose(ref NativeArray<byte> array) {
        if (array.IsCreated)
            array.Dispose();
    }
}
