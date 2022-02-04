using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
	public class MicrophoneWrapper : IDisposable
	{
		private AudioClip _clip;
		private List<string> _microphoneDevices;
		private int _microphoneIndex;

		private int micPrevPos;
		private int micLoopCnt;
		private int readAbsPos;

		private string CurrentMicrophoneDevice
		{ get { return _microphoneDevices.Count > 0 ? _microphoneDevices[_microphoneIndex] : null; } }

		public MicrophoneWrapper(int frequency)
		{
			FindDevices();
			_clip = Microphone.Start(CurrentMicrophoneDevice, true, 1, frequency);
		}
		
		public bool Read(float[] buffer)
		{
			int micPos = Microphone.GetPosition(CurrentMicrophoneDevice);

			if (micPos < micPrevPos)
			{
				micLoopCnt++;
			}
			micPrevPos = micPos;
			int micAbsPos = micLoopCnt * _clip.samples + micPos;

			int bufferSamplesCount = buffer.Length / _clip.channels;
			int nextReadPos = readAbsPos + bufferSamplesCount;

			if (nextReadPos < micAbsPos)
			{
				_clip.GetData(buffer, readAbsPos % _clip.samples);
				readAbsPos = nextReadPos;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Dispose()
		{
			Microphone.End(CurrentMicrophoneDevice);
		}

		private void FindDevices()
		{
			_microphoneDevices = new List<string>();
			foreach (var device in Microphone.devices)
			{
				_microphoneDevices.Add(device);
			}
			_microphoneIndex = 0;
		}
	}
}
