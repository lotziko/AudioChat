﻿using System;
using System.Runtime.InteropServices;

namespace Tools
{
	public static class OpusInfo
	{
		public const int MIN_OPUS_BITRATE = 6000;
		public const int MAX_OPUS_BITRATE = 510000;
	}

	public struct VoiceInfo
	{
		public SamplingRate SamplingRate { get; set; }
		public FrameDuration FrameDuration { get; set; }
		public NumChannels NumChannels { get; set; }
		public int Bitrate { get; set; }
		/// <summary>Uncompressed frame (data packet) size in samples.</summary>
		public int FrameDurationSamples
		{
			get { return (int)((long)SamplingRate * (long)FrameDuration / 1000000); }
		}
	}

	public enum SamplingRate : int
	{
		Frequency_8000 = 8000,
		Frequency_12000 = 12000,
		Frequency_16000 = 16000,
		Frequency_24000 = 24000,
		Frequency_48000 = 48000,
	}

	public enum FrameDuration : int
	{
		Frame2dot5ms = 2500,
		Frame5ms = 5000,
		Frame10ms = 10000,
		Frame20ms = 20000,
		Frame40ms = 40000,
		Frame60ms = 60000
	}

	public enum NumChannels : int
	{
		Mono = 1,
		Stereo = 2,
	}
}

namespace Tools.UnityOpus {
	public enum Delay
	{
		Delay2dot5ms = 5,
		Delay5ms = 10,
		Delay10ms = 20,
		Delay20ms = 40,
		Delay40ms = 80,
		Delay60ms = 120
	}

    public enum OpusApplication : int {
        VoIP = 2048,
        Audio = 2049,
        RestrictedLowDelay = 2051,
    }

    public enum OpusSignal : int {
        Auto = -1000,
        Voice = 3001,
        Music = 3002
    }

    public enum ErrorCode {
        OK = 0,
        BadArg = -1,
        BufferTooSmall = -2,
        InternalError = -3,
        InvalidPacket = -4,
        Unimplemented = -5,
        InvalidState = -6,
        AllocFail = -7,
    }

    public class Library {
        public const int maximumPacketDuration = 5760;

#if UNITY_ANDROID
        const string dllName = "unityopus";
#else
        const string dllName = "UnityOpus";
#endif

        [DllImport(dllName)]
        public static extern IntPtr OpusEncoderCreate(
            SamplingRate samplingFrequency,
            NumChannels channels,
            OpusApplication application,
            out ErrorCode error);

        [DllImport(dllName)]
        public static extern int OpusEncode(
            IntPtr encoder,
            short[] pcm,
            int frameSize,
            byte[] data,
            int maxDataBytes);

        [DllImport(dllName)]
        public static extern int OpusEncodeFloat(
            IntPtr encoder,
            float[] pcm,
            int frameSize,
            byte[] data,
            int maxDataBytes);

        [DllImport(dllName)]
        public static extern void OpusEncoderDestroy(
            IntPtr encoder);

        [DllImport(dllName)]
        public static extern int OpusEncoderSetBitrate(
            IntPtr encoder,
            int bitrate);

        [DllImport(dllName)]
        public static extern int OpusEncoderSetComplexity(
            IntPtr encoder,
            int complexity);

        [DllImport(dllName)]
        public static extern int OpusEncoderSetSignal(
            IntPtr encoder,
            OpusSignal signal);

        [DllImport(dllName)]
        public static extern IntPtr OpusDecoderCreate(
            SamplingRate samplingFrequency,
            NumChannels channels,
            out ErrorCode error);

        [DllImport(dllName)]
        public static extern int OpusDecode(
            IntPtr decoder,
            byte[] data,
            int len,
            short[] pcm,
            int frameSize,
            int decodeFec);

        [DllImport(dllName)]
        public static extern int OpusDecodeFloat(
            IntPtr decoder,
            byte[] data,
            int len,
            float[] pcm,
            int frameSize,
            int decodeFec);

        [DllImport(dllName)]
        public static extern void OpusDecoderDestroy(
            IntPtr decoder);

        [DllImport(dllName)]
        public static extern void OpusPcmSoftClip(
            float[] pcm,
            int frameSize,
            NumChannels channels,
            float[] softclipMem);
    }
}
