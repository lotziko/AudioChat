using TMPro;
using UnityEngine;

namespace AudioChat
{
	public class CreateRoomMenuPanel : MonoBehaviour
	{
		public enum Result
		{
			CREATE, CANCEL
		}

		public delegate void Callback(Result result, string roomName, int usersCount);

		// =============================================================

		public static void Create(MenuContainer container, Callback callback)
		{
			GameObject prefab = Resources.Load("CreateRoomMenuPanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			CreateRoomMenuPanel me = go.GetComponent<CreateRoomMenuPanel>();
			go.GetComponent<MenuPanel>().Open(container, new DefferedMethod<Callback>(me.Init, callback));
		}

		// =============================================================

		[SerializeField] private TMP_InputField _roomNameField;
		[SerializeField] private TMP_InputField _userCountField;

		private Callback _callback;

		// =============================================================

		private void Init(Callback callback)
		{
			_callback = callback;
		}

		public void OnCreateClick()
		{
			GetComponent<MenuPanel>().Close();
			_callback?.Invoke(Result.CREATE, _roomNameField.text, int.Parse(_userCountField.text));
		}

		public void OnCancelClick()
		{
			GetComponent<MenuPanel>().Close();
			_callback?.Invoke(Result.CANCEL, null, 0);
		}
	}
}
