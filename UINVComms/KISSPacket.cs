using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UINVComms
{
	public class KISSPacket
	{
		private byte[] _buffer;
		private bool _decoded = false;

		private string _destinationFrame;

		public string DestinationFrame
		{
			get { return _destinationFrame; }
		}

		private string _sourceFrame;

		public string SourceFrame
		{
			get { return _sourceFrame; }
		}

		

		public KISSPacket(byte[] bytes, int length)
		{
			_buffer = new byte[length];
			Buffer.BlockCopy(bytes, 0, _buffer, 0, length);
		}

		public bool DecodeKISSPacket()
		{
			Console.WriteLine("Decoding packet");
			int pos = 0;

			if (_buffer.Length < 15)
			{
				//Console.WriteLine("Packet too small");
				return false;
			}

			if ((_buffer[pos++] & 1) == 1)
			{
				Console.WriteLine("Compressed FlexNet packet - ignoring");
				return false;
			}

			// Extract the destination frame
			var temp = new byte[6];
			int tempPos = 0;
			for (int i = 0; i < 6; i++)
			{
				if ((_buffer[pos + i] & 0xfe) != 0x40)
					temp[tempPos++] = (byte) (_buffer[pos + i] >> 1);
				else
					temp[tempPos++] = 0x2e;	// Place holder for skipped character
			}

			string destination = Encoding.ASCII.GetString(temp, 0, tempPos);
			var ssid = (_buffer[pos + 6] & 0x1e) >> 1;
			if (ssid > 0)
			{
				destination += "-" + ssid;
			}

			pos += 7;
			
			// Extract the source frame
			tempPos = 0;
			for (int i = 0; i < 6; i++)
			{
				if ((_buffer[pos + i] & 0xfe) != 0x40)
					temp[tempPos++] = (byte) (_buffer[pos + i] >> 1);
			}

			string source = Encoding.ASCII.GetString(temp, 0, tempPos);
			ssid = (_buffer[pos + 6] & 0x1e) >> 1;
			if (ssid > 0)
			{
				source += "-" + ssid;
			}

			pos += 7;

			Console.WriteLine("Destination: " + destination);
			Console.WriteLine("Source: " + source);


			_decoded = true;
			return false;
		}

		public byte[] EncodeKISSPacket()
		{
			return null;
		}
	}
}
