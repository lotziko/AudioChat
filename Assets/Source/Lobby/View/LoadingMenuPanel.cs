using UnityEngine;

namespace AudioChat
{
	public class LoadingMenuPanel : MonoBehaviour
	{
		public static MenuPanel Create(MenuContainer container)
		{
			GameObject prefab = Resources.Load("LoadingMenuPanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			LoadingMenuPanel me = go.GetComponent<LoadingMenuPanel>();
			MenuPanel menuPanel = go.GetComponent<MenuPanel>();
			menuPanel.Open(container, new DefferedMethod(me.Init));
			return menuPanel;
		}

		// =============================================================

		private void Init()
		{

		}
	}
}
