#if UNITY_EDITOR
using AudioChat.Pose;
using UnityEditor;
using UnityEngine;

namespace AudioChat.Editor
{
	[ExecuteInEditMode]
	public class HandManager : MonoBehaviour
	{
		[SerializeField]
		GameObject _leftHandPrefab = null;
		[SerializeField]
		GameObject _rightHandPrefab = null;

		private PreviewHand _leftPreview;
		private PreviewHand _rightPreview;
		private HandPose _lastPose;

		public bool HandsExist => _leftPreview && _rightPreview;

		public PreviewHand LeftPreview
		{ get { return _leftPreview; } }

		public PreviewHand RightPreview
		{ get { return _rightPreview; } }

		// ===============================================================

		private void OnEnable()
		{
			CreateHands();
		}

		private void OnDisable()
		{
			DestroyHands();
		}

		// ===============================================================

		public void AssignHandsToParent(Transform parent, HandPose pose)
		{
			_leftPreview.transform.parent = parent;
			_rightPreview.transform.parent = parent;

			_leftPreview.ApplyPose(pose);
			_rightPreview.ApplyPose(pose);
			_lastPose = pose;
		}

		public void SavePose()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(_lastPose);
#endif
			_lastPose.LeftHandInfo.Save(_leftPreview);
			_lastPose.RightHandInfo.Save(_rightPreview);
		}

		// ===============================================================

		private void CreateHands()
		{
			if (!_leftPreview)
			{
				_leftPreview = CreateHand(_leftHandPrefab);
				_rightPreview = CreateHand(_rightHandPrefab);
			}
		}

		private void DestroyHands()
		{
#if UNITY_EDITOR
			if (_leftPreview)
				DestroyImmediate(_leftPreview.gameObject);
			if (_rightPreview)
				DestroyImmediate(_rightPreview.gameObject);
#endif
		}

		private PreviewHand CreateHand(GameObject prefab)
		{
			GameObject hand = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);

			hand.hideFlags = HideFlags.DontSave;
			hand.gameObject.SetActive(false);

			return hand.GetComponent<PreviewHand>();
		}
	}
}

#endif