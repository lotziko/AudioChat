using System;

namespace Tools
{
	/// <summary>Voice Activity Detector interface.</summary>
	public interface IVoiceDetector
	{
		/// <summary>If true, voice detection enabled.</summary>
		bool On { get; set; }

		/// <summary>Voice detected as soon as signal level exceeds threshold.</summary>
		float Threshold { get; set; }

		/// <summary>If true, voice detected.</summary>
		bool Detected { get; }

		/// <summary>Last time when switched to detected state.</summary>
		DateTime DetectedTime { get; }

		/// <summary>Called when switched to detected state.</summary>
		event Action OnDetectedStart;

		/// <summary>Keep detected state during this time after signal level dropped below threshold.</summary>
		int ActivityDelayMs { get; set; }

		bool Process(float[] buffer);
	}

	/// <summary>
	/// Simple voice activity detector triggered by signal level.
	/// </summary>
	public class VoiceDetector : IVoiceDetector
	{
		/// <summary>If true, voice detection enabled.</summary>
		public bool On { get; set; }

		/// <summary>Voice detected as soon as signal level exceeds threshold.</summary>
		public float Threshold { get { return threshold * norm; } set { threshold = value / norm; } }

		protected float norm;
		protected float threshold;
		bool detected;

		/// <summary>If true, voice detected.</summary>
		public bool Detected
		{
			get { return detected; }
			protected set
			{
				if (detected != value)
				{
					detected = value; DetectedTime = DateTime.Now;
					if (detected && OnDetectedStart != null) OnDetectedStart();
					if (!detected && OnDetectedEnd != null) OnDetectedEnd();
				}
			}
		}

		/// <summary>Last time when switched to detected state.</summary>
		public DateTime DetectedTime { get; private set; }

		/// <summary>Keep detected state during this time after signal level dropped below threshold.</summary>
		public int ActivityDelayMs
		{
			get { return this.activityDelay; }
			set
			{
				this.activityDelay = value;
				this.activityDelayValuesCount = value * valuesCountPerSec / 1000;
			}
		}

		/// <summary>Called when switched to detected state.</summary>
		public event Action OnDetectedStart;
		public event Action OnDetectedEnd;

		protected int activityDelay;
		protected int autoSilenceCounter = 0;
		protected int valuesCountPerSec;
		protected int activityDelayValuesCount;

		internal VoiceDetector(int samplingRate, int numChannels)
		{
			this.valuesCountPerSec = samplingRate * numChannels;
			this.ActivityDelayMs = 500;
			this.On = true;
			this.norm = 1f;
		}

		public bool Process(float[] buffer)
		{
			if (this.On)
			{
				foreach (var s in buffer)
				{
					if (s > this.threshold)
					{
						this.Detected = true;
						this.autoSilenceCounter = 0;
					}
					else
					{
						this.autoSilenceCounter++;
					}
				}
				if (this.autoSilenceCounter > this.activityDelayValuesCount)
				{
					this.Detected = false;
				}
				return Detected;
			}
			else
			{
				return true;
			}
		}

		public void Dispose()
		{
		}
	}
}
