using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioChat
{
	public class RoomMenuPanel : MonoBehaviour
	{
		public enum Result
		{
			CREATE, JOIN, BACK
		}

		public delegate void OnResult(Result result, string roomName);

		// =============================================================

		public static void Create(MenuContainer container, INetworkManager roomManager, OnResult callback)
		{
			GameObject prefab = Resources.Load("RoomMenuPanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			RoomMenuPanel me = go.GetComponent<RoomMenuPanel>();
			go.GetComponent<MenuPanel>().Open(container, new DefferedMethod<INetworkManager, OnResult>(me.Init, roomManager, callback));
		}

		// =============================================================

		[SerializeField] Transform _itemContainer;
		[SerializeField] RoomMenuItem _itemPrefab;

		private Dictionary<RoomMenuItem, RoomInfo> _itemCache = new Dictionary<RoomMenuItem, RoomInfo>();
		private INetworkManager _roomManager;
		private OnResult _callback;

		// =============================================================

		private void OnEnable()
		{
			if (_roomManager != null)
			{
				_roomManager.OnRoomUpdate += OnListRefresh;
			}
		}

		private void OnDisable()
		{
			if (_roomManager != null)
			{
				_roomManager.OnRoomUpdate -= OnListRefresh;
			}
		}

		// =============================================================

		private void Init(INetworkManager roomManager, OnResult callback)
		{
			_roomManager = roomManager;
			_callback = callback;
			OnEnable();
			OnListRefresh();
		}

		private void FillList(List<RoomInfo> rooms)
		{
			if (_itemCache.Count > 0)
			{
				foreach (RoomMenuItem item in _itemCache.Keys)
				{
					item.gameObject.SetActive(false);
				}
			}

			for (int i = 0, count = rooms.Count - _itemCache.Count; i < count; i++)
			{
				RoomMenuItem item = Instantiate(_itemPrefab);
				item.transform.SetParent(_itemContainer);
				item.transform.localPosition = Vector3.zero;
				item.transform.localScale = Vector3.one;
				_itemCache.Add(item, null);
			}

			int roomIndex = 0;
			foreach (RoomMenuItem item in _itemCache.Keys.ToArray())
			{
				RoomInfo roomInfo = rooms[roomIndex];
				item.gameObject.SetActive(true);
				item.Initialize(roomInfo, OnRoomMenuItemClick);
				_itemCache[item] = roomInfo;
				++roomIndex;
			}
		}

		private void OnRoomMenuItemClick(RoomMenuItem item)
		{
			_callback?.Invoke(Result.JOIN, _itemCache[item].Name);
		}

		public void OnListRefresh()
		{
			FillList(_roomManager.Rooms);
		}

		public void OnCreateRoomClick()
		{
			_callback?.Invoke(Result.CREATE, null);
		}

		public void OnBackClick()
		{
			GetComponent<MenuPanel>().Close();
			_callback?.Invoke(Result.BACK, null);
		}
	}
}
