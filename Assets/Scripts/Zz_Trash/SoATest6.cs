using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// Use burst compiled job. Basically SoATest4 but with burst jobs.
/// </summary>
public class SoATest6 : MonoBehaviour {
    const int entityCount = 50_000;

    enum State : byte {
        A, B, C, D, E, F, G, H, I, J, K, L, M,
        N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }

    NativeArray<byte> a;
    NativeArray<int> b;
    NativeArray<int> c;
    NativeArray<int> d;
    NativeArray<int> e;
    NativeArray<int> f;
    NativeArray<int> g;
    NativeArray<int> h;
    NativeArray<int> i;
    NativeArray<int> j;
    NativeArray<int> k;
    NativeArray<int> l;
    NativeArray<int> m;
    NativeArray<int> n;
    NativeArray<int> o;
    NativeArray<int> p;
    NativeArray<int> q;
    NativeArray<int> r;
    NativeArray<int> s;
    NativeArray<int> t;
    NativeArray<int> u;
    NativeArray<int> v;
    NativeArray<int> w;
    NativeArray<int> x;
    NativeArray<int> y;
    NativeArray<int> z;
    NativeArray<int> aa;
    NativeArray<int> ab;
    NativeArray<int> ac;
    NativeArray<int> ad;
    NativeArray<int> ae;
    NativeArray<int> af;
    NativeArray<int> ag;
    NativeArray<int> ah;
    NativeArray<int> ai;
    NativeArray<int> aj;
    NativeArray<int> ak;
    NativeArray<int> al;
    NativeArray<int> am;
    NativeArray<int> an;
    NativeArray<int> ao;
    NativeArray<int> ap;
    NativeArray<int> aq;
    NativeArray<int> ar;
    NativeArray<int> @as;
    NativeArray<int> at;
    NativeArray<int> au;
    NativeArray<int> av;
    NativeArray<int> aw;
    NativeArray<int> ax;
    NativeArray<int> ay;
    NativeArray<int> az;
    NativeArray<int> ba;
    NativeArray<int> bb;
    NativeArray<int> bc;
    NativeArray<int> bd;
    NativeArray<int> be;
    NativeArray<int> bf;
    NativeArray<int> bg;
    NativeArray<int> bh;
    NativeArray<int> bi;
    NativeArray<int> bj;
    NativeArray<int> bk;
    NativeArray<int> bl;
    NativeArray<int> bm;
    NativeArray<int> bn;
    NativeArray<int> bo;
    NativeArray<int> bp;
    NativeArray<int> bq;
    NativeArray<int> br;
    NativeArray<int> bs;
    NativeArray<int> bt;
    NativeArray<int> bu;
    NativeArray<int> bv;
    NativeArray<int> bw;
    NativeArray<int> bx;
    NativeArray<int> by;
    NativeArray<int> bz;
    NativeArray<int> ca;
    NativeArray<int> cb;
    NativeArray<int> cc;
    NativeArray<int> cd;
    NativeArray<int> ce;
    NativeArray<int> cf;
    NativeArray<int> cg;
    NativeArray<int> ch;
    NativeArray<int> ci;
    NativeArray<int> cj;
    NativeArray<int> ck;
    NativeArray<int> cl;
    NativeArray<int> cm;
    NativeArray<int> cn;
    NativeArray<int> co;
    NativeArray<int> cp;
    NativeArray<int> cq;
    NativeArray<int> cr;
    NativeArray<int> cs;
    NativeArray<int> ct;
    NativeArray<int> cu;
    NativeArray<int> cv;
    NativeArray<int> cw;
    NativeArray<int> cx;
    NativeArray<int> cy;
    NativeArray<int> cz;
    NativeArray<int> da;
    NativeArray<int> db;
    NativeArray<int> dc;
    NativeArray<int> dd;
    NativeArray<int> de;
    NativeArray<int> df;
    NativeArray<int> dg;
    NativeArray<int> dh;
    NativeArray<int> di;
    NativeArray<int> dj;
    NativeArray<int> dk;
    NativeArray<int> dl;
    NativeArray<int> dm;
    NativeArray<int> dn;
    NativeArray<int> doo;
    NativeArray<int> dp;
    NativeArray<int> dq;
    NativeArray<int> dr;
    NativeArray<int> ds;
    NativeArray<int> dt;
    NativeArray<int> du;
    NativeArray<int> dv;
    NativeArray<int> dw;
    NativeArray<int> dx;
    NativeArray<int> dy;
    NativeArray<int> dz;
    NativeArray<int> ea;
    NativeArray<int> eb;
    NativeArray<int> ec;
    NativeArray<int> ed;
    NativeArray<int> ee;
    NativeArray<int> ef;
    NativeArray<int> eg;
    NativeArray<int> eh;
    NativeArray<int> ei;
    NativeArray<int> ej;
    NativeArray<int> ek;
    NativeArray<int> el;
    NativeArray<int> em;
    NativeArray<int> en;
    NativeArray<int> eo;
    NativeArray<int> ep;
    NativeArray<int> eq;
    NativeArray<int> er;
    NativeArray<int> es;
    NativeArray<int> et;
    NativeArray<int> eu;
    NativeArray<int> ev;
    NativeArray<int> ew;
    NativeArray<int> ex;
    NativeArray<int> ey;
    NativeArray<int> ez;
    NativeArray<int> fa;
    NativeArray<int> fb;
    NativeArray<int> fc;
    NativeArray<int> fd;
    NativeArray<int> fe;
    NativeArray<int> ff;
    NativeArray<int> fg;
    NativeArray<int> fh;
    NativeArray<int> fi;
    NativeArray<int> fj;
    NativeArray<int> fk;
    NativeArray<int> fl;
    NativeArray<int> fm;
    NativeArray<int> fn;
    NativeArray<int> fo;
    NativeArray<int> fp;
    NativeArray<int> fq;
    NativeArray<int> fr;
    NativeArray<int> fs;
    NativeArray<int> ft;
    NativeArray<int> fu;
    NativeArray<int> fv;
    NativeArray<int> fw;
    NativeArray<int> fx;
    NativeArray<int> fy;
    NativeArray<int> fz;
    NativeArray<int> ga;
    NativeArray<int> gb;
    NativeArray<int> gc;
    NativeArray<int> gd;
    NativeArray<int> ge;
    NativeArray<int> gf;
    NativeArray<int> gg;
    NativeArray<int> gh;
    NativeArray<int> gi;
    NativeArray<int> gj;
    NativeArray<int> gk;
    NativeArray<int> gl;

