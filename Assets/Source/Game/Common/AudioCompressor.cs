using System;
using System.Linq;
using Tools;
using Tools.UnityOpus;

namespace AudioChat
{
	public class AudioCompressor
	{
		private float[] _floatBuffer;
		private byte[] _byteBuffer;

		private Encoder _encoder;
		private Decoder _decoder;

		public AudioCompressor(VoiceInfo info)
		{
			//int sampleSize = (AudioInfo.FREQUENCY / 1000 * AudioInfo.SAMPLE_SIZE * AudioInfo.CHANNELS);
			//_byteBuffer = new byte[sampleSize * 4];

			_floatBuffer = new float[info.FrameDurationSamples];
			_encoder = new Encoder(info.SamplingRate, info.NumChannels, info.Bitrate, OpusApplication.VoIP, Delay.Delay20ms);
			_decoder = new Decoder(info.SamplingRate, info.NumChannels);
		}

		/// <summary>
		/// Make sure not to store this data
		/// </summary>
		public ArraySegment<byte> Compress(float[] data)
		{
			return _encoder.Encode(data);
		}

		/// <summary>
		/// Make sure not to store this data
		/// </summary>
		public float[] Decompress(byte[] data)
		{
			int length = _decoder.Decode(data, data.Length, _floatBuffer);
			return _floatBuffer;
		}

		//private SpeexEncoder _encoder = new SpeexEncoder(BandMode.Narrow);
		//private SpeexDecoder _decoder = new SpeexDecoder(BandMode.Narrow);

		//public AudioCompressor()
		//{
		//	int dataSize = AudioInfo.FREQUENCY / 1000 * AudioInfo.SAMPLE_SIZE * AudioInfo.CHANNELS;
		//	_inputData = new short[dataSize];
		//	_inputCompressed = new byte[dataSize * 2];
		//}

		//public byte[] Compress(float[] data)
		//{
		//	data.ToShortArray(_inputData, 0, data.Length);
		//	int length = _encoder.Encode(_inputData, 0, _inputData.Length, _inputCompressed, 0, _inputCompressed.Length);
		//	return _inputCompressed.Take(length).ToArray();
		//}

		//public float[] Decompress(byte[] data)
		//{
		//	int length = _decoder.Decode(data, 0, data.Length, _inputData, 0, false);
		//	short[] output = _inputData.Take(length).ToArray();
		//	float[] fOutput = new float[output.Length];
		//	output.ToFloatArray(fOutput, output.Length);
		//	return fOutput;
		//}
	}
}
