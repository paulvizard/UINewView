using System;
using System.Collections.Generic;
using System.Text;

namespace UINVComms
{
	public class KISSComms
	{
		public const byte KISS_FEND = 0xc0;  // Frame End
		public const byte KISS_FESC = 0xdb;  // Frame Escape
		public const byte KISS_TFEND = 0xdc;  // Transposed Frame End
		public const byte KISS_TFESC = 0xdd;  // Transposed Frame Escape
		public const byte KISS_DATA = 0x00;
		public const byte KISS_TXDELAY = 0x01;
		public const byte KISS_PERSISTENCE = 0x02;
		public const byte KISS_SLOTTIME = 0x03;
		public const byte KISS_TXTAIL = 0x04;
		public const byte KISS_FULLDUPLEX = 0x05;
		public const byte KISS_SETHARDWARE = 0x06;
		public const byte KISS_RETURN = 0xff;

		private byte[] _buffer;

		private int _nextBufferPos = 0;
		private bool _gotEscape = false;
		private List<KISSPacket> _packets = new List<KISSPacket>();




		public void AddBytesToBuffer(byte[] bytes)
		{
			if (_buffer == null)
			{
				_buffer = new byte[1024];
			}

			foreach (var b in bytes)
			{
				byte value = b;
				if (b == KISSComms.KISS_FESC)
					_gotEscape = true;

				// un-escape any ESC or FEND bytes
				if (_gotEscape)
				{
					if (value == KISS_TFESC)
						value = KISS_FESC;
					else if (value == KISS_TFEND)
						value = KISS_FEND;

					_gotEscape = false;
				}

				// Save the value to the buffer
				_buffer[_nextBufferPos++] = b;

				// Got a frame, so create a KISSPacket
				if (value == KISS_FEND)
				{
					Console.WriteLine("\n\nGot a packet");
					var packet = new KISSPacket(_buffer, _nextBufferPos - 1);
					packet.DecodeKISSPacket();
					_packets.Add(packet);	
					_nextBufferPos = 0;
				}
			}
		}

		public byte[] EnterKISSMode()
		{
			return Encoding.ASCII.GetBytes("\r\nKISS ON\r\nRESTART\r\n");
		}

		public byte[] ExitKISSMode()
		{
			return new[] { KISS_FEND, KISS_RETURN, KISS_FEND };
		}

	}
}
