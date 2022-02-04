using UnityEditor;
using UnityEngine;

namespace AudioChat.Editor
{
	[CustomEditor(typeof(PreviewHand))]
	public class PreviewHandEditor : UnityEditor.Editor
	{
		private PreviewHand _hand;
		private Transform _activeJoint;

		private void OnEnable()
		{
			_hand = target as PreviewHand;
		}

		private void OnSceneGUI()
		{
			DrawHandles();
			DrawActiveHandle();
		}

		private void DrawHandles()
		{
			foreach (Transform joint in _hand.Joints)
			{
				bool isActive = Handles.Button(joint.position, joint.rotation, 0.01f, 0.005f, Handles.SphereHandleCap);
				if (isActive)
				{
					_activeJoint = joint;
					return;
				}
			}
		}

		private void DrawActiveHandle()
		{
			if (_activeJoint != null)
			{
				Quaternion currentRotation = _activeJoint.rotation;

				EditorGUI.BeginChangeCheck();
				Quaternion rotation = Handles.RotationHandle(currentRotation, _activeJoint.position);
				if (EditorGUI.EndChangeCheck())
				{
					_activeJoint.rotation = rotation;
					Undo.RegisterCompleteObjectUndo(_activeJoint, "Changed Joint Rotation");
				}
			}
		}
	}
}
