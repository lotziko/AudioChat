using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace AudioChat
{
	public enum AppType
	{
		PC, VR
	}

	public interface IXRManager
	{
		AppType GetAppType();
	}

	public class XRManager : IXRManager
	{
		private AppType? _type;

		AppType IXRManager.GetAppType()
		{
			if (!_type.HasValue)
			{
				List<XRDisplaySubsystem> subsystems = new List<XRDisplaySubsystem>();
				SubsystemManager.GetInstances(subsystems);
				_type = subsystems.Count > 0 ? AppType.VR : AppType.PC;
			}
			return _type.Value;
		}
	}
}
