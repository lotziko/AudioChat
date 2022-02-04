using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
	public static class Extensions
	{
		/// <summary>
		/// Returns the normalized position of the AudioSource on its AudioClip
		/// </summary>
		public static float Position(this AudioSource source)
		{
			return (float)source.timeSamples / source.clip.samples;
		}

		/// <summary>
		/// Adds the given key to the dictionary with a value if the key
		/// is not present before.
		/// </summary>
		/// <param name="t">The key to check and add</param>
		/// <param name="k">The value if the key is added</param>
		public static void EnsureKey<T, K>
		(this Dictionary<T, K> dict, T t, K k)
		{
			if (!dict.ContainsKey(t))
				dict.Add(t, k);
		}

		/// <summary>
		/// Ensures a key and value pair are in the dictionary
		/// </summary>
		/// <param name="t">The key to ensure</param>
		/// <param name="k">The value associated with the key</param>
		public static void EnsurePair<T, K>
		(this Dictionary<T, K> dict, T t, K k)
		{
			if (dict.ContainsKey(t))
				dict[t] = k;
			else
				dict.Add(t, k);
		}

		public static void ToShortArray(this float[] input, short[] output, int offset, int len)
		{
			for (int i = 0; i < len; ++i)
			{
				output[i] = (short)Mathf.Clamp((int)(input[i + offset] * 32767.0f), short.MinValue, short.MaxValue);
			}
		}

		public static void ToFloatArray(this short[] input, float[] output, int length)
		{
			for (int i = 0; i < length; ++i)
			{
				output[i] = input[i] / (float)short.MaxValue;
			}
		}
	}
}
