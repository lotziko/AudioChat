using TMPro;
using UnityEngine;

namespace AudioChat
{
	public class LoginMenuPanel : MonoBehaviour
	{
		public delegate void OnLogin(string username);

		// =============================================================

		public static void Create(MenuContainer container, string previousName, OnLogin callback)
		{
			GameObject prefab = Resources.Load("LoginMenuPanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			LoginMenuPanel me = go.GetComponent<LoginMenuPanel>();
			go.GetComponent<MenuPanel>().Open(container, new DefferedMethod<string, OnLogin>(me.Init, previousName, callback));
		}

		// =============================================================

		[SerializeField] private TMP_InputField _inputField;

		private OnLogin _callback;

		// =============================================================

		private void Init(string previousName, OnLogin callback)
		{
			_inputField.text = previousName;
			_callback = callback;
		}

		public void OnLoginClick()
		{
			GetComponent<MenuPanel>().Close();
			_callback?.Invoke(_inputField.text);
		}
	}
}
