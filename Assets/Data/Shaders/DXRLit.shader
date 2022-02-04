Shader "Custom/DXRLit"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
		[MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
		_SoundAbsorptionCoefficient("Sound Absorption Coefficient", Range(0, 1)) = 1
		[Toggle(IS_TARGET)]_IsTarget("Enable target", Float) = 0
    }
    SubShader
    {
		Tags { "RenderType" = "Opaque" }//"Queue" = "Geometry" "RenderPipeline" = "UniversalRenderPipeline"}
        LOD 300

        Pass
        {
			Name "Forward"
			Tags{"LightMode" = "UniversalForward"}

			Blend One Zero
			ZWrite On
			Cull Back

			HLSLPROGRAM
			#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.5

			// -------------------------------------
			// Universal Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment

			#include "DXRLitInput.hlsl"
			#include "DXRLitForwardPass.hlsl"
			ENDHLSL
        }

		Pass
		{
			Name "RayPass"
			Tags{ "LightMode" = "RayTracing" }

			HLSLPROGRAM

			#include "UnityShaderVariables.cginc"
			#include "UnityRaytracingMeshUtils.cginc"
			#include "Common.cginc"

			#pragma raytracing test
			#pragma multi_compile __ IS_TARGET

			float _SoundAbsorptionCoefficient;

			[shader("closesthit")]
			void ClosestHitMain(inout Payload payload : SV_RayPayload, AttributeData attributeData : SV_IntersectionAttributes)
			{
				if (payload.depth > maxDepth)
					return;

				payload.distance += RayTCurrent();
				payload.depth += 1;

				#ifdef IS_TARGET
				payload.result = 1.0h;
				return;
				#else
				IntersectionVertex currentvertex;
				GetCurrentIntersectionVertex(attributeData, currentvertex);

				float3x3 objectToWorld = (float3x3)ObjectToWorld3x4();
				float3 worldNormal = normalize(mul(objectToWorld, currentvertex.normalOS));

				float3 rayOrigin = WorldRayOrigin();
				float3 rayDir = WorldRayDirection();

				float3 worldPos = rayOrigin + RayTCurrent() * rayDir;

				float3 reflection = reflect(rayDir, worldNormal);

				RayDesc rayDesc;
				rayDesc.Origin = worldPos;
				rayDesc.Direction = reflection;
				rayDesc.TMin = 0;
				rayDesc.TMax = 10000;

				payload.volume *= _SoundAbsorptionCoefficient;

				/*Payload newPayload;
				newPayload.result = payload.result;
				newPayload.depth = payload.depth;
				newPayload.distance = payload.distance;
				newPayload.volume = payload.volume;*/

				TraceRay(_RaytracingAccelerationStructure, 0, 0xFFFFFFF, 0, 1, 0, rayDesc, payload);

				//payload.volume = newPayload.volume * _SoundAbsorptionCoefficient;//abs(20*log10((newPayload.end - rayOrigin)/(worldPos - rayOrigin + newPayload.end - worldPos)) + 20 * log10(1 - _SoundAbsorptionCoefficient));

				//payload.result = newPayload.result;
				//payload.distance = newPayload.distance;
				//payload.depth = newPayload.depth;
				//payload.color = _BaseColor.xyz * (newPayload.color / 2.0);
				#endif
			}

			ENDHLSL
		}
    }
}
