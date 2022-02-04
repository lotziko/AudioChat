using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AudioChat
{
	public class InputActionManager : MonoBehaviour
	{
		[SerializeField] private List<InputActionAsset> _actionAssets;

		// ===============================================================

		private void OnEnable()
		{
			if (_actionAssets == null)
				return;

			foreach (InputActionAsset asset in _actionAssets)
			{
				if (asset != null)
				{
					asset.Enable();
				}
			}
		}

		private void OnDisable()
		{
			if (_actionAssets == null)
				return;

			foreach (InputActionAsset asset in _actionAssets)
			{
				if (asset != null)
				{
					asset.Disable();
				}
			}
		}
	}
}
