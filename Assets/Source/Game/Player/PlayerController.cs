using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace AudioChat
{
	public interface IPlayerController
	{
		event Action<Vector2> OnMovementSpeedUpdated;
	}

	public class PlayerController : MonoBehaviour, IPlayerController, IPunInstantiateMagicCallback
	{
		[SerializeField] private PhotonView _photonView;
		[SerializeField] private CharacterController _characterController;

		[SerializeField] private AppTypeInfo[] _infos;
		[SerializeField] private GameObject[] _localObjects;
		[SerializeField] private GameObject[] _notLocalObjects;

		[SerializeField] private float _movementSpeed = 1.75f;
		[SerializeField] private float _turnSpeed = 25f;

		private Transform _cameraTransform;
		private Vector2 _currentAxis;
		private Vector2 _currentLerpedAxis;

		private Action<Vector2> _onMovementSpeedUpdated;

		event Action<Vector2> IPlayerController.OnMovementSpeedUpdated
		{ add { _onMovementSpeedUpdated += value; }	remove { _onMovementSpeedUpdated -= value; } }

		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			AppType appType = (AppType)info.photonView.InstantiationData[1];

			Initialize();
			foreach (AppTypeInfo appTypeInfo in _infos)
				Initialize(appTypeInfo, appType);

			if (info.photonView.IsMine)
			{
				StartCoroutine(MovementCoroutine());
			}

			foreach (IPunInstantiateMagicCallback callback in GetComponentsInChildren<IPunInstantiateMagicCallback>())
			{
				if (callback == this as IPunInstantiateMagicCallback)
					continue;
				callback.OnPhotonInstantiate(info);
			}
		}

		public void MoveCharacter(Vector2 normalizedAxis)
		{
			_currentAxis = normalizedAxis;
		}

		//public void SetAvatarLocalPosition(Vector3 position)
		//{
		//	//position.y = _avatar.transform.localPosition.y;
		//	//_avatar.transform.localPosition = position;
		//}

		//public void SetAvatarLocalRotation(Vector3 forward)
		//{
		//	forward.y = 0f;
		//	forward.Normalize();
		//	//_avatar.transform.forward = forward;
		//}

		public void TurnCharacter(Vector3 axis)
		{
			transform.Rotate(axis * _turnSpeed);
		}

		public void MoveCharacterCapsuleToPosition(Vector3 worldPosition)
		{

		}

		private IEnumerator MovementCoroutine()
		{
			WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
			yield return waitForFixedUpdate;
			_cameraTransform = GetComponentInChildren<Camera>().transform;
			while (true)
			{
				_currentLerpedAxis = Vector2.MoveTowards(_currentLerpedAxis, _currentAxis, 12f * Time.fixedDeltaTime);

				Vector3 direction = _cameraTransform.forward * _currentLerpedAxis.y + _cameraTransform.right * _currentLerpedAxis.x;
				direction.y = 0f;
				direction.Normalize();
				Vector3 axis = direction * _currentLerpedAxis.magnitude;
				
				_characterController.Move(axis * _movementSpeed * Time.fixedDeltaTime);
				_characterController.Move(Physics.gravity);
				_onMovementSpeedUpdated?.Invoke(_currentLerpedAxis * _movementSpeed);
				yield return waitForFixedUpdate;
			}
		}

		private void Initialize()
		{
			foreach (GameObject obj in _localObjects)
			{
				if (_photonView.IsMine)
				{
					obj.SetActive(true);
				}
				else
				{
					Destroy(obj);
				}
			}

			foreach (GameObject obj in _notLocalObjects)
			{
				if (_photonView.IsMine)
				{
					Destroy(obj);
				}
				else
				{
					obj.SetActive(true);
				}
			}
		}

		private void Initialize(AppTypeInfo info, AppType globalType)
		{
			if (globalType == info.Type)
			{
				_cameraTransform = info.Camera.transform;
			}
			else
			{
				Destroy(info.Player);
			}
		}

		[System.Serializable]
		private class AppTypeInfo
		{
			[SerializeField] private AppType _appType;
			[SerializeField] private GameObject _player;
			[SerializeField] private Camera _camera;

			public AppType Type
			{ get { return _appType; } }

			public GameObject Player
			{ get { return _player; } }

			public Camera Camera
			{ get { return _camera; } }
		}
	}
}
