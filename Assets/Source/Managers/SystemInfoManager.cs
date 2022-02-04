using System.Collections.Generic;
using UnityEngine;

namespace AudioChat
{
	public interface ISystemInfoManager
	{
		List<VolumeCalculationMode> GetAvailableVolumeCalculationModes();
		bool IsRaytracingAvailable();
	}

	public class SystemInfoManager : ISystemInfoManager
	{
		List<VolumeCalculationMode> ISystemInfoManager.GetAvailableVolumeCalculationModes()
		{
			List<VolumeCalculationMode> result = new List<VolumeCalculationMode>
			{
				VolumeCalculationMode.None
			};
			if (IsRaytracingAvailable())
				result.Add(VolumeCalculationMode.Raytracing);
			return result;
		}

		bool ISystemInfoManager.IsRaytracingAvailable()
		{
			return IsRaytracingAvailable();
		}

		private bool IsRaytracingAvailable()
		{
			return SystemInfo.supportsRayTracing && SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D12;
		}
	}
}
