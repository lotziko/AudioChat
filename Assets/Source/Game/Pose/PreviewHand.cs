using AudioChat.Pose;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioChat.Editor
{
	public enum MirrorAxis
	{
		X, Y, Z
	}

	[SelectionBase]
	[ExecuteInEditMode]
	public class PreviewHand : MonoBehaviour
	{
		[SerializeField] private HandPose _defaultPose;
		[SerializeField] private Hand _hand;
		[SerializeField] private List<Transform> _jointRoots;

		private List<Transform> _jointTransforms;

		public List<Transform> Joints
		{ get { return _jointTransforms; } }

		// ===============================================================

		private void Awake()
		{
			_jointTransforms = GetJointTransforms();
		}

		// ===============================================================

		public void MirrorToHand(PreviewHand hand, MirrorAxis axis = MirrorAxis.X)
		{
			List<Quaternion> mirroredJoints = MirrorJoints(hand.Joints);
			ApplyJointRotations(mirroredJoints);

			transform.localPosition = MirrorPosition(hand.transform, axis);
			transform.localRotation = MirrorRotation(hand.transform, axis);
		}

		public void ApplyDefaultPose()
		{
			ApplyPose(_defaultPose);
		}

		public void ApplyPose(HandPose pose)
		{
			if (pose == null)
				Debug.LogError("Pose is null.");

			HandInfo info = pose.GetInfo(_hand);

			ApplyJointRotations(info.JointRotations);

			transform.localPosition = info.AttachPosition;
			transform.localRotation = info.AttachRotation;
		}

		public void ApplyJointRotations(List<Quaternion> rotations)
		{
			if (rotations.Count == _jointTransforms.Count)
				for (int i = 0; i < _jointTransforms.Count; i++)
					_jointTransforms[i].localRotation = rotations[i];
		}

		public List<Quaternion> GetJointRotations()
		{ return _jointTransforms.Select((t) => t.localRotation).ToList();	}

		// ===============================================================

		private List<Transform> GetJointTransforms()
		{
			List<Transform> result = new List<Transform>();

			foreach (Transform root in _jointRoots)
				result.AddRange(root.GetComponentsInChildren<Transform>().Where((t) => t.childCount > 0));

			return result;
		}

		private List<Quaternion> MirrorJoints(List<Transform> joints)
		{
			List<Quaternion> result = new List<Quaternion>();

			foreach (Transform joint in joints)
				result.Add(MirrorJointRotation(joint));

			return result;
		}

		private Quaternion MirrorJointRotation(Transform joint)
		{
			Quaternion rotation = joint.localRotation;
			return rotation;
		}

		private Vector3 MirrorPosition(Transform transform, MirrorAxis axis)
		{
			Vector3 position = transform.localPosition;
			switch (axis)
			{
				case MirrorAxis.X:
					position.x *= -1f;
					break;
				case MirrorAxis.Y:
					position.y *= -1f;
					break;
				case MirrorAxis.Z:
					position.z *= -1f;
					break;
			}
			return position;
		}

		private Quaternion MirrorRotation(Transform otherTransform, MirrorAxis axis)
		{
			Transform center = otherTransform.parent;
			Vector3 direction = Vector3.zero;
			switch (axis)
			{
				case MirrorAxis.X:
					direction = Vector3.right;
					break;
				case MirrorAxis.Y:
					direction = Vector3.up;
					break;
				case MirrorAxis.Z:
					direction = Vector3.forward;
					break;
			}

			Vector3 forward = otherTransform.forward;
			Vector3 up = otherTransform.up;

			if (otherTransform.parent != null)
			{
				forward = otherTransform.parent.InverseTransformDirection(otherTransform.forward);
				up = otherTransform.parent.InverseTransformDirection(otherTransform.up);
			}

			Vector3 reflectedForward = Vector3.Reflect(forward, direction);
			Vector3 reflectedUp = Vector3.Reflect(up, direction);

			return Quaternion.LookRotation(reflectedForward, reflectedUp);
		}
	}
}
