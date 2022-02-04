using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

namespace AudioChat
{
	public class RoomMenuItem : MonoBehaviour
	{
		[SerializeField] private TMP_Text _name;
		[SerializeField] private TMP_Text _playersInfo;

		private Action<RoomMenuItem> _callback;

		public void Initialize(RoomInfo data, Action<RoomMenuItem> callback)
		{
			_name.text = data.Name;
			_playersInfo.text = data.PlayerCount + "/" + data.MaxPlayers;
			_callback = callback;
		}

		public void OnClick()
		{
			_callback.Invoke(this);
		}
	}
}
