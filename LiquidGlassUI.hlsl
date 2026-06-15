```hlsl
// ==========================================================
// HDRP Custom Shader Graph Node HLSL - Liquid Glass UI Effect
// To be used inside a Unity Shader Graph (Custom Function Node)
// ==========================================================

void LiquidGlassEffect_float(
    float2 UV, 
    UnityTexture2D MainTex, 
    UnitySamplerState Sampler, 
    float DistortionStrength, 
    float ChromaticAberration, 
    float TimeValue,
    out float4 OutColor)
{
    // Simulate liquid movement via sine waves
    float2 noiseUV = UV;
    noiseUV.x += sin(UV.y * 10.0 + TimeValue * 2.0) * 0.01 * DistortionStrength;
    noiseUV.y += cos(UV.x * 10.0 + TimeValue * 2.0) * 0.01 * DistortionStrength;

    // Chromatic Aberration sampling
    float r = MainTex.Sample(Sampler, noiseUV + float2(ChromaticAberration, 0.0)).r;
    float g = MainTex.Sample(Sampler, noiseUV).g;
    float b = MainTex.Sample(Sampler, noiseUV - float2(ChromaticAberration, 0.0)).b;
    float a = MainTex.Sample(Sampler, noiseUV).a;

    // Edge highlight (Simulating glass refraction rim)
    float edge = 1.0 - smoothstep(0.4, 0.5, distance(UV, float2(0.5, 0.5)));
    float3 finalColor = float3(r, g, b) + (edge * 0.2); // Add subtle bloom rim

    OutColor = float4(finalColor, a);
}

```
