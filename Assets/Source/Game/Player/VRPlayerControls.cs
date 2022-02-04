using UnityEngine;

namespace AudioChat
{
	public class VRPlayerControls : MonoBehaviour
	{
		[SerializeField] private PlayerInput _input;
		[SerializeField] private PlayerController _controller;
		[SerializeField] private Transform _cameraTransform;

		private Vector2 _movementDelta;

		// =============================================================

		private void OnEnable()
		{
			_input.OnAxis += OnAxis;
		}

		private void OnDisable()
		{
			_input.OnAxis -= OnAxis;
		}

		private void Update()
		{
			_controller.MoveCharacter(_movementDelta);
			//_controller.SetAvatarLocalPosition(_cameraTransform.localPosition);
			//_controller.SetAvatarLocalRotation(_cameraTransform.forward);
		}

		// =============================================================

		private void OnAxis(Vector2 value)
		{
			_movementDelta = value;
		}
	}
}
