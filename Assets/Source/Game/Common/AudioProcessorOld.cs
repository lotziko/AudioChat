using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public class AudioProcessorOld
	{
		private enum Status
		{
			Ahead,
			Current,
			Behind
		}

		private Dictionary<int, Status> _segments = new Dictionary<int, Status>();
		private AudioSource _audioSource;
		private AudioBuffer _audioBuffer;
		private int _lastIndex = -1;
		private int _minSegmentCount = 0;

		private int _testIndex;

		public AudioProcessorOld(AudioSource audioSource, VoiceInfo voiceInfo)
		{
			_audioSource = audioSource;
			_audioBuffer = new AudioBuffer((int)voiceInfo.SamplingRate, (int)voiceInfo.NumChannels, voiceInfo.FrameDurationSamples, 50);//new AudioBuffer((int)frequency, (int)channels, (int)frequency / 1000 * sampleSize * (int)channels, 150);
			_audioSource.clip = _audioBuffer.Clip;
		}

		public void FeedData(float[] data)
		{
			int index = _testIndex++;

			if (_segments.ContainsKey(index))
				return;

			int locIdx = (int)(_audioSource.Position() * _audioBuffer.SegmentCount);
			locIdx = Mathf.Clamp(locIdx, 0, _audioBuffer.SegmentCount - 1);

			int bufferIndex = _audioBuffer.GetNormalizedIndex(index);

			if (locIdx == bufferIndex)
				return;

			_segments.Add(index, Status.Ahead);
			_audioBuffer.Write(index, data);
		}

		public void Update()
		{
			if (_audioSource.clip == null)
				return;
			
			var index = (int)(_audioSource.Position() * _audioBuffer.SegmentCount);

			if (_lastIndex != index)
			{
				_audioBuffer.Clear(_lastIndex);

				_segments.EnsureKey(_lastIndex, Status.Behind);
				_segments.EnsureKey(index, Status.Current);

				_lastIndex = index;
			}

			int readyCount = GetSegmentCountByStatus(Status.Ahead);
			if (readyCount == 0)
				_audioSource.mute = true;
			else if (readyCount >= _minSegmentCount)
			{
				_audioSource.mute = false;
				if (!_audioSource.isPlaying)
					_audioSource.Play();
			}
		}

		private int GetSegmentCountByStatus(Status status)
		{
			var matches = _segments.Where(x => x.Value == status);
			if (matches == null) return 0;
			return matches.Count();
		}

		private class AudioBuffer
		{
			public AudioClip Clip
			{
				get
				{
					return _clip;
				}
			}

			public int SegmentCount
			{
				get
				{
					return _segmentCount;
				}
			}

			private int _firstIndex;
			private AudioClip _clip;
			private int _segmentLength;
			private int _segmentCount;

			public AudioBuffer(int frequency, int channels, int segmentLength, int segmentCount)
			{
				_clip = AudioClip.Create("clip", segmentLength * segmentCount, channels, frequency, false);
				_firstIndex = -1;
				_segmentLength = segmentLength;
				_segmentCount = segmentCount;
			}

			public bool Write(int absoluteIndex, float[] data)
			{
				if (data.Length != _segmentLength)
					return false;
				if (absoluteIndex < 0 || absoluteIndex < _firstIndex)
					return false;
				if (_firstIndex == -1)
					_firstIndex = absoluteIndex;

				int localIndex = GetNormalizedIndex(absoluteIndex);

				if (localIndex >= 0)
					_clip.SetData(data, localIndex * _segmentLength);
				return true;
			}

			public int GetNormalizedIndex(int absoluteIndex)
			{
				if (_firstIndex == -1 || absoluteIndex <= _firstIndex)
					return -1;
				return (absoluteIndex - _firstIndex) % _segmentCount;
			}

			public bool Clear(int index)
			{
				if (index < 0) return false;
				
				if (index >= _segmentCount)
					index = GetNormalizedIndex(index);
				
				_clip.SetData(new float[_segmentLength], index * _segmentLength);
				return true;
			}

			public void Clear()
			{
				_clip.SetData(new float[_segmentLength * _segmentCount], 0);
			}
		}
	}
}
