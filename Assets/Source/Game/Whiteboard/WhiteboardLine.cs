using ExitGames.Client.Photon;
using UnityEngine;

namespace AudioChat
{
	public struct WhiteboardLine
	{
		public static readonly byte[] memLine = new byte[4 * 4 + 2]; //18

		public WhiteboardInstrument Instrument { get; }
		public Vector4 Points { get; }
		public float Magnitude { get; }

		public WhiteboardLine(WhiteboardInstrument instrument, Vector2 first, Vector2 second)
		{
			Instrument = instrument;
			Points = new Vector4(first.x, first.y, second.x, second.y);
			Magnitude = Vector2.Distance(first, second);
		}

		public static short Serialize(StreamBuffer outStream, object customObject)
		{
			WhiteboardLine line = (WhiteboardLine)customObject;

			lock (memLine)
			{
				int index = 0;

				Protocol.Serialize((short)line.Instrument, memLine, ref index);

				Protocol.Serialize(line.Points.x, memLine, ref index);
				Protocol.Serialize(line.Points.y, memLine, ref index);
				Protocol.Serialize(line.Points.z, memLine, ref index);
				Protocol.Serialize(line.Points.w, memLine, ref index);

				outStream.Write(memLine, 0, 18);
			}

			return 18;
		}

		public static object Deserialize(StreamBuffer inStream, short length)
		{
			short instrument;
			Vector2 first;
			Vector2 second;
			Color color = Color.white;

			lock (memLine)
			{
				inStream.Read(memLine, 0, 18);
				int index = 0;

				Protocol.Deserialize(out instrument, memLine, ref index);

				Protocol.Deserialize(out first.x, memLine, ref index);
				Protocol.Deserialize(out first.y, memLine, ref index);
				Protocol.Deserialize(out second.x, memLine, ref index);
				Protocol.Deserialize(out second.y, memLine, ref index);
			}

			return new WhiteboardLine((WhiteboardInstrument)instrument, first, second);
		}
	}
}
