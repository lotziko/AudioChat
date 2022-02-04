using System.Collections.Generic;
using UnityEngine;

namespace AudioChat
{
	public class MenuContainer : MonoBehaviour
	{
		private Stack<MenuPanel> _panels = new Stack<MenuPanel>();
		private MenuPanel _loadingPanel;

		// =============================================================

		public void Push(MenuPanel panel)
		{
			if (_panels.Count > 0)
			{
				_panels.Peek().gameObject.SetActive(false);
			}

			_panels.Push(panel);
			panel.transform.SetParent(transform);
			panel.transform.localScale = Vector3.one;
			panel.transform.localPosition = Vector3.zero;
			panel.transform.localRotation = Quaternion.identity;
		}

		public void Pop(MenuPanel panel)
		{
			if (_panels.Peek() == panel)
			{
				_panels.Pop();
				Destroy(panel.gameObject);
				if (_panels.Count > 0)
				{
					_panels.Peek().gameObject.SetActive(true);
				}
			}
		}

		public void ShowLoading()
		{
			_loadingPanel = LoadingMenuPanel.Create(this);
		}

		public void HideLoading()
		{
			Pop(_loadingPanel);
		}
	}
}
