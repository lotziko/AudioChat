using System;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public class GameMediator : MonoBehaviour
	{
		DIVar<IInputManager> _inputManager = new DIVar<IInputManager>();
		DIVar<INetworkManager> _networkManager = new DIVar<INetworkManager>();

		[SerializeField] private PlayerInput _input;
		[SerializeField] private MenuContainer _menu;
		
		private IEscapeMenuPanel _escapeMenu;

		// ===============================================================

		private void Start()
		{
			MessagePanel.Create(_menu, _networkManager.Value);
		}

		private void OnEnable()
		{
			_input.OnOpenMenu += OnOpenMenu;
		}

		private void OnDisable()
		{
			_input.OnOpenMenu -= OnOpenMenu;
		}

		// ===============================================================

		private void OnOpenMenu()
		{
			if (_escapeMenu == null)
			{
				_inputManager.Value.Lock();
				_escapeMenu = EscapeMenuPanel.Create(_menu,	
					(EscapeMenuPanel.Result result) =>
					{
						switch (result)
						{
							case EscapeMenuPanel.Result.EXIT:
								_networkManager.Value.CloseGame();
								break;
							case EscapeMenuPanel.Result.CLOSE:
								_inputManager.Value.Unlock();
								break;
						}
						_escapeMenu = null;
					});
			}
			else
			{
				_inputManager.Value.Unlock();
				_escapeMenu.Close();
				_escapeMenu = null;
			}
		}
	}
}
