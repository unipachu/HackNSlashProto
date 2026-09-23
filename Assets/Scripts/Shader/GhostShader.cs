// TODO: Delete
using System;
using UnityEngine;

[Obsolete]
public class GhostShader : MonoBehaviour{
    [SerializeField] Renderer tgtRenderer;

    public void SetTransparency(float value) {
        tgtRenderer.material.SetFloat("_ditherAmount", value);
    }

    public void SetFresnelAmount(float value) {
        tgtRenderer.material.SetFloat("_fresnelAmount", value);
    }
}
