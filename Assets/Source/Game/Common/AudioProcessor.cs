using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public class AudioProcessor
	{
		const int BUFFER_SIZE = 50;
		const int DELAY_LOW = 200;
		const int DELAY_HIGH = 400;
		const int DELAY_MAX = 1000;
		const int SPEED_UP_PERC = 5;
		const int TEMPO_UP_SKIP_GROUP = 6;

		const int NO_PUSH_TIMEOUT_MS = 100; // should be greater than Push() call interval

		private AudioSource _audioSource;
		private Queue<float[]> _frameQueue = new Queue<float[]>();
		private PrimitiveArrayPool<float> _framePool = new PrimitiveArrayPool<float>(BUFFER_SIZE, null);

		private int _frequency;
		private int _channels;
		private int _targetDelaySamples;
		private int _upperTargetDelaySamples;
		private int _maxDelaySamples;

		private int _bufferSamples;
		private int _frameSamples;
		private int _frameSize;
		private int _sourceTimeSamplesPrev;
		private int _playSamplePosPrev;
		private int _playLoopCount;
		private int _clipWriteSamplePos;

		private float[] _zeroFrame;
		private float[] _resampledFrame;

		private bool _flushed = true;
		private bool _catchingUp;
		private bool _tempoChangeHQ;
		private TempoUp<float> _tempoUp;

		private int _lastPushTime = Environment.TickCount - NO_PUSH_TIMEOUT_MS;

		private int OutPos
		{
			get { return _audioSource.timeSamples; }
		}

		public bool IsFlushed
		{
			get { return _flushed; }
		}

		public bool IsPlaying
		{
			get { return !_flushed && (Environment.TickCount - _lastPushTime < NO_PUSH_TIMEOUT_MS); }
		}

		public AudioProcessor(AudioSource audioSource, VoiceInfo voiceInfo)
		{
			_audioSource = audioSource;
			int frameSamples = voiceInfo.FrameDurationSamples;

			_frequency = (int)voiceInfo.SamplingRate;
			_channels = (int)voiceInfo.NumChannels;
			_targetDelaySamples = DELAY_LOW * _frequency / 1000 + frameSamples;
			_upperTargetDelaySamples = DELAY_HIGH * _frequency / 1000 + frameSamples;

			if (_upperTargetDelaySamples < _targetDelaySamples + 2 * frameSamples)
			{
				_upperTargetDelaySamples = _targetDelaySamples + 2 * frameSamples;
			}

			//int resampleRampEndMs = DELAY_MAX;

			_maxDelaySamples = DELAY_MAX * _frequency / 1000;
			if (_maxDelaySamples < _upperTargetDelaySamples)
			{
				_maxDelaySamples = _upperTargetDelaySamples;
			}

			_bufferSamples = 3 * _maxDelaySamples;
			_frameSamples = frameSamples;
			_frameSize = frameSamples * _channels;

			_clipWriteSamplePos = _targetDelaySamples;

			if (_framePool.Info != _frameSize)
			{
				_framePool.Init(_frameSize);
			}

			_zeroFrame = new float[_frameSize];
			_resampledFrame = new float[_frameSize];

			_tempoChangeHQ = false;
			if (!_tempoChangeHQ)
			{
				_tempoUp = new TempoUp<float>();
			}
			
			OutCreate(_frequency, _channels, _bufferSamples);
		}

		public void Start()
		{
			OutStart();
		}

		public void Update()
		{
			int sourceTimeSamples = OutPos;
			if (sourceTimeSamples < _sourceTimeSamplesPrev)
			{
				_playLoopCount++;
			}
			_sourceTimeSamplesPrev = sourceTimeSamples;

			int playSamplePos = _playLoopCount * _bufferSamples + sourceTimeSamples;

			while (_frameQueue.Count > 0)
			{
				float[] frame = _frameQueue.Dequeue();
				if (ProcessFrame(frame, playSamplePos))
				{
					return;
				}
				_framePool.Release(frame, frame.Length);
			}

			// clear played back buffer segment
			var clearStart = _playSamplePosPrev;
			var clearMin = playSamplePos - _bufferSamples;
			if (clearStart < clearMin)
			{
				clearStart = clearMin;
			}
			// round up
			var framesToClear = (playSamplePos - clearStart - 1) / _frameSamples + 1;
			for (var offset = playSamplePos - framesToClear * _frameSamples; offset < playSamplePos; offset += _frameSamples)
			{
				var o = offset % _bufferSamples;
				if (o < 0) o += _bufferSamples;
				OutWrite(_zeroFrame, o);
			}
			_playSamplePosPrev = playSamplePos;
		}

		public void Flush()
		{
			_frameQueue.Enqueue(null);
		}

		public void Push(float[] frame)
		{
			if (frame.Length == 0)
				return;

			if (frame.Length != _frameSize)
				Debug.LogError("Wrong frame size.");

			float[] pooled = _framePool.AcquireOrCreate();
			Buffer.BlockCopy(frame, 0, pooled, 0, frame.Length * sizeof(float));
			_frameQueue.Enqueue(pooled);
		}

		private bool ProcessFrame(float[] frame, int playSamplePos)
		{
			int lagSamples = _clipWriteSamplePos - playSamplePos;
			if (!_flushed)
			{
				if (lagSamples > _maxDelaySamples)
				{
					_clipWriteSamplePos = playSamplePos + _maxDelaySamples;
					lagSamples = _maxDelaySamples;
				}
				else if (lagSamples < 0)
				{
					_clipWriteSamplePos = playSamplePos;
					lagSamples = _targetDelaySamples;
				}
			}

			if (frame == null) //force flush
			{
				_flushed = true;
				if (_catchingUp)
				{
					_catchingUp = false;
				}
				return true;
			}
			else
			{
				if (_flushed)
				{
					_clipWriteSamplePos = playSamplePos;
					lagSamples = _targetDelaySamples;
					_flushed = false;
				}
			}

			if (lagSamples > _upperTargetDelaySamples)
			{
				if (!_tempoChangeHQ)
				{
					_tempoUp.Begin(_channels, SPEED_UP_PERC, TEMPO_UP_SKIP_GROUP);
				}
				_catchingUp = true;
			}

			bool frameIsWritten = false;
			if (lagSamples <= _targetDelaySamples && _catchingUp)
			{
				if (!_tempoChangeHQ)
				{
					int skipSamples = _tempoUp.End(frame);
					int resampledLenSamples = frame.Length / _channels - skipSamples;
					Buffer.BlockCopy(frame, skipSamples * _channels * sizeof(float), _resampledFrame, 0, resampledLenSamples * _channels * sizeof(float));
					WriteResampled(_resampledFrame, resampledLenSamples);
					frameIsWritten = true;
				}
				_catchingUp = false;
			}

			if (frameIsWritten)
				return false;

			if (_catchingUp)
			{
				if (!_tempoChangeHQ)
				{
					int resampledLenSamples = _tempoUp.Process(frame, _resampledFrame);
					WriteResampled(_resampledFrame, resampledLenSamples);
				}
			}
			else
			{
				OutWrite(frame, _clipWriteSamplePos % _bufferSamples);
				_clipWriteSamplePos += frame.Length / _channels;
			}

			return false;
		}

		private int WriteResampled(float[] f, int resampledLenSamples)
		{
			// zero not used part of the buffer because SetData applies entire frame
			// if this frame is the last, grabage may be played back
			var tailSize = (f.Length - resampledLenSamples * _channels) * sizeof(float);
			if (tailSize > 0) // it may be 0 what BlockCopy does not like
			{
				Buffer.BlockCopy(_zeroFrame, 0, f, resampledLenSamples * _channels * sizeof(float), tailSize);
			}

			OutWrite(f, _clipWriteSamplePos % _bufferSamples);
			_clipWriteSamplePos += resampledLenSamples;
			return resampledLenSamples;
		}

		private void OutStart()
		{
			_audioSource.Play();
		}

		private void OutCreate(int frequency, int channels, int bufferSamples)
		{
			_audioSource.loop = true;
			_audioSource.clip = AudioClip.Create("clip", bufferSamples, channels, frequency, false);
		}

		private void OutWrite(float[] data, int offsetSamples)
		{
			_audioSource.clip.SetData(data, offsetSamples);
		}
	}
}