    Stopwatch stopwatch = new Stopwatch();

    void Start() {
        a = new NativeArray<byte>(entityCount, Allocator.Persistent);
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
        ad = NewArray();
        ae = NewArray();
        af = NewArray();
        ag = NewArray();
        ah = NewArray();
        ai = NewArray();
        aj = NewArray();
        ak = NewArray();
        al = NewArray();
        am = NewArray();
        an = NewArray();
        ao = NewArray();
        ap = NewArray();
        aq = NewArray();
        ar = NewArray();
        @as = NewArray();
        at = NewArray();
        au = NewArray();
        av = NewArray();
        aw = NewArray();
        ax = NewArray();
        ay = NewArray();
        az = NewArray();
        ba = NewArray();
        bb = NewArray();
        bc = NewArray();
        bd = NewArray();
        be = NewArray();
        bf = NewArray();
        bg = NewArray();
        bh = NewArray();
        bi = NewArray();
        bj = NewArray();
        bk = NewArray();
        bl = NewArray();
        bm = NewArray();
        bn = NewArray();
        bo = NewArray();
        bp = NewArray();
        bq = NewArray();
        br = NewArray();
        bs = NewArray();
        bt = NewArray();
        bu = NewArray();
        bv = NewArray();
        bw = NewArray();
        bx = NewArray();
        by = NewArray();
        bz = NewArray();
        ca = NewArray();
        cb = NewArray();
        cc = NewArray();
        cd = NewArray();
        ce = NewArray();
        cf = NewArray();
        cg = NewArray();
        ch = NewArray();
        ci = NewArray();
        cj = NewArray();
        ck = NewArray();
        cl = NewArray();
        cm = NewArray();
        cn = NewArray();
        co = NewArray();
        cp = NewArray();
        cq = NewArray();
        cr = NewArray();
        cs = NewArray();
        ct = NewArray();
        cu = NewArray();
        cv = NewArray();
        cw = NewArray();
        cx = NewArray();
        cy = NewArray();
        cz = NewArray();
        da = NewArray();
        db = NewArray();
        dc = NewArray();
        dd = NewArray();
        de = NewArray();
        df = NewArray();
        dg = NewArray();
        dh = NewArray();
        di = NewArray();
        dj = NewArray();
        dk = NewArray();
        dl = NewArray();
        dm = NewArray();
        dn = NewArray();
        doo = NewArray();
        dp = NewArray();
        dq = NewArray();
        dr = NewArray();
        ds = NewArray();
        dt = NewArray();
        du = NewArray();
        dv = NewArray();
        dw = NewArray();
        dx = NewArray();
        dy = NewArray();
        dz = NewArray();
        ea = NewArray();
        eb = NewArray();
        ec = NewArray();
        ed = NewArray();
        ee = NewArray();
        ef = NewArray();
        eg = NewArray();
        eh = NewArray();
        ei = NewArray();
        ej = NewArray();
        ek = NewArray();
        el = NewArray();
        em = NewArray();
        en = NewArray();
        eo = NewArray();
        ep = NewArray();
        eq = NewArray();
        er = NewArray();
        es = NewArray();
        et = NewArray();
        eu = NewArray();
        ev = NewArray();
        ew = NewArray();
        ex = NewArray();
        ey = NewArray();
        ez = NewArray();
        fa = NewArray();
        fb = NewArray();
        fc = NewArray();
        fd = NewArray();
        fe = NewArray();
        ff = NewArray();
        fg = NewArray();
        fh = NewArray();
        fi = NewArray();
        fj = NewArray();
        fk = NewArray();
        fl = NewArray();
        fm = NewArray();
        fn = NewArray();
        fo = NewArray();
        fp = NewArray();
        fq = NewArray();
        fr = NewArray();
        fs = NewArray();
        ft = NewArray();
        fu = NewArray();
        fv = NewArray();
        fw = NewArray();
        fx = NewArray();
        fy = NewArray();
        fz = NewArray();
        ga = NewArray();
        gb = NewArray();
        gc = NewArray();
        gd = NewArray();
        ge = NewArray();
        gf = NewArray();
        gg = NewArray();
        gh = NewArray();
        gi = NewArray();
        gj = NewArray();
        gk = NewArray();
        gl = NewArray();
        System.Random random = new System.Random(12345);
        for (int i = 0; i < entityCount; i++) {
            // NOTE: Change this to both AoS and SoA test scripts to used randomized state.
            //a[i] = (byte)random.Next(26);
            a[i] = (byte)State.A;

        }
    }

