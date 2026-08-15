using System;
using UnityEngine;

/// <summary>
/// Animation event player. Since during Animator's transitions both the events of the current and the
/// next animation state are triggered (leading to erroneus behavior), we instead tie the animation
/// events to the action states.
/// </summary>
public class AnimEventPlr{
    /// <summary>
    /// Max animation loops before animation is forced to restart to avoid timing precision problems.
    /// </summary>
    const int loopRebaseThreshold = 10000;
    /// <summary>
    /// Safety cap for animation loops per tick.
    /// </summary>
    const int maxLoopStepsPerTick = 500;

    readonly Animator anim;
    /// <summary>
    /// This Action will be invoked for all animation events.
    /// NOTE: string parameter details what particular animation event this represents.
    /// </summary>
    Action<string> onEvent;
    
    int shortNameHash;
    int animLayer;
    /// <summary>
    /// NOTE: Must be sorted ascending by normalized time, otherwise they are not necessarily
    /// called in the right order if multiple event trigger during one tick!
    /// </summary>
    ActAnimEvent[] sortedAnimEvents;
    /// <summary>
    /// Does the animation loop?
    /// </summary>
    bool looping;
    /// <summary>
    /// Last frame's normalized time from the animator.
    /// NOTE: This will go over 1.
    /// </summary>
    float prevTotalNrmT;
    /// <summary>
    /// Normalized position within current loop (0-1)
    /// </summary>
    float cursor;
    /// <summary>
    /// Authoritative loop counter. Doesn't reset even when animation state is rebased.
    /// </summary>
    int loopCount;
    /// <summary>
    /// Counter used to start a looping animation from the beginning after
    /// <see cref="loopRebaseThreshold"/> is reached to avoid animation time precision problems.
    /// </summary>
    int loopsSinceRebase;
    /// <summary>
    /// Has the animation finished (for non-looping only)?
    /// </summary>
    bool finished;
    bool firstTick;

    public long LoopCount => loopCount;

    /// <summary>
    /// Call this after action state machine has started a crossfade to a new state to initialize
    /// animation events for the next animation.<br/>
    /// NOTE: THE ANIMATION EVENTS NEED TO BE SORTED ASCENDING BY NORMALIZED TIME!!!
    /// </summary>
    public AnimEventPlr(
        Animator anim,
        int shortNameHash,
        int animLayer,
        ActAnimEvent[] sortedAnimEvents,
        bool looping,
        Action<string> onEvent,
        float startOffset = 0
    ) {
        this.anim = anim;
        this.shortNameHash = shortNameHash;
        this.animLayer = animLayer;
        this.sortedAnimEvents = sortedAnimEvents;
        this.looping = looping;
        this.onEvent = onEvent;
        cursor = startOffset;
        loopCount = 0;
        loopsSinceRebase = 0;
        finished = false;
        firstTick = true;
    }



    /// <summary>
    /// NOTE: Tick this in LateUpdate to make sure that the queued animator changes during this
    /// frame Update have already been applied!
    /// </summary>
    public void Tick() {
        if (finished) {
            firstTick = false;
            return;
        }
        if (!VisUtils.TryGetNewestStateInfo(anim, animLayer, shortNameHash, out AnimatorStateInfo info)) {
            Dbg.inst.LogWrn($"Short name hash ({shortNameHash}) did not match current animation "
                + $"({info.shortNameHash}).\nPerhaps action state was "
                + "changed but animator transition hasn't have the time to start yet?");
            firstTick = false;
            return;
        }
        float curTotalNrmT = info.normalizedTime;
        if (!looping) {
            FireEventsInNrmRange(cursor, Mathf.Min(curTotalNrmT, 1), firstTick);
            if (curTotalNrmT >= 1)
                finished = true;
            cursor = Mathf.Min(curTotalNrmT, 1);
            firstTick = false;
            return;
        }
        float dTotalNrmT = curTotalNrmT - prevTotalNrmT;
        prevTotalNrmT = curTotalNrmT;
        int stepsTaken = 0;
        while (dTotalNrmT > 0) {
            float toLoopEnd = 1 - cursor;
            if (dTotalNrmT < toLoopEnd) {
                FireEventsInNrmRange(cursor, cursor + dTotalNrmT, firstTick);
                cursor += dTotalNrmT;
                dTotalNrmT = 0;
            }
            else {
                FireEventsInNrmRange(cursor, 1, firstTick);
                dTotalNrmT -= toLoopEnd;
                cursor = 0;
                loopCount++;
                loopsSinceRebase++;
                // If animation looped too many times this tick, 
                if (++stepsTaken >= maxLoopStepsPerTick) {
                    Dbg.inst.LogErr($"Animation looped {stepsTaken} times during one tick! " 
                        + $"Max allowed loops: {maxLoopStepsPerTick}");
                    firstTick = false;
                    return;
                }
                // If we have looped too many times and the cursor is at the beginning of the loop,
                // start animation from the beginning.
                if (loopsSinceRebase >= loopRebaseThreshold && cursor == 0) {
                    anim.Play(shortNameHash, animLayer, 0);
                    prevTotalNrmT = 0;
                    loopsSinceRebase = 0;
                }
            }
        }
        firstTick = false;
    }

    /// <summary>
    /// Invokes events.
    /// NOTE: from and to need to be normalized!
    /// </summary>
    void FireEventsInNrmRange(float from, float to, bool includeFrom) {
        // Find first event in the range. NOTE that it doesn't include "from", but does include
        // "to". This way events do not fire twice. That also means we need a separate check for
        // first tick to fire any possible events at 0 normalized time.
        int startIndex = includeFrom
            ? LowerBoundInclusive(sortedAnimEvents, from)
            : LowerBound(sortedAnimEvents, from);
        for (int i = startIndex; i < sortedAnimEvents.Length; i++) {
            float t = sortedAnimEvents[i].nrmT;
            if (t > to) break;
            onEvent?.Invoke(sortedAnimEvents[i].id);
        }
    }

    /// <summary>
    /// Finds the index of the first event that has its normalized time above the threshold.
    /// </summary>
    int LowerBound(ActAnimEvent[] events, float nrmTThreshold) {
        int lo = 0;
        int hi = events.Length;
        while (lo < hi) {
            int mid = (lo + hi) / 2;
            if (events[mid].nrmT <= nrmTThreshold) lo = mid + 1;
            else hi = mid;
        }
        return lo;
    }

    /// <summary>
    /// Finds the index of the first event that has its normalized time above or exactly at the threshold.
    /// </summary>
    int LowerBoundInclusive(ActAnimEvent[] events, float nrmTThreshold) {
        int lo = 0;
        int hi = events.Length;
        while (lo < hi) {
            int mid = (lo + hi) / 2;
            if (events[mid].nrmT < nrmTThreshold)
                lo = mid + 1;
            else
                hi = mid;
        }
        return lo;
    }
}
