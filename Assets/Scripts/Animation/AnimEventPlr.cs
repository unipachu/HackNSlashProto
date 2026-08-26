using System;
using UnityEngine;

/// <summary>
/// Custom animation event controller. Since during Animator's transitions both the events of the current and the
/// next animation state are triggered (which causes erroneus behavior), we instead tie the animation
/// events to the action FSM states and use this for the animation events instead.
/// </summary>
public class AnimEventPlr : MonoBehaviour{
    /// <summary>
    /// Max animation loops before animation is forced to restart to avoid timing precision problems.
    /// </summary>
    const int loopRebaseThreshold = 10000;
    /// <summary>
    /// Safety cap for animation loops per tick.
    /// </summary>
    const int maxLoopStepsPerTick = 500;

    /// <summary>
    /// Starts crossfade and initializes animation event data.
    /// </summary>
    public static void CrossfadeNInitAnimEventPlr(
        ref AnimEventPlrData animEventPlrData,
        Animator anim,
        AnimInfo animInfo,
        Action<int, CapsuleCharAnimEventT> animEventAction,
        float nrmTransDur = 0.1f,
        float startOffset = 0
    ) {
        anim.CrossFade(
            animInfo.shortNameHash,
            nrmTransDur,
            animInfo.animLayer,
            startOffset
        );
        InitAnimEventPlrData(
            ref animEventPlrData,
            animInfo,
            animEventAction,
            startOffset
        );
    }

    /// <summary>
    /// Creates an array of animation events with their normalized times calculated from their frame indices.
    /// </summary>
    /// <param name="lastFrame">
    /// Index of the last frame of the animation (the one in the Animation timeline where the color changes).
    /// </param>
    /// <param name="events">Animation events specified as frame index and unique event ID pairs.</param>
    public static AnimEvent[] CreateAnimEvents(
        AnimInfo animInfo,
        params (int frame, CapsuleCharAnimEventT id)[] events
    ) {
        AnimEvent[] result = new AnimEvent[events.Length];
        for (int i = 0; i < events.Length; i++)
            result[i] = new AnimEvent(events[i].frame, animInfo.lastFrame, events[i].id);
        return result;
    }

    /// <summary>
    /// Call this after action state machine has started a crossfade to a new state to initialize
    /// animation events for the next animation.<br/>
    /// NOTE: THE ANIMATION EVENTS NEED TO BE SORTED ASCENDING BY NORMALIZED TIME!!!
    /// </summary>
    public static void InitAnimEventPlrData(
        ref AnimEventPlrData animEventPlrData,
        AnimInfo animInfo,
        Action<int, CapsuleCharAnimEventT> onEvent,
        float startOffset = 0
    ) {
        animEventPlrData.animInfo = animInfo;
        animEventPlrData.prevTotalNrmT = startOffset;
        animEventPlrData.cursor = startOffset;
        animEventPlrData.loopCount = 0;
        animEventPlrData.loopsSinceRebase = 0;
        animEventPlrData.finished = false;
        animEventPlrData.firstTick = true;
    }

    /// <summary>
    /// NOTE: Tick this in LateUpdate to make sure that the queued animator changes during this
    /// frame Update have already been applied!
    /// </summary>
    public static void Tick(
        int caId,
        ref AnimEventPlrData data,
        Animator anim,
        Action<int, CapsuleCharAnimEventT> animEventAction
    ) {
        if (data.finished) {
            data.firstTick = false;
            return;
        }
        if (!VisUtils.TryGetNewestStInfo(
            anim,
            data.animInfo.animLayer,
            data.animInfo.shortNameHash,
            out AnimatorStateInfo info
        )) {
            Debug.LogWarning(
                $"Short name hash ({data.animInfo.shortNameHash}) did not match current animation "
                + $"({info.shortNameHash}).\nPerhaps action state was "
                + "changed but animator transition hasn't have the time to start yet?"
            );
            data.firstTick = false;
            return;
        }
        float curTotalNrmT = info.normalizedTime;
        if (!data.animInfo.looping) {
            FireEventsInNrmRange(
                caId,
                data,
                data.cursor,
                Mathf.Min(curTotalNrmT, 1),
                data.firstTick,
                animEventAction
            );
            if (curTotalNrmT >= 1)
                data.finished = true;
            data.cursor = Mathf.Min(curTotalNrmT, 1);
            data.firstTick = false;
            return;
        }
        float dTotalNrmT = curTotalNrmT - data.prevTotalNrmT;
        data.prevTotalNrmT = curTotalNrmT;
        int stepsTaken = 0;
        while (dTotalNrmT > 0) {
            float toLoopEnd = 1 - data.cursor;
            if (dTotalNrmT < toLoopEnd) {
                FireEventsInNrmRange(
                    caId,
                    data,
                    data.cursor,
                    data.cursor + dTotalNrmT,
                    data.firstTick,
                    animEventAction
                );
                data.cursor += dTotalNrmT;
                dTotalNrmT = 0;
            }
            else {
                FireEventsInNrmRange(caId, data, data.cursor, 1, data.firstTick, animEventAction);
                dTotalNrmT -= toLoopEnd;
                data.cursor = 0;
                data.loopCount++;
                data.loopsSinceRebase++;
                // If animation looped too many times this tick, 
                if (++stepsTaken >= maxLoopStepsPerTick) {
                    Debug.LogError($"Animation looped {stepsTaken} times during one tick! " 
                        + $"Max allowed loops: {maxLoopStepsPerTick}");
                    data.firstTick = false;
                    return;
                }
                // If we have looped too many times and the cursor is at the beginning of the loop,
                // start animation from the beginning.
                if (data.loopsSinceRebase >= loopRebaseThreshold && data.cursor == 0) {
                    anim.Play(data.animInfo.shortNameHash, data.animInfo.animLayer, 0);
                    data.prevTotalNrmT = 0;
                    data.loopsSinceRebase = 0;
                }
            }
        }
        data.firstTick = false;
    }

    /// <summary>
    /// Invokes events.
    /// NOTE: from and to need to be normalized!
    /// </summary>
    static void FireEventsInNrmRange(
        int caId,
        in AnimEventPlrData data,
        float from,
        float to,
        bool includeFrom,
        Action<int, CapsuleCharAnimEventT> animEventAction
    ) {
        // Find first event in the range. NOTE that it doesn't include "from", but does include
        // "to". This way events do not fire twice. That also means we need a separate check for
        // first tick to fire any possible events at 0 normalized time.
        int startIndex = includeFrom
            ? LowerBoundInclusive(data.animInfo.sortedAnimEvents, from)
            : LowerBound(data.animInfo.sortedAnimEvents, from);
        for (int i = startIndex; i < data.animInfo.sortedAnimEvents.Length; i++) {
            float t = data.animInfo.sortedAnimEvents[i].nrmT;
            if (t > to) break;
            //Debug.Log($"Event called: {animInfo.sortedAnimEvents[i].id}.");
            animEventAction?.Invoke(caId, data.animInfo.sortedAnimEvents[i].id);
        }
    }

    /// <summary>
    /// Finds the index of the first event that has its normalized time above the threshold.
    /// </summary>
    static int LowerBound(AnimEvent[] events, float nrmTThreshold) {
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
    static int LowerBoundInclusive(AnimEvent[] events, float nrmTThreshold) {
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
