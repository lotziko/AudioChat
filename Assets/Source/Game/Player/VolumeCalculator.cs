using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace AudioChat
{
	public enum VolumeCalculationMode
	{
		None,
		//dot based too
		Raytracing,
	}
	
	public class VolumeCalculator : MonoBehaviour
	{
		DIVar<IProfileManager> _profileManager = new DIVar<IProfileManager>();
		DIVar<IPlayerManager> _playerManager = new DIVar<IPlayerManager>();

		[SerializeField] private AudioSource _targetSource;
		[SerializeField] private CalculationData _calculationData;
		[SerializeField] private RaytracingData _raytracingData;

		//private Vector3 _targetSourcePosition;
		private float _targetVolume = 1;

		private Vector3 _lastCalculationPosition;
		private Quaternion _lastCalculationAngle;

		private Vector3 _lastListenerCalculationPosition;
		private Quaternion _lastListenerCalculationAngle;

		private AudioListener _listener;

		private bool _isDirty = true;

		// =============================================================

		private void Start()
		{
			_lastCalculationPosition = transform.position;
			_lastCalculationAngle = transform.rotation;
			//_targetSourcePosition = transform.position;
			StartCoroutine(CalculationCoroutine());

			_playerManager.Value.OnUpdate += OnPlayerUpdate;
			FindListener();
			_isDirty = true;
		}

		private void Update()
		{
			_targetSource.volume = Mathf.Lerp(_targetSource.volume, _targetVolume, 0.5f);
			//_targetSource.transform.position = Vector3.Lerp(_targetSource.transform.position, _targetSourcePosition, 0.5f);
		}

		private void OnDestroy()
		{
			_playerManager.Value.OnUpdate -= OnPlayerUpdate;
			Dispose();
		}

		//private void OnDrawGizmos()
		//{
		//	GizmosExtensions.DrawGizmoFOV(transform, _fov);

		//	#region old

		//	//Camera camera = GetComponent<Camera>();
		//	//Matrix4x4 viewMatrix = Matrix4x4.LookAt(transform.position, transform.position - transform.forward, transform.up);//camera.cameraToWorldMatrix;//Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, -1)) * transform.localToWorldMatrix.inverse;//camera.cameraToWorldMatrix;
		//	//Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(60, 1, 0.1f, 1000), false).inverse;//GL.GetGPUProjectionMatrix(, false).inverse;

		//	//int raysX = 16, raysY = 16;

		//	//for (int i = 0; i < raysX; i++)
		//	//{
		//	//	for (int j = 0; j < raysY; j++)
		//	//	{
		//	//		Vector2 texcoord = new Vector2(i + 0.5f, j + 0.5f) / new Vector2(raysX, raysY);
		//	//		Vector3 viewPosition = new Vector3(texcoord.x * 2 - 1, texcoord.y * 2 - 1, 0);
		//	//		Vector4 clip = new Vector4(viewPosition.x, viewPosition.y, viewPosition.z, 1);

		//	//		Vector4 worldPos = projectionMatrix * clip;
		//	//		worldPos /= worldPos.w;
		//	//		worldPos = viewMatrix * worldPos;
		//	//		Gizmos.DrawLine(transform.position, worldPos);
		//	//	}
		//	//}

		//	//_raytracingData.DrawGizmos(transform);

		//	#endregion
		//}

		private void OnGUI()
		{
			//if (_profileManager.Value.GetVolumeCalculationMode() == VolumeCalculationMode.Raytracing)
			//	_raytracingData?.OnGUI();
		}

		// =============================================================

		private void OnPlayerUpdate()
		{
			FindListener();
			_isDirty = true;
		}

		// =============================================================

		private IEnumerator CalculationCoroutine()
		{
			WaitForSeconds delay = new WaitForSeconds(_calculationData.SecondsBetweenCalculations);
			Initialize();
			while (true)
			{
				if (!_isDirty)
				{
					_isDirty = CheckForDirty();
				}

				if (_isDirty)
				{
					Calculate();
					_isDirty = false;
				}
				yield return delay;
			}
		}

		private bool CheckForDirty()
		{
			float listenerAngleDifference = Quaternion.Angle(_lastListenerCalculationAngle, _listener.transform.rotation);
			if (listenerAngleDifference > _calculationData.DirtyDegree)
			{
				_lastListenerCalculationAngle = _listener.transform.rotation;
				return true;
			}

			float listenerPositionDifference = Vector3.Distance(_lastListenerCalculationPosition, _listener.transform.position);
			if (listenerPositionDifference > _calculationData.DirtyDistance)
			{
				_lastListenerCalculationPosition = _listener.transform.position;
				return true;
			}

			float angleDifference = Quaternion.Angle(_lastCalculationAngle, transform.rotation);
			if (angleDifference > _calculationData.DirtyDegree)
			{
				_lastCalculationAngle = transform.rotation;
				return true;
			}

			float positionDifference = Vector3.Distance(_lastCalculationPosition, transform.position);
			if (positionDifference > _calculationData.DirtyDistance)
			{
				_lastCalculationPosition = transform.position;
				return true;
			}

			return false;
		}

		private void FindListener()
		{
			_listener = _playerManager.Value.LocalPlayer?.GetComponentInChildren<AudioListener>();
			if (_listener)
			{
				_lastListenerCalculationPosition = _listener.transform.position;
				_lastListenerCalculationAngle = _listener.transform.rotation;
			}
		}

		private void Initialize()
		{
			if (_profileManager.Value.GetVolumeCalculationMode() == VolumeCalculationMode.Raytracing)
				_raytracingData.Initialize();
		}

		private void Calculate()
		{
			if (_profileManager.Value.GetVolumeCalculationMode() == VolumeCalculationMode.Raytracing)
			{
				float volume = _raytracingData.Calculate(transform);
				_targetVolume = volume;

				//float distance = _raytracingData.Calculate(transform);
				//float distanceToListener = Vector3.Distance(transform.position, _listener.transform.position);
				//if (_listener)
				//{
				//	_targetSourcePosition = (_listener.transform.position + (transform.position - _listener.transform.position).normalized * distance);
				//}
			}
		}

		private void Dispose()
		{
			if (_profileManager.Value.GetVolumeCalculationMode() == VolumeCalculationMode.Raytracing)
				_raytracingData.Dispose();
		}

		// =============================================================

		[System.Serializable]
		private class CalculationData
		{
			[SerializeField] private float _dirtyDistance = 0.5f;
			[SerializeField] private float _dirtyDegree = 15;
			[SerializeField] private float _secondsBetweenCalculations = 1f;

			public float DirtyDistance => _dirtyDistance;
			public float DirtyDegree => _dirtyDegree;
			public float SecondsBetweenCalculations => _secondsBetweenCalculations;
		}

		[System.Serializable]
		public class RaytracingData
		{
			[SerializeField] private Vector2Int _frameBounds;
			[SerializeField] private LayerMask _layer;
			[SerializeField] private float _fov = 120;
			[SerializeField] private RayTracingShader _rayTracingShader;

			[SerializeField] private Texture2D _renderTexture;

			private RayTracingShader _shader;
			private ComputeBuffer _dataBuffer;
			private Vector4[] _dataArray;

			private RayTracingAccelerationStructure.RASSettings _settings;
			private RayTracingAccelerationStructure _rayTracingAccelerationStructure;

			public void Initialize()
			{
				_shader = Instantiate(_rayTracingShader);

				_renderTexture = new Texture2D(_frameBounds.x, _frameBounds.y);

				_dataBuffer = new ComputeBuffer(_frameBounds.x * _frameBounds.y, sizeof(float) * 4, ComputeBufferType.Raw);
				_dataArray = new Vector4[_frameBounds.x * _frameBounds.y];

				_settings = new RayTracingAccelerationStructure.RASSettings();
				_settings.layerMask = _layer;
				_settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Automatic;
				_settings.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;

				_rayTracingAccelerationStructure = new RayTracingAccelerationStructure(_settings);

				_shader.SetShaderPass("RayPass");
				_shader.SetFloat("_CameraZFar", 1000);
				_shader.SetMatrix("_InverseProjectionMatrix", GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(_fov, 1, 0.1f, 1000), false).inverse);
				_shader.SetBuffer("_DataBuffer", _dataBuffer);
			}

			public void OnGUI()
			{
				GUI.DrawTexture(new Rect(10, 10, 300, 300), _renderTexture, ScaleMode.ScaleToFit, false, 0);
			}

			public void Dispose()
			{
				if (_renderTexture)
					Destroy(_renderTexture);
				if (_dataBuffer != null)
					_dataBuffer.Dispose();
			}

			public float Calculate(Transform transform)
			{
				_rayTracingAccelerationStructure.Build();
				_shader.SetAccelerationStructure("_RaytracingAccelerationStructure", _rayTracingAccelerationStructure);

				_shader.SetVector("_WorldSpaceCameraPos", transform.position);
				_shader.SetMatrix("_InverseViewMatrix", Matrix4x4.LookAt(transform.position, transform.position - transform.forward, transform.up));

				_shader.Dispatch("MyRaygenShader", _frameBounds.x, _frameBounds.y, 1);
				_dataBuffer.GetData(_dataArray);

				//float closestDistance = 100;
				float longestDistance = float.MinValue;
				
				int closestDepth = 10;
				float closestVolume = 0;

				int hits = 0;

				for (int i = 0; i < _frameBounds.x * _frameBounds.y; i++)
				{
					if (_dataArray[i].x > 0)
					{
						++hits;

						//float distance = _dataArray[i].y;
						int depth = (int)_dataArray[i].y;
						if (depth < closestDepth)
						{
							closestDepth = depth;
							//closestDistance = _dataArray[i].z;
							closestVolume = _dataArray[i].w;
						}
						//if (distance < closestDistance)
						//{
						//	closestDistance = distance;
						//}
						longestDistance = Mathf.Max(longestDistance, _dataArray[i].z);
					}
				}

				//Debug.Log(hits);
				
				//Debug.Log((float)counter / (_frameBounds.x * _frameBounds.y) * 10);

				for (int i = 0; i < _frameBounds.x; i++)
				{
					for (int j = 0; j < _frameBounds.y; j++)
					{
						//float distance = _dataArray[i + j * _frameBounds.x].y * 1000;
						_renderTexture.SetPixel(i, j, Color.white * (_dataArray[i + j * _frameBounds.x].z / longestDistance));
					}
				}
				//closestDistance *= 1000;
				_renderTexture.Apply();
				return (closestVolume * 0.75f + (hits / (_frameBounds.x * _frameBounds.y)) * 0.25f);//closestDistance * 1000;
			}
		}
	}
}
