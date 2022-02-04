using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public class RayDebugger : MonoBehaviour
	{
		[SerializeField] private LineRenderer _rendererPrefab;
		[SerializeField] private LayerMask _mask;

		private void Start()
		{
			Test();
		}

		//[ContextMenu("Debug")]
		private void Test()
		{
			float fov = 80;
			Vector2Int frameDimensions = new Vector2Int(5, 5);
			int reflections = 6;

			List<Ray> startingRays = DebugRayGenerator.Generate(frameDimensions, Matrix4x4.LookAt(transform.position, transform.position - transform.forward, transform.up), GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(fov, 1, 0.1f, 1000), false).inverse, transform.position);
			
			for (int i = 0, n = frameDimensions.x * frameDimensions.y; i < n; i++)
			{
				StartCoroutine(RayCoroutine(startingRays[i], i, reflections));
			}
		}

		private IEnumerator RayCoroutine(Ray ray, int index, int reflections)
		{
			LineRenderer lineRenderer = Instantiate(_rendererPrefab, transform);
			lineRenderer.positionCount = 1;
			lineRenderer.SetPosition(0, ray.origin);

			Ray currentRay = ray;
			for (int i = 0; i < reflections; i++)
			{
				int lineEndIndex = 1 + i;
				lineRenderer.positionCount = 2 + i;
				Physics.Raycast(currentRay, out RaycastHit hitInfo, Mathf.Infinity, _mask);

				yield return CoroutineInterpolator.InterpolateUnmanaged(this, currentRay.origin, hitInfo.point, Vector3.Distance(currentRay.origin, hitInfo.point) * 0.5f, 
					(Vector3 point) =>
					{
						lineRenderer.SetPosition(lineEndIndex, point);
					});

				if (1 << hitInfo.collider?.gameObject.layer == LayerMask.GetMask("DebugPlayer"))
				{
					lineRenderer.startColor = Color.green;
					lineRenderer.endColor = Color.green;
					yield break;
				}

				Vector3 reflectedDir = Vector3.Reflect(currentRay.direction, hitInfo.normal);
				currentRay = new Ray(hitInfo.point, reflectedDir);
			}
		}
	}
}
