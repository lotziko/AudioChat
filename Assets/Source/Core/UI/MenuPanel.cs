using UnityEngine;

namespace AudioChat
{
	public class MenuPanel : MonoBehaviour
	{
		private MenuContainer _container;

		public void Open(MenuContainer container, IDefferedMethod method)
		{
			_container = container;
			_container.Push(this);
			method.Call();
		}

		public void Close()
		{
			_container.Pop(this);
		}
	}
}