    NativeArray<int> NewArray() {
        return new NativeArray<int>(entityCount, Allocator.Persistent);
    }

    void Update() {
        stopwatch.Restart();
        SoAJob job = new SoAJob {
            a = a,
            b = b,
            c = c,
            d = d,
            e = e,
            f = f,
            g = g,
            h = h,
            i = i,
            j = j,
            k = k,
            l = l,
            m = m,
            n = n,
            o = o,
            p = p,
            q = q,
            r = r,
            s = s,
            t = t,
            u = u,
            v = v,
            w = w,
            x = x,
            y = y,
            z = z,
            aa = aa,
            ab = ab,
            ac = ac,
            ad = ad,
            ae = ae,
            af = af,
            ag = ag,
            ah = ah,
            ai = ai,
            aj = aj,
            ak = ak,
            al = al,
            am = am,
            an = an,
            ao = ao,
            ap = ap,
            aq = aq,
            ar = ar,
            @as = @as,
            at = at,
            au = au,
            av = av,
            aw = aw,
            ax = ax,
            ay = ay,
            az = az,
            ba = ba,
            bb = bb,
            bc = bc,
            bd = bd,
            be = be,
            bf = bf,
            bg = bg,
            bh = bh,
            bi = bi,
            bj = bj,
            bk = bk,
            bl = bl,
            bm = bm,
            bn = bn,
            bo = bo,
            bp = bp,
            bq = bq,
            br = br,
            bs = bs,
            bt = bt,
            bu = bu,
            bv = bv,
            bw = bw,
            bx = bx,
            by = by,
            bz = bz,
            ca = ca,
            cb = cb,
            cc = cc,
            cd = cd,
            ce = ce,
            cf = cf,
            cg = cg,
            ch = ch,
            ci = ci,
            cj = cj,
            ck = ck,
            cl = cl,
            cm = cm,
            cn = cn,
            co = co,
            cp = cp,
            cq = cq,
            cr = cr,
            cs = cs,
            ct = ct,
            cu = cu,
            cv = cv,
            cw = cw,
            cx = cx,
            cy = cy,
            cz = cz,
            da = da,
            db = db,
            dc = dc,
            dd = dd,
            de = de,
            df = df,
            dg = dg,
            dh = dh,
            di = di,
            dj = dj,
            dk = dk,
            dl = dl,
            dm = dm,
            dn = dn,
            doo = doo,
            dp = dp,
            dq = dq,
            dr = dr,
            ds = ds,
            dt = dt,
            du = du,
            dv = dv,
            dw = dw,
            dx = dx,
            dy = dy,
            dz = dz,
            ea = ea,
            eb = eb,
            ec = ec,
            ed = ed,
            ee = ee,
            ef = ef,
            eg = eg,
            eh = eh,
            ei = ei,
            ej = ej,
            ek = ek,
            el = el,
            em = em,
            en = en,
            eo = eo,
            ep = ep,
            eq = eq,
            er = er,
            es = es,
            et = et,
            eu = eu,
            ev = ev,
            ew = ew,
            ex = ex,
            ey = ey,
            ez = ez,
            fa = fa,
            fb = fb,
            fc = fc,
            fd = fd,
            fe = fe,
            ff = ff,
            fg = fg,
            fh = fh,
            fi = fi,
            fj = fj,
            fk = fk,
            fl = fl,
            fm = fm,
            fn = fn,
            fo = fo,
            fp = fp,
            fq = fq,
            fr = fr,
            fs = fs,
            ft = ft,
            fu = fu,
            fv = fv,
            fw = fw,
            fx = fx,
            fy = fy,
            fz = fz,
            ga = ga,
            gb = gb,
            gc = gc,
            gd = gd,
            ge = ge,
            gf = gf,
            gg = gg,
            gh = gh,
            gi = gi,
            gj = gj,
            gk = gk,
            gl = gl
        };
        JobHandle handle = job.ScheduleParallel(entityCount, 64, default);
        handle.Complete();
        stopwatch.Stop();
        UnityEngine.Debug.Log(
            $"Burst SoA Job Update: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
    }

    [BurstCompile]
    struct SoAJob : IJobFor {
        [ReadOnly] public NativeArray<byte> a;
        public NativeArray<int> b;
        public NativeArray<int> c;
        public NativeArray<int> d;
        public NativeArray<int> e;
        public NativeArray<int> f;
        public NativeArray<int> g;
        public NativeArray<int> h;
        public NativeArray<int> i;
        public NativeArray<int> j;
        public NativeArray<int> k;
        public NativeArray<int> l;
        public NativeArray<int> m;
        public NativeArray<int> n;
        public NativeArray<int> o;
        public NativeArray<int> p;
        public NativeArray<int> q;
        public NativeArray<int> r;
        public NativeArray<int> s;
        public NativeArray<int> t;
        public NativeArray<int> u;
        public NativeArray<int> v;
        public NativeArray<int> w;
        public NativeArray<int> x;
        public NativeArray<int> y;
        public NativeArray<int> z;
        public NativeArray<int> aa;
        public NativeArray<int> ab;
        public NativeArray<int> ac;
        public NativeArray<int> ad;
        public NativeArray<int> ae;
        public NativeArray<int> af;
        public NativeArray<int> ag;
        public NativeArray<int> ah;
        public NativeArray<int> ai;
        public NativeArray<int> aj;
        public NativeArray<int> ak;
        public NativeArray<int> al;
        public NativeArray<int> am;
        public NativeArray<int> an;
        public NativeArray<int> ao;
        public NativeArray<int> ap;
        public NativeArray<int> aq;
        public NativeArray<int> ar;
        public NativeArray<int> @as;
        public NativeArray<int> at;
        public NativeArray<int> au;
        public NativeArray<int> av;
        public NativeArray<int> aw;
        public NativeArray<int> ax;
        public NativeArray<int> ay;
        public NativeArray<int> az;
        public NativeArray<int> ba;
        public NativeArray<int> bb;
        public NativeArray<int> bc;
        public NativeArray<int> bd;
        public NativeArray<int> be;
        public NativeArray<int> bf;
        public NativeArray<int> bg;
        public NativeArray<int> bh;
        public NativeArray<int> bi;
        public NativeArray<int> bj;
        public NativeArray<int> bk;
        public NativeArray<int> bl;
        public NativeArray<int> bm;
        public NativeArray<int> bn;
        public NativeArray<int> bo;
        public NativeArray<int> bp;
        public NativeArray<int> bq;
        public NativeArray<int> br;
        public NativeArray<int> bs;
        public NativeArray<int> bt;
        public NativeArray<int> bu;
        public NativeArray<int> bv;
        public NativeArray<int> bw;
        public NativeArray<int> bx;
        public NativeArray<int> by;
        public NativeArray<int> bz;
        public NativeArray<int> ca;
        public NativeArray<int> cb;
        public NativeArray<int> cc;
        public NativeArray<int> cd;
        public NativeArray<int> ce;
        public NativeArray<int> cf;
        public NativeArray<int> cg;
        public NativeArray<int> ch;
        public NativeArray<int> ci;
        public NativeArray<int> cj;
        public NativeArray<int> ck;
        public NativeArray<int> cl;
        public NativeArray<int> cm;
        public NativeArray<int> cn;
        public NativeArray<int> co;
        public NativeArray<int> cp;
        public NativeArray<int> cq;
        public NativeArray<int> cr;
        public NativeArray<int> cs;
        public NativeArray<int> ct;
        public NativeArray<int> cu;
        public NativeArray<int> cv;
        public NativeArray<int> cw;
        public NativeArray<int> cx;
        public NativeArray<int> cy;
        public NativeArray<int> cz;
        public NativeArray<int> da;
        public NativeArray<int> db;
        public NativeArray<int> dc;
        public NativeArray<int> dd;
        public NativeArray<int> de;
        public NativeArray<int> df;
        public NativeArray<int> dg;
        public NativeArray<int> dh;
        public NativeArray<int> di;
        public NativeArray<int> dj;
        public NativeArray<int> dk;
        public NativeArray<int> dl;
        public NativeArray<int> dm;
        public NativeArray<int> dn;
        public NativeArray<int> doo;
        public NativeArray<int> dp;
        public NativeArray<int> dq;
        public NativeArray<int> dr;
        public NativeArray<int> ds;
        public NativeArray<int> dt;
        public NativeArray<int> du;
        public NativeArray<int> dv;
        public NativeArray<int> dw;
        public NativeArray<int> dx;
        public NativeArray<int> dy;
        public NativeArray<int> dz;
        public NativeArray<int> ea;
        public NativeArray<int> eb;
        public NativeArray<int> ec;
        public NativeArray<int> ed;
        public NativeArray<int> ee;
        public NativeArray<int> ef;
        public NativeArray<int> eg;
        public NativeArray<int> eh;
        public NativeArray<int> ei;
        public NativeArray<int> ej;
        public NativeArray<int> ek;
        public NativeArray<int> el;
        public NativeArray<int> em;
        public NativeArray<int> en;
        public NativeArray<int> eo;
        public NativeArray<int> ep;
        public NativeArray<int> eq;
        public NativeArray<int> er;
        public NativeArray<int> es;
        public NativeArray<int> et;
        public NativeArray<int> eu;
        public NativeArray<int> ev;
        public NativeArray<int> ew;
        public NativeArray<int> ex;
        public NativeArray<int> ey;
        public NativeArray<int> ez;
        public NativeArray<int> fa;
        public NativeArray<int> fb;
        public NativeArray<int> fc;
        public NativeArray<int> fd;
        public NativeArray<int> fe;
        public NativeArray<int> ff;
        public NativeArray<int> fg;
        public NativeArray<int> fh;
        public NativeArray<int> fi;
        public NativeArray<int> fj;
        public NativeArray<int> fk;
        public NativeArray<int> fl;
        public NativeArray<int> fm;
        public NativeArray<int> fn;
        public NativeArray<int> fo;
        public NativeArray<int> fp;
        public NativeArray<int> fq;
        public NativeArray<int> fr;
        public NativeArray<int> fs;
        public NativeArray<int> ft;
        public NativeArray<int> fu;
        public NativeArray<int> fv;
        public NativeArray<int> fw;
        public NativeArray<int> fx;
        public NativeArray<int> fy;
        public NativeArray<int> fz;
        public NativeArray<int> ga;
        public NativeArray<int> gb;
        public NativeArray<int> gc;
        public NativeArray<int> gd;
        public NativeArray<int> ge;
        public NativeArray<int> gf;
        public NativeArray<int> gg;
        public NativeArray<int> gh;
        public NativeArray<int> gi;
        public NativeArray<int> gj;
        public NativeArray<int> gk;
        public NativeArray<int> gl;

        public void Execute(int ii) {
            switch ((State)a[ii]) {
                case State.A:
                    b[ii]++;
                    co[ii]++;
                    gl[ii]++;
                    break;
                case State.B:
                    d[ii]++;
                    bq[ii]++;
                    ee[ii]++;
                    break;
                case State.C:
                    g[ii]++;
                    bs[ii]++;
                    eg[ii]++;
                    break;
                case State.D:
                    j[ii]++;
                    bu[ii]++;
                    ej[ii]++;
                    break;
                case State.E:
                    m[ii]++;
                    bw[ii]++;
                    el[ii]++;
                    break;
                case State.F:
                    p[ii]++;
                    by[ii]++;
                    en[ii]++;
                    break;
                case State.G:
                    s[ii]++;
                    ca[ii]++;
                    ep[ii]++;
                    break;
                case State.H:
                    v[ii]++;
                    cc[ii]++;
                    er[ii]++;
                    break;
                case State.I:
                    y[ii]++;
                    ce[ii]++;
                    et[ii]++;
                    break;
                case State.J:
                    ab[ii]++;
                    cg[ii]++;
                    ev[ii]++;
                    break;
                case State.K:
                    ad[ii]++;
                    ci[ii]++;
                    ex[ii]++;
                    break;
                case State.L:
                    ag[ii]++;
                    ck[ii]++;
                    ez[ii]++;
                    break;
                case State.M:
                    aj[ii]++;
                    cm[ii]++;
                    fb[ii]++;
                    break;
                case State.N:
                    al[ii]++;
                    co[ii]++;
                    fd[ii]++;
                    break;
                case State.O:
                    ao[ii]++;
                    cq[ii]++;
                    ff[ii]++;
                    break;
                case State.P:
                    ar[ii]++;
                    cs[ii]++;
                    fh[ii]++;
                    break;
                case State.Q:
                    at[ii]++;
                    cu[ii]++;
                    fj[ii]++;
                    break;
                case State.R:
                    aw[ii]++;
                    cw[ii]++;
                    fl[ii]++;
                    break;
                case State.S:
                    az[ii]++;
                    cy[ii]++;
                    fn[ii]++;
                    break;
                case State.T:
                    bb[ii]++;
                    da[ii]++;
                    fp[ii]++;
                    break;
                case State.U:
                    be[ii]++;
                    dc[ii]++;
                    fr[ii]++;
                    break;
                case State.V:
                    bg[ii]++;
                    de[ii]++;
                    ft[ii]++;
                    break;
                case State.W:
                    bj[ii]++;
                    dg[ii]++;
                    fv[ii]++;
                    break;
                case State.X:
                    bl[ii]++;
                    di[ii]++;
                    fx[ii]++;
                    break;
                case State.Y:
                    bn[ii]++;
                    dk[ii]++;
                    fz[ii]++;
                    break;
                case State.Z:
                    bq[ii]++;
                    dm[ii]++;
                    gl[ii]++;
                    break;
            }
        }
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
        Dispose(ref ad);
        Dispose(ref ae);
        Dispose(ref af);
        Dispose(ref ag);
        Dispose(ref ah);
        Dispose(ref ai);
        Dispose(ref aj);
        Dispose(ref ak);
        Dispose(ref al);
        Dispose(ref am);
        Dispose(ref an);
        Dispose(ref ao);
        Dispose(ref ap);
        Dispose(ref aq);
        Dispose(ref ar);
        Dispose(ref @as);
        Dispose(ref at);
        Dispose(ref au);
        Dispose(ref av);
        Dispose(ref aw);
        Dispose(ref ax);
        Dispose(ref ay);
        Dispose(ref az);
        Dispose(ref ba);
        Dispose(ref bb);
        Dispose(ref bc);
        Dispose(ref bd);
        Dispose(ref be);
        Dispose(ref bf);
        Dispose(ref bg);
        Dispose(ref bh);
        Dispose(ref bi);
        Dispose(ref bj);
        Dispose(ref bk);
        Dispose(ref bl);
        Dispose(ref bm);
        Dispose(ref bn);
        Dispose(ref bo);
        Dispose(ref bp);
        Dispose(ref bq);
        Dispose(ref br);
        Dispose(ref bs);
        Dispose(ref bt);
        Dispose(ref bu);
        Dispose(ref bv);
        Dispose(ref bw);
        Dispose(ref bx);
        Dispose(ref by);
        Dispose(ref bz);
        Dispose(ref ca);
        Dispose(ref cb);
        Dispose(ref cc);
        Dispose(ref cd);
        Dispose(ref ce);
        Dispose(ref cf);
        Dispose(ref cg);
        Dispose(ref ch);
        Dispose(ref ci);
        Dispose(ref cj);
        Dispose(ref ck);
        Dispose(ref cl);
        Dispose(ref cm);
        Dispose(ref cn);
        Dispose(ref co);
        Dispose(ref cp);
        Dispose(ref cq);
        Dispose(ref cr);
        Dispose(ref cs);
        Dispose(ref ct);
        Dispose(ref cu);
        Dispose(ref cv);
        Dispose(ref cw);
        Dispose(ref cx);
        Dispose(ref cy);
        Dispose(ref cz);
        Dispose(ref da);
        Dispose(ref db);
        Dispose(ref dc);
        Dispose(ref dd);
        Dispose(ref de);
        Dispose(ref df);
        Dispose(ref dg);
        Dispose(ref dh);
        Dispose(ref di);
        Dispose(ref dj);
        Dispose(ref dk);
        Dispose(ref dl);
        Dispose(ref dm);
        Dispose(ref dn);
        Dispose(ref doo);
        Dispose(ref dp);
        Dispose(ref dq);
        Dispose(ref dr);
        Dispose(ref ds);
        Dispose(ref dt);
        Dispose(ref du);
        Dispose(ref dv);
        Dispose(ref dw);
        Dispose(ref dx);
        Dispose(ref dy);
        Dispose(ref dz);
        Dispose(ref ea);
        Dispose(ref eb);
        Dispose(ref ec);
        Dispose(ref ed);
        Dispose(ref ee);
        Dispose(ref ef);
        Dispose(ref eg);
        Dispose(ref eh);
        Dispose(ref ei);
        Dispose(ref ej);
        Dispose(ref ek);
        Dispose(ref el);
        Dispose(ref em);
        Dispose(ref en);
        Dispose(ref eo);
        Dispose(ref ep);
        Dispose(ref eq);
        Dispose(ref er);
        Dispose(ref es);
        Dispose(ref et);
        Dispose(ref eu);
        Dispose(ref ev);
        Dispose(ref ew);
        Dispose(ref ex);
        Dispose(ref ey);
        Dispose(ref ez);
        Dispose(ref fa);
        Dispose(ref fb);
        Dispose(ref fc);
        Dispose(ref fd);
        Dispose(ref fe);
        Dispose(ref ff);
        Dispose(ref fg);
        Dispose(ref fh);
        Dispose(ref fi);
        Dispose(ref fj);
        Dispose(ref fk);
        Dispose(ref fl);
        Dispose(ref fm);
        Dispose(ref fn);
        Dispose(ref fo);
        Dispose(ref fp);
        Dispose(ref fq);
        Dispose(ref fr);
        Dispose(ref fs);
        Dispose(ref ft);
        Dispose(ref fu);
        Dispose(ref fv);
        Dispose(ref fw);
        Dispose(ref fx);
        Dispose(ref fy);
        Dispose(ref fz);
        Dispose(ref ga);
        Dispose(ref gb);
        Dispose(ref gc);
        Dispose(ref gd);
        Dispose(ref ge);
        Dispose(ref gf);
        Dispose(ref gg);
        Dispose(ref gh);
        Dispose(ref gi);
        Dispose(ref gj);
        Dispose(ref gk);
        Dispose(ref gl);
    }

    static void Dispose(ref NativeArray<byte> array) {
        if (array.IsCreated)
            array.Dispose();
    }

    static void Dispose(ref NativeArray<int> array) {
        if (array.IsCreated)
            array.Dispose();
    }
}
