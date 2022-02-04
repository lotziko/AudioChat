using System;
using UnityEngine;

namespace AudioChat
{
	public interface IEscapeMenuPanel
	{
		void Close();
	}

	public class EscapeMenuPanel : MonoBehaviour, IEscapeMenuPanel
	{
		public enum Result
		{
			EXIT, CLOSE
		}

		// =============================================================

		public static IEscapeMenuPanel Create(MenuContainer container, Action<Result> callback)
		{
			GameObject prefab = Resources.Load("EscapeMenuPanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			EscapeMenuPanel me = go.GetComponent<EscapeMenuPanel>();
			go.GetComponent<MenuPanel>().Open(container, new DefferedMethod<Action<Result>>(me.Init, callback));
			return me;
		}

		// =============================================================

		private Action<Result> _callback;

		// =============================================================

		private void Init(Action<Result> callback)
		{
			_callback = callback;
		}

		// =============================================================

		void IEscapeMenuPanel.Close()
		{
			GetComponent<MenuPanel>()?.Close();
		}

		// =============================================================

		public void OnExitClick()
		{
			GetComponent<MenuPanel>().Close();
			_callback?.Invoke(Result.EXIT);
		}

		public void OnCloseClick()
		{
			GetComponent<MenuPanel>().Close();
			_callback?.Invoke(Result.CLOSE);
		}
	}
}
