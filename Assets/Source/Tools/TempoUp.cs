
namespace Tools
{
	public class TempoUp<T>
	{
		readonly int sizeofT = System.Runtime.InteropServices.Marshal.SizeOf(default(T));
		int channels;
		int skipGroup;

		int skipFactor;
		int sign = 0;
		int waveCnt;
		bool skipping;

		public void Begin(int channels, int changePerc, int skipGroup)
		{
			this.channels = channels;
			this.skipFactor = 100 / changePerc;
			this.skipGroup = skipGroup;
			sign = 0;
			skipping = false;
			waveCnt = 0;
		}

		public int Process(T[] s, T[] d)
		{
			if (sizeofT == 2)
			{
				return processShort(s as short[], d as short[]);
			}
			else
			{
				return processFloat(s as float[], d as float[]);
			}
		}

		// returns the number of samples required to skip in order to complete currently skipping wave
		public int End(T[] s)
		{
			if (!skipping)
			{
				return 0;
			}
			if (sizeofT == 2)
			{
				return endShort(s as short[]);
			}
			else
			{
				return endFloat(s as float[]);
			}
		}

		int processFloat(float[] s, float[] d)
		{
			int dPos = 0;
			if (channels == 1)
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						sign = 0;
					}

					if (!skipping)
					{
						d[dPos++] = s[i];
					}
				}
			}

			else if (channels == 2)
			{
				for (int i = 0; i < s.Length; i += 2)
				{
					if (s[i] + s[i + 1] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						sign = 0;
					}

					if (!skipping)
					{
						d[dPos++] = s[i];
						d[dPos++] = s[i + 1];
					}
				}
			}

			else
			{
				for (int i = 0; i < s.Length; i += channels)
				{
					var x = s[i] + s[i + 1];
					for (int j = 2; i < channels; j++)
					{
						x += s[i + j];
					}
					if (x < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						sign = 0;
					}

					if (!skipping)
					{
						d[dPos++] = s[i];
						d[dPos++] = s[i + 1];
						for (int j = 2; i < channels; j++)
						{
							d[dPos++] += s[i + j];
						}
					}
				}
			}

			return dPos / channels;
		}

		public int endFloat(float[] s)
		{
			if (channels == 1)
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						if (!skipping)
						{
							return i;
						}
						sign = 0;
					}
				}
			}

			else if (channels == 2)
			{
				for (int i = 0; i < s.Length; i += 2)
				{
					if (s[i] + s[i + 1] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						if (!skipping)
						{
							return i / 2;
						}
						sign = 0;
					}
				}
			}

			else
			{
				for (int i = 0; i < s.Length; i += channels)
				{
					var x = s[i] + s[i + 1];
					for (int j = 2; i < channels; j++)
					{
						x += s[i + j];
					}
					if (x < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						if (!skipping)
						{
							return i / channels;
						}
						sign = 0;
					}
				}
			}
			return 0;
		}

		int processShort(short[] s, short[] d)
		{
			int dPos = 0;
			if (channels == 1)
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						sign = 0;
					}

					if (!skipping)
					{
						d[dPos++] = s[i];
					}
				}
			}

			else if (channels == 2)
			{
				for (int i = 0; i < s.Length; i += 2)
				{
					if (s[i] + s[i + 1] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						sign = 0;
					}

					if (!skipping)
					{
						d[dPos++] = s[i];
						d[dPos++] = s[i + 1];
					}
				}
			}

			else
			{
				for (int i = 0; i < s.Length; i += channels)
				{
					var x = s[i] + s[i + 1];
					for (int j = 2; i < channels; j++)
					{
						x += s[i + j];
					}
					if (x < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						sign = 0;
					}

					if (!skipping)
					{
						d[dPos++] = s[i];
						d[dPos++] = s[i + 1];
						for (int j = 2; i < channels; j++)
						{
							d[dPos++] += s[i + j];
						}
					}
				}
			}

			return dPos / channels;
		}

		public int endShort(short[] s)
		{
			if (channels == 1)
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						if (!skipping)
						{
							return i;
						}
						sign = 0;
					}
				}
			}

			else if (channels == 2)
			{
				for (int i = 0; i < s.Length; i += 2)
				{
					if (s[i] + s[i + 1] < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						if (!skipping)
						{
							return i / 2;
						}
						sign = 0;
					}
				}
			}

			else
			{
				for (int i = 0; i < s.Length; i += channels)
				{
					var x = s[i] + s[i + 1];
					for (int j = 2; i < channels; j++)
					{
						x += s[i + j];
					}
					if (x < 0)
					{
						sign = -1;
					}
					else if (sign < 0)
					{
						waveCnt++;
						skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
						if (!skipping)
						{
							return i / channels;
						}
						sign = 0;
					}
				}
			}
			return 0;
		}
	}
}
