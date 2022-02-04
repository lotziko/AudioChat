using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace AudioChat
{
	public static class DebugRayGenerator
	{
		public static List<Ray> Generate(Vector2Int dimensions, Matrix4x4 inverseView, Matrix4x4 inverseProjection, Vector3 cameraPos)
		{
			List<Ray> result = new List<Ray>();
			float2 dispatchDim = float2(dimensions.x, dimensions.y);
			float3 worldCameraPos = float3(cameraPos);

			for (int i = 0; i < dimensions.x; i++)
			{
				for (int j = 0; j < dimensions.y; j++)
				{
					float2 texcoord = float2(i + 0.5f, j + 0.5f) / dispatchDim;
					float3 viewPosition = float3(texcoord * 2 - float2(1, 1), 0.0f);
					float4 clip = float4(viewPosition.xyz, 1.0f);
					float4 viewPos = mul(inverseProjection, clip);
					viewPos.xyz /= viewPos.w;
					float4 worldPos = mul(inverseView, viewPos);
					float3 worldDirection = worldPos.xyz - worldCameraPos;

					Ray ray = new Ray(cameraPos, normalize(worldDirection));
					result.Add(ray);
				}
			}
			
			return result;
		}
	}
}
