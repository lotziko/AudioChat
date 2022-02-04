using UnityEngine;

namespace AudioChat
{
	public class GameContext : Tools.DIContext
	{
		[SerializeField] private NetworkManager _networkManager;
		[SerializeField] private AudioManager _audioManager;
		[SerializeField] private PlayerManager _playerManager;

		protected override void OnBind()
		{
			Bind<INetworkManager>(_networkManager);
			Bind<IAudioManager>(_audioManager);
			Bind<IPlayerManager>(_playerManager);
			Bind<IInputManager>(new InputManager());
		}
	}
}
