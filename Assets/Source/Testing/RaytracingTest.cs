using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace AudioChat
{
	public class RaytracingTest : MonoBehaviour
	{
		[SerializeField] private Vector2Int _size = new Vector2Int(256, 256);
		[SerializeField] private Camera _camera;
		[SerializeField] private LayerMask _layer;
		[SerializeField] private RayTracingShader _rayTracingShader;
		[SerializeField] private RenderTexture _renderTexture;

		private RayTracingAccelerationStructure _rayTracingAccelerationStructure;

		private void Start()
		{
			//_size = new Vector2Int(Screen.width, Screen.height);

			_renderTexture = new RenderTexture(_size.x, _size.y, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Default);
			_renderTexture.enableRandomWrite = true;
			_renderTexture.Create();

			var settings = new RayTracingAccelerationStructure.RASSettings();
			settings.layerMask = _layer;
			settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Automatic;
			settings.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;

			_rayTracingAccelerationStructure = new RayTracingAccelerationStructure(settings);
			_rayTracingAccelerationStructure.Build();

			var projection = GL.GetGPUProjectionMatrix(_camera.projectionMatrix, false);
			var inverseProjection = projection.inverse;

			_rayTracingShader.SetAccelerationStructure("_RaytracingAccelerationStructure", _rayTracingAccelerationStructure);
			_rayTracingShader.SetShaderPass("Test");
			_rayTracingShader.SetMatrix("_InverseProjection", inverseProjection);
			_rayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
			_rayTracingShader.SetVector("_WorldSpaceCameraPos", _camera.transform.position);
			_rayTracingShader.SetTexture("RenderTarget", _renderTexture);
			_rayTracingShader.Dispatch("MyRaygenShader", _size.x, _size.y, 1, _camera);
		}
	}
}
