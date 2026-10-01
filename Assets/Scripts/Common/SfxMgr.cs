using UnityEngine;

/// <summary>
/// Plays sound effects.
/// </summary>
public class SfxMgr : Singleton<SfxMgr>{
    [Header("Default")]
    [SerializeField] AudioClip defaultPierce;
    [SerializeField] AudioClip defaultLightSlam;

    [Header("Metal")]
    [SerializeField] AudioClip metalPierce;
    [SerializeField] AudioClip metalLightSlam;

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
        AudioSource.PlayClipAtPoint(clip, pos);
    }
}
