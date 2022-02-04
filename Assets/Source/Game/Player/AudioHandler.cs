using Photon.Pun;
using System;
using Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AudioChat
{
	public interface ISpeaker
	{
		event Action OnPlayingStarted;
		event Action OnPlayingEnded;
		event Action<bool> OnMute;
	}

	public class AudioHandler : MonoBehaviour, IAudioDataCallback, ISpeaker, IPunInstantiateMagicCallback
	{
		DIVar<INetworkManager> _networkManager = new DIVar<INetworkManager>();
		DIVar<IAudioManager> _audioManager = new DIVar<IAudioManager>();

		[SerializeField] private SamplingRate _samplingRate = SamplingRate.Frequency_24000;
		[Range(OpusInfo.MIN_OPUS_BITRATE, OpusInfo.MAX_OPUS_BITRATE)]
		[SerializeField] private int _bitrate = 20000;
		[SerializeField] private FrameDuration _frameDuration = FrameDuration.Frame20ms;
		[SerializeField] private NumChannels _channels = NumChannels.Mono;
		[Range(0f, 1f)]
		[SerializeField] private float _voiceDetectionThreshold = 0.1f;

		[SerializeField] private PlayerInput _input;
		[SerializeField] private PhotonView _photonView;
		[SerializeField] private MicrophoneRecorder _recorder;
		[SerializeField] private AudioSource _audioSource;

		private VoiceInfo _info;
		private AudioProcessor _audioProcessor;
		private AudioCompressor _audioCompressor;
		private VoiceDetector _voiceDetector;
		private bool _isMuted = false;

		// ===============================================================

		int IAudioDataCallback.Id => _photonView.Owner.ActorNumber;

		// ===============================================================

		private Action _onPlayingStarted;
		private Action _onPlayingEnded;
		private Action<bool> _onMute;

		event Action ISpeaker.OnPlayingStarted
		{
			add
			{
				_onPlayingStarted += value;
			}

			remove
			{
				_onPlayingStarted -= value;
			}
		}

		event Action ISpeaker.OnPlayingEnded
		{
			add
			{
				_onPlayingEnded += value;
			}

			remove
			{
				_onPlayingEnded -= value;
			}
		}

		event Action<bool> ISpeaker.OnMute
		{
			add
			{
				_onMute += value;
			}

			remove
			{
				_onMute -= value;
			}
		}

		// ===============================================================

		void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
		{
			if (info.photonView.Owner.CustomProperties.ContainsKey("_isMuted"))
			{
				_isMuted = (bool)info.photonView.Owner.CustomProperties["_isMuted"];
				_onMute?.Invoke(_isMuted);
			}

			if (!info.photonView.IsMine)
			{
				_audioProcessor.Start();
			}
		}

		// ===============================================================

		private void Awake()
		{
			_info = new VoiceInfo() { SamplingRate = _samplingRate, FrameDuration = _frameDuration, NumChannels = _channels, Bitrate = _bitrate };
			_audioProcessor = new AudioProcessor(_audioSource, _info);
			_audioCompressor = new AudioCompressor(_info);
			_voiceDetector = new VoiceDetector((int)_samplingRate, (int)_channels) { Threshold = _voiceDetectionThreshold };
		}

		private void OnEnable()
		{
			AddCallbacks();
			StartRecording();
		}

		private void OnDisable()
		{
			RemoveCallbacks();
			StopRecording();
		}

		private void Update()
		{
			if (!_photonView.IsMine)
			{
				_audioProcessor.Update();
			}
		}

		// ===============================================================

		private void AddCallbacks()
		{
			_recorder.OnSample += OnSampleReady;
			_voiceDetector.OnDetectedEnd += OnDetectedEnd;
			if (_input)
				_input.OnMute += OnMute;
			_networkManager.Value.OnPropertiesUpdate += OnPropertiesUpdate;
			_audioManager.Value?.AddDataCallback(this);
		}

		private void RemoveCallbacks()
		{
			_recorder.OnSample -= OnSampleReady;
			_voiceDetector.OnDetectedEnd -= OnDetectedEnd;
			if (_input)
				_input.OnMute -= OnMute;
			_audioManager.Value?.RemoveDataCallback(this);
		}

		private void StartRecording()
		{
			if (_photonView.IsMine)
			{
				_recorder.StartRecording(_info);
			}
		}

		private void StopRecording()
		{
			if (_photonView.IsMine)
			{
				_recorder.StopRecording();
			}
		}

		// ===============================================================

		private void OnDetectedEnd()
		{
			if (_photonView.IsMine)
			{
				_audioManager.Value.SendFlush();
			}
		}

		private void OnSampleReady(float[] data)
		{
			if (_isMuted)
			{
				return;//maybe show message
			}

			if (_voiceDetector.Process(data))
			{
				ArraySegment<byte> compressedData = _audioCompressor.Compress(data);
				_audioManager.Value.SendData(compressedData);
			}
		}

		private void OnMute()
		{
			if (_photonView.IsMine)
			{
				_isMuted = !_isMuted;
				_networkManager.Value.SetPlayerProperty("_isMuted", _isMuted);
				_audioManager.Value.SendFlush();
			}
		}

		private void OnPropertiesUpdate(Photon.Realtime.Player player, ExitGames.Client.Photon.Hashtable properties)
		{
			if (player == _photonView.Owner)
			{
				_isMuted = (bool)properties["_isMuted"];
				_onMute?.Invoke(_isMuted);
			}
		}

		// ===============================================================

		void IAudioDataCallback.ReceiveData(byte[] data)
		{
			var decompressed = _audioCompressor.Decompress(data);
			_audioProcessor.Push(decompressed);
			if (_audioProcessor.IsFlushed)
				_onPlayingStarted?.Invoke();
		}

		void IAudioDataCallback.ReceiveFlush()
		{
			_audioProcessor.Flush();
			_onPlayingEnded?.Invoke();
		}
	}
}
