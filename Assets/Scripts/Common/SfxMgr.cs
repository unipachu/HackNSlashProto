using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Plays sound effects using a pool of reusable <see cref="AudioSource"/>s.<br/>
/// NOTE: Only works for static sound effects (with no doppler effect). Moving AudioSources should likely
/// be attached to the game objects that are supposed to make the sound.
/// </summary>
public class SfxMgr : Singleton<SfxMgr> {
    [Header("Default")]
    [SerializeField] AudioClip defaultPierce;
    [SerializeField] AudioClip defaultLightSlam;

    [Header("Metal")]
    [SerializeField] AudioClip metalPierce;
    [SerializeField] AudioClip metalLightSlam;

    [Header("Source Pool")]
    [SerializeField] int poolSize = 32;

    [Header("Default Sound Settings")]
    [SerializeField] float defaultVol = 1;
    [SerializeField, Range(0f, 1f)] float defaultSpatialBlend = 1;
    [SerializeField] AudioRolloffMode defaultRolloffMode = AudioRolloffMode.Linear;
    [SerializeField] float defaultminDist = 5;
    [SerializeField] float defaultMaxDist = 30;
    [SerializeField] AudioMixerGroup outputAudioMixerGroup;

    AudioSource[] sfxSrcs;

    public void Init() {
        if (poolSize <= 0) {
            Debug.LogError("SfxMgr pool size must be greater than zero.", this);
            return;
        }
        sfxSrcs = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++) {
            GameObject sfxObj = new($"SfxSrc_{i}");
            sfxObj.transform.SetParent(transform);
            AudioSource audioSrc = sfxObj.AddComponent<AudioSource>();
            audioSrc.playOnAwake = false;
            audioSrc.loop = false;
            audioSrc.volume = defaultVol;
            audioSrc.spatialBlend = defaultSpatialBlend;
            audioSrc.rolloffMode = defaultRolloffMode;
            audioSrc.minDistance = defaultminDist;
            audioSrc.maxDistance = defaultMaxDist;
            // TODO MAYBE: Audio srcs should be attached to moving objects for doppler effect to propely work.
            // C: So this manager is not fit for it at all, but this is good enough for this project.
            audioSrc.dopplerLevel = 0;
            audioSrc.outputAudioMixerGroup = outputAudioMixerGroup;
            sfxSrcs[i] = audioSrc;
        }
    }

    /// <summary>
    /// Can be called anywhere to play a sound effect as 2D audio.
    /// </summary>
    public void PlaySfx(SfxId sfxId) {
        AudioClip clip = sfxId switch {
            SfxId.DefaultPierce => defaultPierce,
            SfxId.DefaultLightSlam => defaultLightSlam,
            SfxId.MetalPierce => metalPierce,
            SfxId.MetalLightSlam => metalLightSlam,
            _ => null
        };
        if (clip == null) {
            Debug.LogError($"No AudioClip assigned for {sfxId}.", this);
            return;
        }
        AudioSource audioSrc = null;
        for (int i = 0; i < sfxSrcs.Length; i++) {
            if (!sfxSrcs[i].isPlaying) {
                audioSrc = sfxSrcs[i];
                break;
            }
        }
        if (audioSrc == null) {
            Debug.LogWarning($"SfxMgr pool empty while playing {sfxId}. Consider pooling more audio srcs", this);
            return;
        }
        audioSrc.volume = defaultVol;
        audioSrc.spatialBlend = 0;
        audioSrc.rolloffMode = defaultRolloffMode;
        audioSrc.minDistance = defaultminDist;
        audioSrc.maxDistance = defaultMaxDist;
        audioSrc.transform.position = transform.position; // Makes it easier to debug sounds.
        audioSrc.clip = clip;
        audioSrc.Play();
    }

    /// <summary>
    /// Can be called anywhere to play a sound effect.
    /// </summary>
    public void PlaySfx(SfxId sfxId, Vector3 pos) {
        AudioClip clip = sfxId switch {
            SfxId.DefaultPierce => defaultPierce,
            SfxId.DefaultLightSlam => defaultLightSlam,
            SfxId.MetalPierce => metalPierce,
            SfxId.MetalLightSlam => metalLightSlam,
            _ => null
        };
        if (clip == null) {
            Debug.LogError($"No AudioClip assigned for {sfxId}.", this);
            return;
        }
        AudioSource audioSrc = null;
        for (int i = 0; i < sfxSrcs.Length; i++) {
            if (!sfxSrcs[i].isPlaying) {
                audioSrc = sfxSrcs[i];
                break;
            }
        }
        if (audioSrc == null) {
            Debug.LogWarning($"SfxMgr pool empty while playing {sfxId}. Consider pooling more audio srcs", this);
            return;
        }
        audioSrc.volume = defaultVol;
        audioSrc.spatialBlend = defaultSpatialBlend;
        audioSrc.rolloffMode = defaultRolloffMode;
        audioSrc.minDistance = defaultminDist;
        audioSrc.maxDistance = defaultMaxDist;
        audioSrc.transform.position = pos;
        audioSrc.clip = clip;
        audioSrc.Play();
    }

    /// <summary>
    /// Can be called anywhere to play a sound effect.
    /// </summary>
    public void PlaySfx(
        SfxId sfxId,
        Vector3 pos,
        float vol,
        float spatialBlend,
        AudioRolloffMode rolloffMode,
        float minDist,
        float maxDist
    ) {
        AudioClip clip = sfxId switch {
            SfxId.DefaultPierce => defaultPierce,
            SfxId.DefaultLightSlam => defaultLightSlam,
            SfxId.MetalPierce => metalPierce,
            SfxId.MetalLightSlam => metalLightSlam,
            _ => null
        };
        if (clip == null) {
            Debug.LogError($"No AudioClip assigned for {sfxId}.", this);
            return;
        }
        AudioSource audioSrc = null;
        for (int i = 0; i < sfxSrcs.Length; i++) {
            if (!sfxSrcs[i].isPlaying) {
                audioSrc = sfxSrcs[i];
                break;
            }
        }
        if (audioSrc == null) {
            Debug.LogWarning($"SfxMgr pool empty while playing {sfxId}. Consider pooling more audio srcs", this);
            return;
        }
        audioSrc.volume = vol;
        audioSrc.spatialBlend = spatialBlend;
        audioSrc.rolloffMode = rolloffMode;
        audioSrc.minDistance = minDist;
        audioSrc.maxDistance = maxDist;
        audioSrc.transform.position = pos;
        audioSrc.clip = clip;
        audioSrc.Play();
    }
}