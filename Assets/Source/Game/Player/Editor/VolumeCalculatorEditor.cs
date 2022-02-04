using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using static AudioChat.VolumeCalculator;

namespace AudioChat.Editor
{
	[CustomEditor(typeof(VolumeCalculator))]
	public class VolumeCalculatorEditor : UnityEditor.Editor
	{
		private List<RayInfo> _rays = new List<RayInfo>();

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Debug"))
			{
				VolumeCalculator calculator = (VolumeCalculator)target;
				FieldInfo dataFieldInfo = calculator.GetType().GetField("_raytracingData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				RaytracingData data = (RaytracingData)dataFieldInfo.GetValue(calculator);

				Transform calculatorTransform = calculator.transform;
				float fov = (float)data.GetType().GetField("_fov", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(data);
				Vector2Int frameDimensions = new Vector2Int(3, 3);

				//foreach (Ray ray in DebugRayGenerator.Generate(_debugFrameDimensions, Matrix4x4.LookAt(transform.position, transform.position - transform.forward, transform.up), GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(_fov, 1, 0.1f, 1000), false).inverse, transform.position))
				//{
				//	Physics.Raycast(ray, out RaycastHit hitInfo);
				//	Gizmos.DrawLine(ray.origin, hitInfo.point);
				//	Vector3 reflectedDir = Vector3.Reflect(ray.direction, hitInfo.normal);
				//	Ray newRay = new Ray(hitInfo.point, reflectedDir);
				//	Physics.Raycast(newRay, out RaycastHit newHitInfo);
				//	Gizmos.DrawLine(newRay.origin, newHitInfo.point);
				//}
				EditorCoroutineUtility.StartCoroutine(DebugCoroutine(frameDimensions, fov, calculatorTransform), this);
			}
		}

		private void OnSceneGUI()
		{
			Handles.color = new Color(1, 1, 1, 0.5f);
			foreach (RayInfo info in _rays)
			{
				Handles.DrawLine(info.Origin, Vector3.Lerp(info.Origin, info.HitPoint, info.Time));
			}
		}

		private IEnumerator DebugCoroutine(Vector2Int frameDimensions, float fov, Transform transform)
		{
			_rays.Clear();

			List<Ray> startingRays = DebugRayGenerator.Generate(frameDimensions, Matrix4x4.LookAt(transform.position, transform.position - transform.forward, transform.up), GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(fov, 1, 0.1f, 1000), false).inverse, transform.position);

			foreach (Ray ray in startingRays)
			{
				Physics.Raycast(ray, out RaycastHit hitInfo);
				_rays.Add(new RayInfo(ray.origin, hitInfo.point, 0));

				Vector3 reflectedDir = Vector3.Reflect(ray.direction, hitInfo.normal);
				Ray ray1 = new Ray(hitInfo.point, reflectedDir);
				Physics.Raycast(ray1, out RaycastHit hitInfo1);
				_rays.Add(new RayInfo(ray1.origin, hitInfo1.point, -1));

				Vector3 reflectedDir1 = Vector3.Reflect(ray1.direction, hitInfo1.normal);
				Ray ray2 = new Ray(hitInfo1.point, reflectedDir1);
				Physics.Raycast(ray2, out RaycastHit hitInfo2);
				_rays.Add(new RayInfo(ray2.origin, hitInfo2.point, -2));

				Vector3 reflectedDir2 = Vector3.Reflect(ray2.direction, hitInfo2.normal);
				Ray ray3 = new Ray(hitInfo2.point, reflectedDir2);
				Physics.Raycast(ray3, out RaycastHit hitInfo3);
				_rays.Add(new RayInfo(ray3.origin, hitInfo3.point, -3));
			}

			for (float i = 0; i < 30; i += Time.deltaTime)
			{
				foreach (RayInfo info in _rays)
				{
					info.Time += Time.deltaTime / 5;
				}
				SceneView.RepaintAll();
				yield return null;
			}

			//for (float i = 0; i <= 1; i += Time.deltaTime)
			//{
			//	foreach (RayInfo info in rays)
			//	{
			//		Handles.DrawLine(info.Origin, Vector3.Lerp(info.Origin, info.HitPoint, i));
			//	}
			//	yield return null;
			//}
		}

		private class RayInfo
		{
			public Vector3 Origin { get; }
			public Vector3 HitPoint { get; }
			public float Time { get; set; }

			public RayInfo(Vector3 origin, Vector3 hitPoint, float time)
			{
				Origin = origin;
				HitPoint = hitPoint;
				Time = time;
			}
		}
	}
}