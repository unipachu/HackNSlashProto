using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  Utility and extension methods for Animator, Mesh, Material, and other visual-only types (excluding UI).
/// </summary>
public static class VisUtils {
    /// <summary>
    /// Creates an array of animation events with their normalized times calculated from their frame indices.
    /// </summary>
    /// <param name="lastFrame">
    /// Index of the last frame of the animation (the one in the Animation timeline where the color changes).
    /// </param>
    /// <param name="events">Animation events specified as frame index and unique event ID pairs.</param>
    public static ActAnimEvent[] CreateAnimEvents(
        AnimInfo animInfo,
        params (int frame, string id)[] events
    ) {
        ActAnimEvent[] result = new ActAnimEvent[events.Length];
        for (int i = 0; i < events.Length; i++)
            result[i] = new ActAnimEvent(events[i].frame, animInfo.lastFrame, events[i].id);
        return result;
    }

    /// <summary>
    /// Starts a crossfade to an animation state.
    /// </summary>
    /// <param name="anim">Animator whose state is changed.</param>
    /// <param name="animInfo">Information about the animation state to crossfade to.</param>
    /// <param name="nrmTransDur">Duration of the crossfade in the next animation normalized time.</param>
    /// <param name="startOffset">Normalized starting time within the next animation.</param>
    public static void CrossfadeAnim(
        Animator anim,
        AnimInfo animInfo,
        float nrmTransDur = 0.1f,
        float startOffset = 0
    ) {
        anim.CrossFade(
            animInfo.shortNameHash,
            nrmTransDur,
            animInfo.animLayer,
            startOffset
        );
    }

    /// <summary>
    /// Flashes the given mesh renderer between two materials for a specified duration.
    /// </summary>
    public static IEnumerator FlashMeshCoroutine(
        MeshRenderer meshRenderer,
        Material matA,
        Material matB,
        float flashDuration = 1.5f,
        float flashInterval = 0.2f,
        Action onComplete = null
    ) {
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration) {
            // Switch materials based on the flash interval.
            meshRenderer.material = (elapsedTime % flashInterval < flashInterval / 2)
                ? matA
                : matB;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Invoke the callback if provided.
        onComplete?.Invoke();
    }

    /// <summary>
    /// Flashes mesh between two materials based on Time.time. Has to be called each frame to work.
    /// </summary>
    public static void FlashMeshUpdate(
        this MeshRenderer meshRenderer,
        Material matA,
        Material matB,
        float flashInterval = 0.2f
    )
        => meshRenderer.material = (Time.time % flashInterval < flashInterval / 2)
            ? matA
            : matB;

    /// <returns>
    /// Normalized time (0-1 for non looping animations) of the "current animation state", or "next animation state" if in transition.
    /// </returns>
    public static float GetNewestStNrmTime(Animator anim, int animLayer)
        => anim.IsInTransition(animLayer)
            ? anim.GetNextAnimatorStateInfo(animLayer).normalizedTime
            : anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime;

    /// <summary>
    /// Returns current state info or next state info if in transition.
    /// </summary>
    public static AnimatorStateInfo GetNewestStInfo(Animator anim, int animLayer)
        => anim.IsInTransition(animLayer)
            ? anim.GetNextAnimatorStateInfo(animLayer)
            : anim.GetCurrentAnimatorStateInfo(animLayer);

