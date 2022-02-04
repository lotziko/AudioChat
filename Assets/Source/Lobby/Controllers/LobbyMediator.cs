using Tools;
using UnityEngine;

namespace AudioChat
{
	public class LobbyMediator : MonoBehaviour
	{
		DIVar<IProfileManager> _profileManager = new DIVar<IProfileManager>();
		DIVar<INetworkManager> _networkManager = new DIVar<INetworkManager>();

		[SerializeField] private MenuContainer _menu;

		private void Start()
		{
			OpenLoginMenu();
		}

		private void OpenLoginMenu()
		{
			LoginMenuPanel.Create(_menu, _profileManager.Value.GetNickname(),
				(string newName) =>
				{
					_profileManager.Value.SetNickname(newName);
					_menu.ShowLoading();
					_networkManager.Value.Connect(newName,
						() =>
						{
							_menu.HideLoading();
							OpenMainMenu();
						});
				});
		}

		private void OpenMainMenu()
		{
			MainMenuPanel.Create(_menu,
				(MainMenuPanel.Result mainMenuResult) =>
				{
					switch (mainMenuResult)
					{
						case MainMenuPanel.Result.START:
							OpenRoomMenu();
							break;
						case MainMenuPanel.Result.SETTINGS:
							OpenSettingsMenu();
							break;
					}
				});
		}

		private void OpenRoomMenu()
		{
			_menu.ShowLoading();
			_networkManager.Value.ConnectToLobby(() =>
			{
				_menu.HideLoading();
				RoomMenuPanel.Create(_menu, _networkManager.Value,
				(RoomMenuPanel.Result roomMenuResult, string roomName) =>
				{
					switch (roomMenuResult)
					{
						case RoomMenuPanel.Result.CREATE:
							OpenCreateRoomMenu();
							break;
						case RoomMenuPanel.Result.JOIN:
							_menu.ShowLoading();
							_networkManager.Value.ConnectToRoom(roomName, 
								() =>
								{
									//_menu.HideLoading();
									_networkManager.Value.EnterGame();
								});
							break;
					}
				});
			});
		}

		private void OpenSettingsMenu()
		{
			SettingsMenuPanel.Create(_menu);
		}

		private void OpenCreateRoomMenu()
		{
			CreateRoomMenuPanel.Create(_menu,
				(CreateRoomMenuPanel.Result result, string roomName, int userCount) =>
				{
					switch (result)
					{
						case CreateRoomMenuPanel.Result.CREATE:
							_menu.ShowLoading();
							_networkManager.Value.CreateRoom(roomName, userCount,
								() =>
								{
									//_menu.HideLoading();
									_networkManager.Value.EnterGame();
								});
							break;
					}
				});
		}
	}
}
