using System;
using UnityEngine;

namespace AudioChat.Pose
{
	public class PoseContainer : MonoBehaviour
	{
		[SerializeField] private PosePreset _preset;
		
		public PosePreset Preset
		{ get { return _preset; } }

		[Serializable]
		public class PosePreset
		{
			public string Name;
			public HandPose Pose;
		}
	}
}
