using AudioChat.Pose;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AudioChat.Editor
{
    public class PoseEditorWindow : EditorWindow
    {
		private GameObject _poseHelper;
        private SerializedProperty _poseProperty;
        private HandPose _pose;

		private HandManager _handManager;

		private bool _showLeftHand;
		private bool _showRightHand;
		private MirrorAxis _mirrorAxis;

		// ===============================================================

		[MenuItem("Window/XR/Pose Editor")]
        public static void Open()
        {
            Open(null);
        }

        public static void Open(SerializedProperty poseProperty)
        {
            PoseEditorWindow window = (PoseEditorWindow)GetWindow(typeof(PoseEditorWindow), false, "Pose Editor");
            window._poseProperty = poseProperty;
            window._pose = poseProperty?.objectReferenceValue as HandPose;
            window.Show();
			window.TryAssignHands();
		}

		// ===============================================================

		private void OnEnable()
		{
			CreatePoseHelper();
			Selection.selectionChanged += SelectionChanged;
			EditorApplication.playModeStateChanged += CloseWindow;
			EditorSceneManager.sceneClosing += CloseWindow;
		}

        private void OnDisable()
        {
			DestroyPoseHelper();
			Selection.selectionChanged -= SelectionChanged;
			EditorApplication.playModeStateChanged -= CloseWindow;
			EditorSceneManager.sceneClosing -= CloseWindow;
		}

		private void OnGUI()
        {
            GUILayout.Label(_pose == null ? "No Pose" : _pose.name, EditorStyles.label);

            if (GUILayout.Button("Create Pose"))
                CreatePose();

			if (_handManager.HandsExist && _pose)
			{
				float halfWidth = EditorGUIUtility.currentViewWidth / 2f;

				PreviewHand leftHand = _handManager.LeftPreview;
				PreviewHand rightHand = _handManager.RightPreview;

				_mirrorAxis = (MirrorAxis)EditorGUILayout.EnumPopup("Mirror axis", _mirrorAxis);

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("Left hand", GUILayout.Width(halfWidth));
					GUILayout.Label("Right hand", GUILayout.Width(halfWidth));
				}

				using (new GUILayout.HorizontalScope())
				{
					if (GUILayout.Button(IsHandActive(leftHand) ? "Hide" : "Show", GUILayout.Width(halfWidth)))
						ToggleHand(leftHand);

					if (GUILayout.Button(IsHandActive(rightHand) ? "Hide" : "Show", GUILayout.Width(halfWidth)))
						ToggleHand(rightHand);
				}

				using (new GUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Mirror from R", GUILayout.Width(halfWidth)))
						MirrorHand(rightHand, leftHand, _mirrorAxis);

					if (GUILayout.Button("Mirror from L", GUILayout.Width(halfWidth)))
						MirrorHand(leftHand, rightHand, _mirrorAxis);
				}

				using (new GUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Reset", GUILayout.Width(halfWidth)))
						ResetHand(leftHand);

					if (GUILayout.Button("Reset", GUILayout.Width(halfWidth)))
						ResetHand(rightHand);
				}

				if (GUILayout.Button("Save"))
						SavePose();
			}
        }

		// ===============================================================

		private void CloseWindow(PlayModeStateChange stateChange)
		{
			if (stateChange == PlayModeStateChange.ExitingEditMode)
				Close();
		}

		private void CloseWindow(Scene scene, bool removingScene)
		{
			if (removingScene)
				Close();
		}

		private void SelectionChanged()
		{

		}

		private void CreatePoseHelper()
		{
			if (!_poseHelper)
			{
				Object prefab = Resources.Load("PoseEditorHelper");
				_poseHelper = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				_poseHelper.hideFlags = HideFlags.DontSave;

				_handManager = _poseHelper.GetComponent<HandManager>();
				TryAssignHands();
			}
		}

		private void DestroyPoseHelper()
		{
			DestroyImmediate(_poseHelper);
		}

        private void CreatePose()
        {
            _pose = CreateInstance<HandPose>();

            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewHandPose.asset");
            AssetDatabase.CreateAsset(_pose, path);
            AssetDatabase.SaveAssets();

            if (_poseProperty != null)
            {
                _poseProperty.objectReferenceValue = _pose;
                _poseProperty.serializedObject.ApplyModifiedProperties();
            }
			TryAssignHands();
			_handManager.SavePose();
		}

		private void SavePose()
		{
			_handManager.SavePose();
		}

		private bool TryAssignHands()
		{
			if (_pose && Selection.activeGameObject && Selection.activeGameObject.GetComponent<PoseContainer>())
			{
				_handManager.AssignHandsToParent(Selection.activeGameObject.transform, _pose);
				return true;
			}
			return false;
		}

		private bool IsHandActive(PreviewHand hand)
		{
			return hand.gameObject.activeSelf;
		}

		private void ToggleHand(PreviewHand hand)
		{
			Undo.RecordObject(hand.gameObject, "Hand toggled");
			hand.gameObject.SetActive(!hand.gameObject.activeSelf);
			SceneView.RepaintAll();
		}

		private void ResetHand(PreviewHand hand)
		{
			Undo.RecordObject(hand.gameObject, "Hand reset");
			hand.ApplyDefaultPose();
			SceneView.RepaintAll();
		}

		private void MirrorHand(PreviewHand fromHand, PreviewHand toHand, MirrorAxis axis)
		{
			Undo.RecordObject(toHand.gameObject, "Hand mirrored");
			toHand.MirrorToHand(fromHand, axis);
			SceneView.RepaintAll();
		}
    }
}
