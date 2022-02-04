using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public enum WhiteboardInstrument : short
	{
		NONE, MARKER_BLACK, ERASER
	}

	public class Whiteboard : MonoBehaviour
	{
		DIVar<INetworkManager> _networkManager = new DIVar<INetworkManager>();

		[SerializeField] private PhotonView _view;
		[SerializeField] private MeshRenderer _boardRenderer;
		[SerializeField] private InstrumentInfo[] _infos;
		[SerializeField] private Texture2D _defaultData;

		[SerializeField] private Vector2Int _size = new Vector2Int(512, 512);
		[SerializeField] private float _drawTimer = 0.5f;

		private Dictionary<WhiteboardInstrument, InstrumentInfo> _infosDict = new Dictionary<WhiteboardInstrument, InstrumentInfo>();
		private Texture2D _blackTexture;
		private RenderTexture _targetTexture;
		private Material _boardMaterial;

		private float _currentTimer;

		private Queue<WhiteboardLine> _lineQueue = new Queue<WhiteboardLine>(); //add color later

		// =============================================================

		private void Awake()
		{
			foreach (InstrumentInfo info in _infos)
				_infosDict.Add(info.Instrument, info);

			_blackTexture = new Texture2D(2, 2);
			_blackTexture.SetPixels(0, 0, 2, 2, new Color[] { Color.clear, Color.clear, Color.clear, Color.clear });
			_targetTexture = new RenderTexture(_size.x, _size.y, 0, RenderTextureFormat.ARGB32, 0);

			_boardMaterial = _boardRenderer.material;
			_boardMaterial.SetTexture("_PaintMap", _targetTexture);

			if (_networkManager.Value.IsMaster && _defaultData)
			{
				Graphics.Blit(_defaultData, _targetTexture);
			}

			//if (_networkManager.Value.IsMaster)
			//{
			//	_lineQueue.Enqueue(new WhiteboardLine(WhiteboardInstrument.MARKER_BLACK, new Vector2(0, 0), new Vector2(1, 1)));
			//	_lineQueue.Enqueue(new WhiteboardLine(WhiteboardInstrument.ERASER, new Vector2(0, 1), new Vector2(1, 0)));
			//	//_lineQueue.Enqueue(new Line(new Vector4(0, 0.5f, 1, 0.5f), Color.black));
			//}
		}

		private void OnEnable()
		{
			AddCallbacks();
		}

		private void OnDisable()
		{
			RemoveCallbacks();
		}

		private void OnDestroy()
		{
			if (_targetTexture != null)
				_targetTexture.Release();
			if (_blackTexture != null)
				Destroy(_blackTexture);
		}

		private void Update()
		{
			ProcessQueue();
			_currentTimer -= Time.deltaTime;
		}

		// =============================================================

		public void DrawLine(WhiteboardInstrument instrument, Vector2 first, Vector2 second)
		{
			if (instrument == WhiteboardInstrument.NONE)
				return;

			_view.RPC("DrawLineRPC", RpcTarget.All, new WhiteboardLine(instrument, first, second));
		}

		private void AddCallbacks()
		{
			_networkManager.Value.OnPlayerJoined += OnPlayerJoined;
		}

		private void RemoveCallbacks()
		{
			_networkManager.Value.OnPlayerJoined -= OnPlayerJoined;
		}

		private void ProcessQueue()
		{
			//better check magnitude of lines
			if (_currentTimer < 0)
			{
				int count = _lineQueue.Count;
				for (int i = 0; i < count; i++)
				{
					WhiteboardLine line = _lineQueue.Dequeue();
					Material material = _infosDict[line.Instrument].Material;
					material.SetVector("_Points", line.Points);
					Graphics.Blit(_blackTexture, _targetTexture, material);
				}
				_currentTimer = _drawTimer;
			}
		}

		[PunRPC]
		private void DrawLineRPC(WhiteboardLine line)
		{
			_lineQueue.Enqueue(line);
		}

		[PunRPC]
		private void SyncTexture(byte[] data)
		{
			Texture2D tempTexture = new Texture2D(_targetTexture.width, _targetTexture.height, TextureFormat.ARGB32, false);
			tempTexture.LoadImage(data);
			Graphics.CopyTexture(tempTexture, _targetTexture);
			Destroy(tempTexture);
		}

		// =============================================================

		private void OnPlayerJoined(Photon.Realtime.Player player)
		{
			if (_networkManager.Value.IsMaster)
			{
				StartCoroutine(SyncCoroutine(player, 0.1f));
			}
		}

		// =============================================================

		private IEnumerator SyncCoroutine(Photon.Realtime.Player player, float delay)
		{
			yield return new WaitForSeconds(delay);
			Texture2D tempTexture = new Texture2D(_targetTexture.width, _targetTexture.height, TextureFormat.ARGB32, false);
			RenderTexture currentRenderTexture = RenderTexture.active;
			RenderTexture.active = _targetTexture;
			tempTexture.ReadPixels(new Rect(0, 0, _targetTexture.width, _targetTexture.height), 0, 0);
			_view.RPC("SyncTexture", player, tempTexture.EncodeToPNG());
			Destroy(tempTexture);
			RenderTexture.active = currentRenderTexture;
		}

		[System.Serializable]
		private class InstrumentInfo
		{
			[SerializeField] private WhiteboardInstrument _instrument;
			[SerializeField] private Material _material;

			public WhiteboardInstrument Instrument
			{ get { return _instrument; } }

			public Material Material
			{ get { return _material; } }
		}
	}
}
