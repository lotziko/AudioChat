#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AudioChat
{
	public class AnimationTest : MonoBehaviour
	{
		[SerializeField] private AnimationClip _clip;
		[SerializeField] private Animator _animator;

		private void Start()
		{
			_animator.SetFloat("VelocityZ", 2);
		}

		[ContextMenu("Rename animation")]
		private void RenameAnimation()
		{
			var bindings = AnimationUtility.GetCurveBindings(_clip);
			for (int i = 0; i < bindings.Length; i++)
			{
				var curve = AnimationUtility.GetEditorCurve(_clip, bindings[i]);
				AnimationUtility.SetEditorCurve(_clip, bindings[i], null);
				bindings[i].path = bindings[i].path.Replace("mixamorig:", "");
				AnimationUtility.SetEditorCurve(_clip, bindings[i], curve);
			}
			EditorUtility.SetDirty(_clip);
			AssetDatabase.SaveAssets();
		}
		[ContextMenu("Rename rig")]
		private void RenameRig()
		{
			foreach (Transform transform in GetComponentsInChildren<Transform>())
			{
				transform.gameObject.name = transform.gameObject.name.Replace("mixamorig:", "");
				EditorUtility.SetDirty(transform.gameObject);
			}
			AssetDatabase.SaveAssets();
		}
	}
}
#endif