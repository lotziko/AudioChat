using AudioChat.Pose;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AudioChat
{
	public class HandControls : MonoBehaviourPun
	{
		[SerializeField] private InputActionProperty _switchItemInputAction;
		[SerializeField] private GameObject[] _items;
		[SerializeField] private PosableHand _posableHand;

		private int _currentItemIndex;

		// =============================================================

		private void OnEnable()
		{
			AddCallbacks();
		}

		private void OnDisable()
		{
			RemoveCallbacks();	
		}

		//private void Start()
		//{
		//	OnSwitchItem(default);
		//	OnSwitchItem(default);
		//}

		// =============================================================

		private void AddCallbacks()
		{
			_switchItemInputAction.action.performed += OnSwitchItem;
		}

		private void RemoveCallbacks()
		{
			_switchItemInputAction.action.performed -= OnSwitchItem;
		}

		// =============================================================

		private void OnSwitchItem(InputAction.CallbackContext obj)
		{
			if (photonView.IsMine)
			{
				_currentItemIndex = (_currentItemIndex + 1) % _items.Length;
				photonView.RPC("SwitchItem", RpcTarget.AllBuffered, _currentItemIndex);
			}
		}

		[PunRPC]
		private void SwitchItem(int index)
		{
			_currentItemIndex = index;
			for (int i = 0; i < _items.Length; i++)
			{
				_items[i].SetActive(i == _currentItemIndex);
				if (i == _currentItemIndex)
				{
					if (_items[i].TryGetComponent(out PoseContainer container))
					{
						_posableHand?.Apply(container.Preset);
					}
				}
			}
		}
	}
}
