using UnityEngine;

namespace Tools
{
	public class GizmosExtensions
	{
		public static void DrawGizmoFOV(Transform transform, float fov, float range = 5.0f)
		{
			Gizmos.color = Color.blue;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.DrawFrustum(Vector3.zero, fov, 5, 0, 1);
			//float totalFOV = fov;
			//float rayRange = range;
			//float halfFOV = totalFOV / 2.0f;
			//Gizmos.color = Color.blue;
			//Gizmos.DrawRay(transform.position, (transform.rotation * Quaternion.Euler(Vector3.up * halfFOV)) * transform.forward);
			//Gizmos.DrawRay(transform.position, (transform.rotation * Quaternion.Euler(Vector3.up * -halfFOV)) * transform.forward);
		}
	}
}
