using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace udp
{
	class UDPClient
	{
		const int PORT = 9000;

		private UDPClient(string[] args)
		{
			// Validation of parameter list
			if(args.Length != 2) 
			{
				Console.WriteLine("Incorrect parameterlist.");
				Console.WriteLine("Please type-in valid hostname and valid input for reading uptime and loadavg");
				Environment.Exit(1);
			}

			// Creation of new UDPClient
			var client = new UdpClient();

			// Connecting to end point on host with the PORT 9000
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(args[0]), PORT);
			client.Connect(endPoint);

			// Read request sent to server
			byte[] DataSent = Encoding.ASCII.GetBytes(args[1]);
			client.Send(DataSent, DataSent.Length);

			// Output received data from server on screen
			byte[] DataReceived = client.Receive(ref endPoint);
			string data = Encoding.ASCII.GetString(DataReceived);
			Console.WriteLine(data);
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Client starts...");
			new UDPClient(args);
		}
	}
}

