using System;
using Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AudioChat
{
	public class PlayerInput : MonoBehaviour
	{
		DIVar<IInputManager> _inputManager = new DIVar<IInputManager>();

		[SerializeField] private InputActionProperty _axisInputAction;
		[SerializeField] private InputActionProperty _muteInputAction;
		[SerializeField] private InputActionProperty _cameraRotationInputAction;
		[SerializeField] private InputActionProperty _openMenuInputAction;

		public event Action<Vector2> OnAxis;
		public event Action OnMute;
		public event Action<Vector2> OnCameraRotation;
		public event Action OnOpenMenu;

		// ===============================================================

		private void OnEnable()
		{
			AddCallbacks();
		}

		private void OnDisable()
		{
			RemoveCallbacks();
		}

		// ===============================================================

		private void AddCallbacks()
		{
			_axisInputAction.action.performed += OnAxisPerformed;
			_axisInputAction.action.canceled += OnAxisCanceled;
			_muteInputAction.action.performed += OnMutePerformed;
			if (_cameraRotationInputAction.reference)
				_cameraRotationInputAction.action.performed += OnCameraRotationPerformed;
			_openMenuInputAction.action.performed += OnOpenMenuPerformed;
		}

		private void RemoveCallbacks()
		{
			_axisInputAction.action.performed -= OnAxisPerformed;
			_axisInputAction.action.canceled -= OnAxisCanceled;
			_muteInputAction.action.performed -= OnMutePerformed;
			if (_cameraRotationInputAction.reference)
				_cameraRotationInputAction.action.performed -= OnCameraRotationPerformed;
			_openMenuInputAction.action.performed -= OnOpenMenuPerformed;
		}

		// ===============================================================

		private void OnAxisPerformed(InputAction.CallbackContext context)
		{
			if (_inputManager.Value.IsLocked)
				return;

			OnAxis?.Invoke(context.ReadValue<Vector2>());
		}

		private void OnAxisCanceled(InputAction.CallbackContext context)
		{
			OnAxis?.Invoke(Vector2.zero);
		}

		private void OnMutePerformed(InputAction.CallbackContext context)
		{
			if (_inputManager.Value.IsLocked)
				return;

			OnMute?.Invoke();
		}

		private void OnCameraRotationPerformed(InputAction.CallbackContext context)
		{
			if (_inputManager.Value.IsLocked)
				return;

			OnCameraRotation?.Invoke(context.ReadValue<Vector2>());
		}

		private void OnOpenMenuPerformed(InputAction.CallbackContext context)
		{
			OnOpenMenu?.Invoke();
		}

		// ===============================================================
	}
}
