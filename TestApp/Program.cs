using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UINVComms;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Opened port...");
			SerialDevice serial = new SerialDevice("COM3", 9600);
			serial.Open();


			Console.ReadKey();
			serial.Close();
		}
	}
}
