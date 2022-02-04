using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
	public interface IVRKeyboardManager
	{
		void Open(TMP_InputField field);
		void Close();
	}

	public class VRKeyboardManager : MonoBehaviour, IVRKeyboardManager
	{
		[SerializeField] private VRKeyboard _keyboard;

		// =============================================================

		private void Awake()
		{
			Debug.Assert(_keyboard, "No keyboard.");
		}

		// =============================================================

		void IVRKeyboardManager.Open(TMP_InputField field)
		{
			_keyboard.Open(field);
		}

		void IVRKeyboardManager.Close()
		{
			_keyboard.Close();
		}
	}
}
