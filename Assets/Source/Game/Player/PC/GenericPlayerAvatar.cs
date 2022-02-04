using Photon.Pun;
using TMPro;
using UnityEngine;

namespace AudioChat
{
	public abstract class GenericPlayerAvatar : MonoBehaviour, IPunInstantiateMagicCallback
	{
		[SerializeField] protected PlayerController _controller;
		[SerializeField] protected AudioHandler _audioHandler;
		[SerializeField] protected Animator _animator;

		[SerializeField] protected SkinnedMeshRenderer _meshRenderer;
		[SerializeField] protected Material _invisibleHeadMaterial;

		[Header("UI")]
		[SerializeField] protected TMP_Text _nickname;
		[SerializeField] protected GameObject _speaker;
		[SerializeField] protected GameObject _mutedSpeaker;

		// ===============================================================

		protected void OnEnable()
		{
			AddCallbacks();
		}

		protected void OnDisable()
		{
			RemoveCallbacks();
		}

		// ===============================================================

		void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
		{
			string nickname = (string)info.photonView.InstantiationData[0];
			AppType appType = (AppType)info.photonView.InstantiationData[1];
			
			_nickname.text = nickname;
			Initialize(info.photonView.IsMine);
		}

		// ===============================================================
		
		protected void AddCallbacks()
		{
			ISpeaker speaker = _audioHandler;
			speaker.OnPlayingStarted += OnAudioStarted;
			speaker.OnPlayingEnded += OnAudioEnded;
			speaker.OnMute += OnMute;

			IPlayerController controller = _controller;
			controller.OnMovementSpeedUpdated += OnMovementSpeedUpdated;
		}

		protected void RemoveCallbacks()
		{
			ISpeaker speaker = _audioHandler;
			speaker.OnPlayingStarted -= OnAudioStarted;
			speaker.OnPlayingEnded -= OnAudioEnded;
			speaker.OnMute -= OnMute;

			IPlayerController controller = _controller;
			controller.OnMovementSpeedUpdated -= OnMovementSpeedUpdated;
		}

		// ===============================================================

		protected void OnAudioStarted()
		{
			_speaker.SetActive(true);
		}

		protected void OnAudioEnded()
		{
			_speaker.SetActive(false);
		}

		protected void OnMute(bool isMuted)
		{
			_mutedSpeaker.SetActive(isMuted);
		}

		private void OnMovementSpeedUpdated(Vector2 value)
		{
			_animator.SetFloat("VelocityX", value.x);
			_animator.SetFloat("VelocityZ", value.y);
		}

		// ===============================================================

		protected virtual void Initialize(bool isMine)
		{
			//disable head
			if (isMine)
			{
				_meshRenderer.materials = new Material[] { _invisibleHeadMaterial, _meshRenderer.sharedMaterial };
			}
		}
	}
}
