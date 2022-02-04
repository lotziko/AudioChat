using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
	public class VRKeyboardKey : MonoBehaviour
	{
		[SerializeField] private string _key;
		[SerializeField] private Button _button;
#if UNITY_EDITOR
		[SerializeField] private bool _renameToObjectName = false;
#endif

		private Action<string> _callback;

		// =============================================================

		public void SetCallback(Action<string> callback)
		{
			_callback = callback;
		}

		// =============================================================

		private void Start()
		{
			_button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(OnClick);
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (_renameToObjectName)
			{
				_key = gameObject.name;
				gameObject.GetComponentInChildren<TMP_Text>().text = _key;
			}
		}
#endif

		// =============================================================

		private void OnClick()
		{
			_callback?.Invoke(_key);
		}
	}
}
