using System;

namespace Tools.UnityOpus {
    public class Encoder : IDisposable {
		const int MaxPacketSize = 4000;

		private int bitrate;
        public int Bitrate {
            get { return bitrate; }
            set {
                Library.OpusEncoderSetBitrate(_encoder, value);
                bitrate = value;
            }
        }

        private int complexity;
        public int Complexity {
            get {
                return complexity;
            }
            set {
                Library.OpusEncoderSetComplexity(_encoder, value);
                complexity = value;
            }
        }

		private OpusSignal signal;
        public OpusSignal Signal {
            get { return signal; }
            set {
                Library.OpusEncoderSetSignal(_encoder, value);
                signal = value;
            }
        }

		private readonly byte[] writePacket = new byte[MaxPacketSize];
		private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });

		private int _frameSizePerChannel = 960;
		private Delay _encoderDelay = Delay.Delay20ms;
		public Delay EncoderDelay
		{
			set
			{
				_encoderDelay = value;
				_frameSizePerChannel = (int)((((int)_samplingRate) / 1000) * ((decimal)_encoderDelay) / 2);
			}
			get
			{
				return _encoderDelay;
			}
		}

		private IntPtr _encoder;
		private NumChannels _channels;
		private SamplingRate _samplingRate;

		public Encoder(SamplingRate inputSamplingRateHz, NumChannels numChannels, int bitrate, OpusApplication applicationType, Delay encoderDelay)
		{
			_channels = numChannels;
			_samplingRate = inputSamplingRateHz;
			_channels = numChannels;
			_encoder = Library.OpusEncoderCreate(inputSamplingRateHz, numChannels, applicationType, out ErrorCode error);
			if (error != ErrorCode.OK)
			{
				UnityEngine.Debug.LogError("[UnityOpus] Failed to init encoder. Error code: " + error.ToString());
				_encoder = IntPtr.Zero;
			}
			EncoderDelay = encoderDelay;
		}

			//public Encoder(
			//    SamplingFrequency samplingFrequency,
			//    NumChannels channels,
			//    OpusApplication application) {
			//    this.channels = channels;
			//    ErrorCode error;
			//    encoder = Library.OpusEncoderCreate(
			//        samplingFrequency,
			//        channels,
			//        application,
			//        out error);
			//    if (error != ErrorCode.OK) {
			//        UnityEngine.Debug.LogError("[UnityOpus] Failed to init encoder. Error code: " + error.ToString());
			//        encoder = IntPtr.Zero;
			//    }
			//}

		

        public int Encode(float[] pcm, byte[] output) {
            if (_encoder == IntPtr.Zero) {
                return 0;
            }
            return Library.OpusEncodeFloat(
				_encoder,
                pcm,
                pcm.Length / (int)_channels,
                output,
                output.Length
                );
        }

		public ArraySegment<byte> Encode(float[] pcm)
		{
			int size = Library.OpusEncodeFloat(_encoder, pcm, _frameSizePerChannel, writePacket, writePacket.Length);
			if (size <= 1) //DTX. Negative already handled at this point
				return EmptyBuffer;
			else
				return new ArraySegment<byte>(writePacket, 0, size);
		}

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (_encoder == IntPtr.Zero) {
                    return;
                }
                Library.OpusEncoderDestroy(_encoder);
				_encoder = IntPtr.Zero;

                disposedValue = true;
            }
        }

        ~Encoder() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
