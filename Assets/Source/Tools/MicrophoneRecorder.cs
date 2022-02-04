using System;
using UnityEngine;

namespace Tools
{
	public class MicrophoneRecorder : MonoBehaviour
	{
		public Action<float[]> OnSample;

		private MicrophoneWrapper _wrapper;
		private float[] _buffer;
		private bool _active = true;

		private void Update()
		{
			if (_wrapper == null || !_active)
				return;

			if (_wrapper.Read(_buffer))
			{
				OnSample?.Invoke(_buffer);
			}
		}

		public void SetActive(bool value)
		{
			_active = value;
		}

		public void StartRecording(VoiceInfo info)
		{
			_wrapper = new MicrophoneWrapper((int)info.SamplingRate);
			_buffer = new float[info.FrameDurationSamples];
		}

		public void StopRecording()
		{
			if (_wrapper == null)
				return;
			_wrapper.Dispose();
			_wrapper = null;
		}
	}
}
