using UnityEngine;

namespace AudioChat
{
	public class Rotator : MonoBehaviour
	{
		private void Update()
		{
			transform.Rotate(Vector3.up, 10 * Time.deltaTime);
		}
	}
}
