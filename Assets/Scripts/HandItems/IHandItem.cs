using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHandItem {
    /// <summary>
    /// NOTE: Not all hand items need to be able to "hit something" but since most of them do, we save some
    /// casting by doing this.
    /// </summary>
    public event Action<HashSet<HitResult>> hitSomething;

    Transform Trf { get; }
}
