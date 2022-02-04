using Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AudioChat
{
	public class PCPlayerControls : MonoBehaviour
	{
		DIVar<IInputManager> _inputManager = new DIVar<IInputManager>();

		[SerializeField] private GameMediator _gameMediator;
		[SerializeField] private PlayerInput _input;
		[SerializeField] private PlayerController _controller;
		[SerializeField] private Transform _cameraTransform;

		private Vector2 _lookDelta;
		private Vector2 _movementDelta;
		private float _xRotation;

		// =============================================================

		private void OnEnable()
		{
			EnableInput();
			AddCallbacks();
		}

		private void OnDisable()
		{
			DisableInput();
			RemoveCallbacks();
		}

		private void Update()
		{
			if (_lookDelta != Vector2.zero)
			{
				_xRotation = Mathf.Clamp(_xRotation - (_lookDelta.y * 8f * Time.deltaTime), -89f, 89f);

				_cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
				_controller.TurnCharacter(Vector3.up * _lookDelta.x * Time.deltaTime);
				_lookDelta = Vector2.zero;
			}
			_controller.MoveCharacter(_movementDelta);
		}
		
		// =============================================================

		private void AddCallbacks()
		{
			_input.OnCameraRotation += OnCameraRotation;
			_input.OnAxis += OnAxis;
			_inputManager.Value.OnUpdate += OnInputUpdate;
		}

		private void RemoveCallbacks()
		{
			_input.OnCameraRotation -= OnCameraRotation;
			_input.OnAxis -= OnAxis;
			_inputManager.Value.OnUpdate -= OnInputUpdate;
		}

		private void DisableInput()
		{
			Cursor.lockState = CursorLockMode.None;
		}

		private void EnableInput()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		// =============================================================

		private void OnCameraRotation(Vector2 value)
		{
			_lookDelta = value;
		}

		private void OnAxis(Vector2 value)
		{
			_movementDelta = value.normalized;
		}

		private void OnInputUpdate()
		{
			if (_inputManager.Value.IsLocked)
			{
				DisableInput();
			}
			else
			{
				EnableInput();
			}
		}
	}
}