using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioChat
{
	public interface IAudioManager
	{
		void AddDataCallback(IAudioDataCallback callback);
		void RemoveDataCallback(IAudioDataCallback callback);
		void SendData(ArraySegment<byte> data);
		void SendFlush();
	}

	public interface IAudioDataCallback
	{
		int Id { get; }
		void ReceiveData(byte[] data);
		void ReceiveFlush();
	}
	
	public class AudioManager : MonoBehaviour, IOnEventCallback, IAudioManager
	{
		const byte VOICE_DATA_EVENT_ID = 190;
		const byte VOICE_END_EVENT_ID = 191;
		const int DATA_OFFSET = 0;

		[SerializeField] private ReceiverGroup _receiverGroup = ReceiverGroup.All;

		private Dictionary<int, IAudioDataCallback> _dataCallbacks = new Dictionary<int, IAudioDataCallback>();

		private void OnEnable()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		private void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		void IOnEventCallback.OnEvent(EventData photonEvent)
		{
			byte eventCode = photonEvent.Code;

			switch (eventCode)
			{
				case VOICE_DATA_EVENT_ID:
					ByteArraySlice content = photonEvent[(byte)ParameterCode.CustomEventContent] as ByteArraySlice;
					_dataCallbacks[photonEvent.Sender].ReceiveData(content.Buffer);
					break;
				case VOICE_END_EVENT_ID:
					_dataCallbacks[photonEvent.Sender].ReceiveFlush();
					break;
			}
		}

		void IAudioManager.AddDataCallback(IAudioDataCallback callback)
		{
			_dataCallbacks.Add(callback.Id, callback);
		}

		void IAudioManager.RemoveDataCallback(IAudioDataCallback callback)
		{
			_dataCallbacks.Remove(callback.Id);
		}

		void IAudioManager.SendData(ArraySegment<byte> data)
		{
			ByteArraySlice frameData = PhotonNetwork.NetworkingClient.LoadBalancingPeer.ByteArraySlicePool.Acquire(data.Count + DATA_OFFSET);
			Buffer.BlockCopy(data.Array, 0, frameData.Buffer, DATA_OFFSET, data.Count);
			frameData.Count = data.Count + DATA_OFFSET;

			RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = _receiverGroup };
			PhotonNetwork.RaiseEvent(VOICE_DATA_EVENT_ID, frameData, raiseEventOptions, SendOptions.SendReliable);
		}

		void IAudioManager.SendFlush()
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = _receiverGroup };
			PhotonNetwork.RaiseEvent(VOICE_END_EVENT_ID, new byte[0], raiseEventOptions, SendOptions.SendReliable);
		}
	}
}
