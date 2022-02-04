using Photon.Pun;
using RootMotion.FinalIK;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace AudioChat
{
	public class PlayerAvatar : MonoBehaviourPun, IPunInstantiateMagicCallback
	{
		[SerializeField] private Renderer _mesh;
		[SerializeField] private Animator _animator;
		[SerializeField] private VRIK _ik;
		[SerializeField] private IKTargets _ikTargets;
		[SerializeField] private AudioHandler _audioHandler;

		[SerializeField] private RuntimeAnimatorController _pcController;
		[SerializeField] private RuntimeAnimatorController _vrController;
		
		[Header("View")]
		[SerializeField] private TMP_Text _nickname;
		[SerializeField] private GameObject _speaker;

		public Animator Animator { get { return _animator; } }

		//private Serializer _serializer;

		// ===============================================================

		private void OnEnable()
		{
			AddCallbacks();
		}

		private void OnDisable()
		{
			RemoveCallbacks();
		}

		// ===============================================================

		//void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		//{
		//	//_serializer.Serialize(stream, info);
		//}

		void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
		{
			string nickname = (string)info.photonView.InstantiationData[0];
			AppType appType = (AppType)info.photonView.InstantiationData[1];

			Initialize(info.photonView.IsMine, appType);
			_nickname.text = nickname;
		}

		// ===============================================================

		private void OnAudioStarted()
		{
			_speaker.SetActive(true);
		}

		private void OnAudioEnded()
		{
			_speaker.SetActive(false);
		}

		// ===============================================================

		private void Initialize(bool isMine, AppType appType)
		{
			switch (appType)
			{
				case AppType.PC:
					_animator.runtimeAnimatorController = _pcController;
					//turn off ik
					Destroy(_ik);
					Destroy(_ikTargets.Parent);
					break;
				case AppType.VR:
					_animator.runtimeAnimatorController = _vrController;
					_ik.gameObject.SetActive(true);

					if (isMine)
					{
						_ikTargets.Head.GetComponent<ParentConstraint>().constraintActive = true;
						_ikTargets.LeftHand.GetComponent<ParentConstraint>().constraintActive = true;
						_ikTargets.RightHand.GetComponent<ParentConstraint>().constraintActive = true;
					}
					break;
			}

			//remove head on self
			//if (!isMine)
			//{
			//	Debug.Log(_mesh.sharedMaterials[1]);
			//	_mesh.materials[0] = _mesh.materials[1];
			//}
		}

		private void AddCallbacks()
		{
			ISpeaker speaker = _audioHandler;
			speaker.OnPlayingStarted += OnAudioStarted;
			speaker.OnPlayingEnded += OnAudioEnded;
		}

		private void RemoveCallbacks()
		{
			ISpeaker speaker = _audioHandler;
			speaker.OnPlayingStarted -= OnAudioStarted;
			speaker.OnPlayingEnded -= OnAudioEnded;
		}

		// ===============================================================

		//private abstract class Serializer
		//{
		//	public abstract void Serialize(PhotonStream stream, PhotonMessageInfo info);

		//	public class PCSerializer : Serializer
		//	{
		//		public PCSerializer(PlayerAvatar avatar)
		//		{
		//			avatar._ik.enabled = false;
		//			Destroy(avatar._ikTargets.Parent.gameObject);
		//		}

		//		public override void Serialize(PhotonStream stream, PhotonMessageInfo info)
		//		{
		//		}
		//	}

		//	public class VRSerializer : Serializer
		//	{
		//		private IKTargets _targets;

		//		public VRSerializer(PlayerAvatar avatar)
		//		{
		//			_targets = avatar._ikTargets;

		//			if (!avatar.photonView.IsMine)
		//			{
		//				Destroy(_targets.Head.GetComponent<ParentConstraint>());
		//				Destroy(_targets.LeftHand.GetComponent<ParentConstraint>());
		//				Destroy(_targets.RightHand.GetComponent<ParentConstraint>());
		//			}
		//		}

		//		public override void Serialize(PhotonStream stream, PhotonMessageInfo info)
		//		{
		//			//if (stream.IsWriting)
		//			//{
		//			//	stream.SendNext(_targets.Head.localPosition);
		//			//	stream.SendNext(_targets.Head.localRotation);
		//			//	stream.SendNext(_targets.LeftHand.localPosition);
		//			//	stream.SendNext(_targets.LeftHand.localRotation);
		//			//	stream.SendNext(_targets.RightHand.localPosition);
		//			//	stream.SendNext(_targets.RightHand.localRotation);
		//			//}
		//			//else
		//			//{
		//			//	_targets.Head.localPosition = (Vector3)stream.ReceiveNext();
		//			//	_targets.Head.localRotation = (Quaternion)stream.ReceiveNext();
		//			//	_targets.LeftHand.localPosition = (Vector3)stream.ReceiveNext();
		//			//	_targets.LeftHand.localRotation = (Quaternion)stream.ReceiveNext();
		//			//	_targets.RightHand.localPosition = (Vector3)stream.ReceiveNext();
		//			//	_targets.RightHand.localRotation = (Quaternion)stream.ReceiveNext();
		//			//}
		//		}

		//		private struct TransformInfo
		//		{
		//			public float Distance;
		//			public float Angle;

		//			public Vector3 Direction;
		//			public Vector3 NetworkPosition;
		//			public Vector3 StoredPosition;
		//		}
		//	}
		//}

		[System.Serializable]
		public class IKTargets
		{
			[SerializeField] private GameObject _parent;
			[SerializeField] private Transform _headTarget;
			[SerializeField] private Transform _leftHandTarget;
			[SerializeField] private Transform _rightHandTarget;

			public GameObject Parent { get { return _parent; } }
			public Transform Head { get { return _headTarget; } }
			public Transform LeftHand { get { return _leftHandTarget; } }
			public Transform RightHand { get { return _rightHandTarget; } }
		}
	}
}
