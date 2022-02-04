#ifndef WHITEBOARD_LIT_INPUT_INCLUDED
#define WHITEBOARD_LIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
CBUFFER_END

TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
TEXTURE2D(_PaintMap);SAMPLER(sampler_PaintMap);

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
{
    half4 mainColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    half4 paintColor = SAMPLE_TEXTURE2D(_PaintMap, sampler_PaintMap, uv);
    
    outSurfaceData.albedo = mainColor.rgb * (1 - paintColor.a) + paintColor.rgb * _BaseColor.rgb;
    outSurfaceData.alpha = 1.0h;
    outSurfaceData.metallic = 0.0h;
    outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
    outSurfaceData.smoothness = 0.0h;
    outSurfaceData.occlusion = 1.0h;
    outSurfaceData.emission = half3(0.0h, 0.0h, 0.0h);
    outSurfaceData.normalTS = half3(0.0h, 0.0h, 1.0h);
    outSurfaceData.clearCoatMask = 0.0h;
    outSurfaceData.clearCoatSmoothness = 0.0h;
}

#endif