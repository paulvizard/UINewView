using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UINVComms
{
	public class SerialDevice
	{
		private SerialPort _port;
		private KISSComms _kissComms;
		private bool _kissMode = false;

		public SerialDevice(string port, int baudRate)
		{
			Console.WriteLine("Creating serial device: " + port);
			_port = new SerialPort(port, baudRate, Parity.None, 8);
			_kissComms = new KISSComms();
		}

		

		public bool Open()
		{
			Console.WriteLine("Opening port");
			_port.Handshake = Handshake.RequestToSend;
			_port.ReceivedBytesThreshold = 1;

			try
			{
				_port.Open();

				_port.DataReceived += DataReceived;
				_port.ErrorReceived += _port_ErrorReceived;
			}
			catch (Exception ex )
			{
				Console.WriteLine("Failed to open port: " + ex.Message );
				return false;
			}


			Console.WriteLine("Entering KISS mode");
			_kissMode = false;
			var data = _kissComms.ExitKISSMode();
			_port.Write(data, 0, data.Length);

			data = _kissComms.EnterKISSMode();
			_port.Write(data, 0, data.Length);
			_kissMode = true;


			Console.WriteLine("Port listening...");

			return true;
		}

		void _port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			_kissMode = false;

			Console.WriteLine("Exiting KISS mode");
			var data = _kissComms.ExitKISSMode();
			_port.Write(data, 0, data.Length);

			_port.Close();

		}

		void DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			while (_port.BytesToRead > 0)
			{
				var len = _port.BytesToRead;
				var data = new byte[len];
				_port.Read(data, 0, len);

				if (!_kissMode)
				{
					Console.WriteLine("Not in KISS mode yet...");
					return;
				}

				foreach(var b in data)
				{
					Console.Write("{0:X2}", b);
				}


				_kissComms.AddBytesToBuffer(data);
			}
		}
	}
}
