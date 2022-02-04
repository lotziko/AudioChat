using UnityEngine;

namespace AudioChat
{
	public class MainMenuPanel : MonoBehaviour
	{
		public enum Result
		{
			START, SETTINGS
		}

		public delegate void OnResult(Result result);

		// =============================================================

		public static void Create(MenuContainer container, OnResult callback)
		{
			GameObject prefab = Resources.Load("MainMenuPanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			MainMenuPanel me = go.GetComponent<MainMenuPanel>();
			go.GetComponent<MenuPanel>().Open(container, new DefferedMethod<OnResult>(me.Init, callback));
		}

		// =============================================================

		private OnResult _callback;

		// =============================================================

		private void Init(OnResult callback)
		{
			_callback = callback;
		}

		public void OnStartClick()
		{
			_callback?.Invoke(Result.START);
		}

		public void OnSettingsClick()
		{
			_callback?.Invoke(Result.SETTINGS);
		}
	}
}