    /// <summary>
    /// Gets the number of samples (frames) in the AnimationClip of the current state on a specified Animator layer.
    /// Throws exceptions if the Animator, layer, or state is invalid.
    /// </summary>
    /// <param name="animator">The Animator to check.</param>
    /// <param name="layerIndex">The layer index where the state exists.</param>
    /// <returns>The number of samples (frames) in the clip.</returns>
    public static int GetNumberOfSamplesOfCurrentState(Animator animator, int layerIndex) {
        if (animator == null)
            throw new ArgumentNullException(nameof(animator), "Animator cannot be null.");
        if (layerIndex < 0 || layerIndex >= animator.layerCount)
            throw new ArgumentOutOfRangeException(
                nameof(layerIndex),
                $"Layer index {layerIndex} is out of range. Animator has {animator.layerCount} layers."
            );
        //Get all clips currently playing on the layer
        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (clipInfos.Length == 0 || clipInfos[0].clip == null)
            throw new InvalidOperationException($"No AnimationClip found for the current state on layer {layerIndex}.");
        AnimationClip clip = clipInfos[0].clip;
        // Calculate number of frames / samples
        int numSamples = Mathf.RoundToInt(clip.frameRate * clip.length);
        return numSamples;
    }

    /// <returns>
    /// The hash of the current animator state if not in transition, or the the hash of the next animator state if in transition.
    /// </returns>
    public static int HashOfActiveAnimatorState(this Animator animator, int animatorLayer) {
        return animator.IsInTransition(animatorLayer)
            ? animator.GetCurrentAnimatorStateInfo(animatorLayer).shortNameHash
            : animator.GetNextAnimatorStateInfo(animatorLayer).shortNameHash;
    }

    /// <returns>
    /// True if currently in the specified state (if not in transition) or transitioning into the specified
    /// state (if in transition) in the Animator.
    /// </returns>
    public static bool IsActiveAnimatorState(this Animator animator, int animatorLayer, int stateHash) {
        if (animator.IsInTransition(animatorLayer)) {
            AnimatorStateInfo next =
                animator.GetNextAnimatorStateInfo(animatorLayer);
            //Debug.Log("next hash: " + next.shortNameHash + ". saved hash: " + stateHash);
            if (next.shortNameHash == stateHash)
                return true;
        }
        else {
            AnimatorStateInfo current =
            animator.GetCurrentAnimatorStateInfo(animatorLayer);
            //Debug.Log("current hash: " + current.shortNameHash + ". saved hash: " + stateHash);
            if (current.shortNameHash == stateHash)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Starts crossfade and initializes animation event player.
    /// </summary>
    /// <param name="animEventPlr">
    /// Animation event player to initialize.
    /// </param>
    /// <param name="anim">
    /// Animator whose animation state is being changed.
    /// </param>
    /// <param name="animInfo">
    /// Information about the animation state to crossfade to.
    /// </param>
    /// <param name="sortedAnimEvents">
    /// Animation events for the animation, sorted in ascending order by normalized time.
    /// </param>
    /// <param name="onEvent">
    /// Callback invoked when an animation event is triggered. The string parameter is the
    /// unique ID/name of the triggered animation event e.g. "TurnOnInvulnerability".
    /// </param>
    /// <param name="nrmTransDur">
    /// Duration of the crossfade in normalized time of the next animation.
    /// </param>
    /// <param name="startOffset">
    /// Normalized starting time within the next animation.
    /// </param>
    public static void CrossfadeNInitAnimEventPlr(
        ref AnimEventPlr animEventPlr,
        Animator anim,
        AnimInfo animInfo,
        ActAnimEvent[] sortedAnimEvents,
        Action<string> onEvent,
        float nrmTransDur = 0.1f,
        float startOffset = 0
    ) {
        anim.CrossFade(
            animInfo.shortNameHash,
            nrmTransDur,
            animInfo.animLayer,
            startOffset
        );

        animEventPlr = new AnimEventPlr(
            anim,
            animInfo,
            sortedAnimEvents,
            onEvent,
            startOffset
        );
    }

    /// <summary>
    /// Gets newest animator state info and returns true if matches with shortNameHash.
    /// </summary>
    /// <param name="shortNameHash">State hash of the expected newest animator state.</param>
    public static bool TryGetNewestStateInfo(Animator anim, int animLayer, int shortNameHash, out AnimatorStateInfo info) {
        info = GetNewestStInfo(anim, animLayer);
        return info.shortNameHash == shortNameHash;
    }
}
