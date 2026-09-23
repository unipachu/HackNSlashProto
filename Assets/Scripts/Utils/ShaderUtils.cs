using UnityEngine;

public static class ShaderUtils{
    /// <summary>
    /// Use normalized value (0-1) to set dither amount.
    /// </summary>
    public static void SetDitherAmount(Renderer tgtRenderer, float nrmValue) {
        //Debug.Log($"Dither nrm value: {nrmValue},\n"
        //    + $"actual value: {Mathf.Lerp(1f / 17f, 16f / 17f, Mathf.Clamp01(nrmValue))}"
        //);
        tgtRenderer.material.SetFloat(
            "_ditherAmount",
            // TODO: These seem to be the right lerp values but I have no idea why and google gave no results.
            Mathf.Lerp(1.5f, 0.5f, Mathf.Clamp01(nrmValue))
        );
    }

    public static void SetColor(Renderer tgtRenderer, Color value) {
        tgtRenderer.material.SetColor("_color", value);
    }

    /// <param name="value">
    /// Should probably be around 1, and >= 0 and <= 100.
    /// </param>
    public static void SetFresnelAmount(Renderer tgtRenderer, float value) {
        tgtRenderer.material.SetFloat("_fresnelAmount", value);
    }
}
