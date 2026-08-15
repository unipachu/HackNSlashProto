using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  Utility and extension methods for Animator, Mesh, Material, and other visual-only types.
/// </summary>
public static class VisUtils {
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
    ) {
        // Switch materials based on the flash interval.
        meshRenderer.material = (Time.time % flashInterval < flashInterval / 2)
            ? matA
            : matB;
    }

    /// <returns>
    /// Normalized time (0-1 for non looping animations) of the "current animation state", or "next animation state" if in transition.
    /// </returns>
    public static float GetMostRecentAnimationNormalizedTime(Animator animator, int layer = 0) {
        return animator.IsInTransition(layer)
            ? animator.GetNextAnimatorStateInfo(layer).normalizedTime
            : animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
    }

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
}
