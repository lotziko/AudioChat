using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AudioChat
{
	public interface INetworkManager
	{
		event Action<Player> OnPlayerJoined;
		event Action<Player> OnPlayerLeft;
		event Action<Player, Hashtable> OnPropertiesUpdate;
		event Action OnRoomUpdate;
		List<RoomInfo> Rooms { get; }
		bool IsMaster { get; }

		void Connect(string username, Action callback = null);
		void ConnectToLobby(Action callback = null);
		void CreateRoom(string name, int userCount, Action callback = null);
		void ConnectToRoom(string name, Action callback = null);
		void SetPlayerProperty(string name, object value);
		void EnterGame();
		void CloseGame();
	}

	public class NetworkManager : MonoBehaviourPunCallbacks, INetworkManager
	{
		private List<RoomInfo> _rooms = new List<RoomInfo>();

		private Action _onRoomUpdate;
		private Action<Player> _onPlayerJoined;
		private Action<Player> _onPlayerLeft;
		private Action<Player, Hashtable> _onProperitesUpdate;

		private Action _masterConnectCallback;
		private Action _lobbyJoinCallback;
		private Action _roomCreateCallback;
		private Action _roomJoinCallback;
		
		public event Action OnRoomUpdate
		{
			add	{ _onRoomUpdate += value; }
			remove { _onRoomUpdate -= value; }
		}
		public event Action<Player> OnPlayerJoined
		{
			add { _onPlayerJoined += value; }
			remove { _onPlayerJoined -= value; }
		}
		public event Action<Player> OnPlayerLeft
		{
			add { _onPlayerLeft += value; }
			remove { _onPlayerLeft -= value; }
		}
		public event Action<Player, Hashtable> OnPropertiesUpdate
		{
			add { _onProperitesUpdate += value; }
			remove { _onProperitesUpdate -= value; }
		}
		List<RoomInfo> INetworkManager.Rooms => _rooms;

		bool INetworkManager.IsMaster
		{ get { return PhotonNetwork.IsMasterClient; } }

		// =============================================================

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void EnableByteArraySlicePooling()
		{
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.UseByteArraySlicePoolForEvents = true;
			PhotonNetwork.UseRpcMonoBehaviourCache = true;

			PhotonPeer.RegisterType(typeof(WhiteboardLine), (byte)'L', WhiteboardLine.Serialize, WhiteboardLine.Deserialize);
		}

		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			base.OnRoomListUpdate(roomList);
			_rooms.Clear();
			_rooms.AddRange(roomList);
			_onRoomUpdate?.Invoke();
		}

		public override void OnConnectedToMaster()
		{
			base.OnConnectedToMaster();
			_masterConnectCallback?.Invoke();
			_masterConnectCallback = null;
		}

		public override void OnJoinedLobby()
		{
			base.OnJoinedLobby();
			_lobbyJoinCallback?.Invoke();
			_lobbyJoinCallback = null;
		}

		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			_roomCreateCallback?.Invoke();
			_roomCreateCallback = null;
			_roomJoinCallback?.Invoke();
			_roomJoinCallback = null;
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			base.OnPlayerEnteredRoom(newPlayer);
			_onPlayerJoined?.Invoke(newPlayer);
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			_onPlayerLeft?.Invoke(otherPlayer);
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
			_onProperitesUpdate?.Invoke(targetPlayer, changedProps);
		}

		// =============================================================

		void INetworkManager.Connect(string username, Action callback)
		{
			PhotonNetwork.LocalPlayer.NickName = username;
			PhotonNetwork.ConnectUsingSettings();
			_masterConnectCallback = callback;
		}

		void INetworkManager.ConnectToLobby(Action callback)
		{
			if (!PhotonNetwork.InLobby)
			{
				PhotonNetwork.JoinLobby();
				_lobbyJoinCallback = callback;
			}
			else
			{
				callback?.Invoke();
			}
		}

		void INetworkManager.CreateRoom(string name, int userCount, Action callback)
		{
			PhotonNetwork.CreateRoom(name, new RoomOptions() { MaxPlayers = (byte)userCount, PlayerTtl = 0 });
			_roomCreateCallback = callback;
		}

		void INetworkManager.ConnectToRoom(string name, Action callback)
		{
			PhotonNetwork.JoinRoom(name);
			_roomJoinCallback = callback;
		}

		void INetworkManager.SetPlayerProperty(string name, object value)
		{
			Hashtable properties = new Hashtable();
			properties.Add(name, value);
			PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
		}

		void INetworkManager.EnterGame()
		{
			if (PhotonNetwork.CurrentRoom != null)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					PhotonNetwork.CurrentRoom.IsOpen = true;
					PhotonNetwork.CurrentRoom.IsVisible = true;
				}
				
				PhotonNetwork.LoadLevel("GameScene");
			}
		}

		void INetworkManager.CloseGame()
		{
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene("LoadingScene");
		}
	}
}
