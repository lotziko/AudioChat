using Photon.Realtime;
using UnityEngine;

namespace AudioChat
{
	public class MessagePanel : MonoBehaviour
	{
		public static void Create(MenuContainer container, INetworkManager networkManager)
		{
			GameObject prefab = Resources.Load("MessagePanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			MessagePanel me = go.GetComponent<MessagePanel>();
			go.GetComponent<MenuPanel>().Open(container, new DefferedMethod<INetworkManager>(me.Init, networkManager));
		}

		// =============================================================

		[SerializeField] Transform _itemContainer;
		[SerializeField] MessagePanelItem _itemPrefab;

		private INetworkManager _networkManager;

		// =============================================================

		private void OnEnable()
		{
			if (_networkManager != null)
			{
				_networkManager.OnPlayerJoined += OnPlayerJoined;
				_networkManager.OnPlayerLeft += OnPlayerLeft;
			}
		}

		private void OnDisable()
		{
			if (_networkManager != null)
			{
				_networkManager.OnPlayerJoined -= OnPlayerJoined;
				_networkManager.OnPlayerLeft -= OnPlayerLeft;
			}
		}

		private void OnDestroy()
		{
			OnDisable();
		}

		// =============================================================

		private void Init(INetworkManager networkManager)
		{
			_networkManager = networkManager;
			OnEnable();
		}

		// =============================================================

		private void OnPlayerJoined(Player player)
		{
			AddMessage(player.NickName + " joined.");
		}

		private void OnPlayerLeft(Player player)
		{
			AddMessage(player.NickName + " left.");
		}

		private void AddMessage(string text)
		{
			MessagePanelItem item = Instantiate(_itemPrefab);
			item.transform.SetParent(_itemContainer);
			item.transform.SetAsFirstSibling();
			item.transform.localPosition = Vector3.zero;
			item.transform.localScale = Vector3.one;
			item.transform.localRotation = Quaternion.identity;
			item.Initialize(text);
		}
	}
}
