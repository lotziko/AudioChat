using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
#endif

namespace AudioChat
{
	[ExecuteInEditMode]
	public class FPSSetter : MonoBehaviour
	{
		[SerializeField] private bool _vSync = true;
		[SerializeField] private int _targetFramerate;

		private void Start()
		{
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
//#if UNITY_EDITOR
//			Assembly assembly = typeof(EditorWindow).Assembly;
//			Type type = assembly.GetType("UnityEditor.GameView");
//			EditorWindow window = EditorWindow.GetWindow(type);
//			type.GetProperty("vSyncEnabled").SetValue(window, _vSync);
//#endif
//			QualitySettings.vSyncCount = _vSync ? 1 : 0;
//			Application.targetFrameRate = _vSync ? -1 : _targetFramerate;
		}
	}
}
