using Photon.Pun;
using UnityEngine;

namespace AudioChat
{
	public class WhiteboardPen : MonoBehaviour
	{
		[SerializeField] private float _tipSize;
		[SerializeField] private WhiteboardInstrument _instrument = WhiteboardInstrument.NONE;

		private Whiteboard _lastWhiteboard;
		private RaycastHit _lastHit;
		private Vector2 _previousPoint;
		private bool _isTouching;

		// =============================================================

		private void Start()
		{
			PhotonView view = GetComponentInParent<PhotonView>();
			if (!view.IsMine)
				enabled = false;
		}

		private void FixedUpdate()
		{
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit info, _tipSize))
			{
				_lastHit = info;
				_lastWhiteboard = info.transform.GetComponent<Whiteboard>();
				if (_lastWhiteboard)
				{
					if (_isTouching)
					{
						Touch();
					}
					else
					{
						TouchStart();
					}
				}
			}
			else
			{
				TouchEnd();
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawLine(transform.position, transform.position + transform.forward * _tipSize);
		}

		// =============================================================

		private void TouchStart()
		{
			_isTouching = true;
			_previousPoint = _lastHit.textureCoord;
		}

		private void Touch()
		{
			_lastWhiteboard.DrawLine(_instrument, _previousPoint, _lastHit.textureCoord);
			_previousPoint = _lastHit.textureCoord;
		}

		private void TouchEnd()
		{
			if (_isTouching)
			{
				if (_lastWhiteboard != null)
					_lastWhiteboard.DrawLine(_instrument, _previousPoint, _lastHit.textureCoord);
				_isTouching = false;
				_lastWhiteboard = null;
			}
		}
	}
}
