using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AudioChat.Pose.PoseContainer;

namespace AudioChat
{
	public enum Hand
	{
		Left, Right
	}

	public class PosableHand : MonoBehaviour
	{
		[SerializeField] private Hand _hand;
		[SerializeField] private List<Transform> _fingerRoots;

		private List<Transform> _jointTransforms;

		// =============================================================

		private void Awake()
		{
			_jointTransforms = GetJointTransforms();
		}

		// =============================================================

		public void Apply(PosePreset preset)
		{
			Pose.HandInfo info = preset.Pose.GetInfo(_hand);
			for (int i = 0; i < info.JointRotations.Count; i++)
				_jointTransforms[i].localRotation = info.JointRotations[i];
		}

		// =============================================================

		private List<Transform> GetJointTransforms()
		{
			List<Transform> result = new List<Transform>();
			foreach (Transform root in _fingerRoots)
				result.AddRange(root.GetComponentsInChildren<Transform>().Where((t) => t.childCount > 0));
			return result;
		}
	}
}
