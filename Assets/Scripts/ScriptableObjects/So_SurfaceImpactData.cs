// TODO: Delete
using System;
using UnityEngine;

/// <summary>
/// Enivronmental surfaces can use this data to mark what effects they use as they get hit.
/// </summary>
[Obsolete]
public class So_SurfaceImpactData : ScriptableObject{
    public ParticleSystem impactParticles;
    public AudioClip[] impactSounds;
    public Material impactDecal;
}
