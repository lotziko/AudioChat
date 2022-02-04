using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Tools
{
	public class VRKeyboard : MonoBehaviour
	{
		private XRRig _rig;
		private TMP_InputField _targetField;

		// =============================================================

		public void Open(TMP_InputField target)
		{
			if (!gameObject.activeInHierarchy)
				gameObject.SetActive(true);

			transform.SetParent(_rig.transform);

			_targetField = target;
		}

		public void Close()
		{
			if (gameObject.activeInHierarchy)
				gameObject.SetActive(false);
		}

		// =============================================================

		private void Awake()
		{
			_rig = FindObjectOfType<XRRig>();
			Debug.Assert(_rig, "Rig not found.");
		}

		private void Start()
		{
			foreach (VRKeyboardKey key in GetComponentsInChildren<VRKeyboardKey>())
			{
				key.SetCallback(PressCallback);
			}
		}

		// =============================================================

		private void PressCallback(string key)
		{
			if (_targetField != null)
			{
				_targetField.ProcessEvent(Event.KeyboardEvent(key));
				_targetField.ForceLabelUpdate();
			}
		}
	}
}
