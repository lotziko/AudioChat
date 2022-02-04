using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public interface IPlayerManager
	{
		event Action OnUpdate;

		GameObject LocalPlayer { get; }
	}

	public class PlayerManager : MonoBehaviour, IPlayerManager
	{
		DIVar<INetworkManager> _networkManager = new DIVar<INetworkManager>();
		DIVar<IXRManager> _xrManager = new DIVar<IXRManager>();

		[SerializeField] private GameObject _pcPlayerPrefab;
		[SerializeField] private GameObject _vrPlayerPrefab;

		private GameObject _localPlayer;

		private Action _onUpdate;

		// ===============================================================

		GameObject IPlayerManager.LocalPlayer => _localPlayer;

		event Action IPlayerManager.OnUpdate
		{
			add	{ _onUpdate += value; }
			remove { _onUpdate -= value; }
		}

		// ===============================================================

		private void Start()
		{
			if (_localPlayer == null)
			{
				AppType appType = _xrManager.Value.GetAppType();
				string prefabName = null;
				switch (appType)
				{
					case AppType.PC:
						prefabName = _pcPlayerPrefab.name;
						break;
					case AppType.VR:
						prefabName = _vrPlayerPrefab.name;
						break;
				}
				_localPlayer = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, 0, new object[] { PhotonNetwork.LocalPlayer.NickName, appType});
				_onUpdate?.Invoke();
			}
		}

		// ===============================================================
	}
}