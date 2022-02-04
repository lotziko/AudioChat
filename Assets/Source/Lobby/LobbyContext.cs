using UnityEngine;

namespace AudioChat
{
	public class LobbyContext : Tools.DIContext
	{
		[SerializeField] private NetworkManager _roomManager;
		[SerializeField] private Tools.VRKeyboardManager _keyboardManager;

		protected override void OnBind()
		{
			Bind<INetworkManager>(_roomManager);
			Bind<Tools.IVRKeyboardManager>(_keyboardManager);
		}
	}
}
