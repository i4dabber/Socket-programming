using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace udp
{
	class UDPServer
	{
		const int PORT = 9000;

		private UDPServer()
		{
			var server = new UdpClient(PORT);

			// Endpoint establishment
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);

			while (true) 
			{
				// Input from client
				byte[] DataReceived = server.Receive(ref endPoint); //listen on port 9000
				string data = Encoding.ASCII.GetString(DataReceived);

				Console.WriteLine ("Client input received: " + data);

				// Get info
				string measurement = GetMeasurement(data);

				// Send data back
				byte[] DataSent = Encoding.ASCII.GetBytes(measurement);
				server.Send(DataSent, DataSent.Length, endPoint);					
			}
		}

		public string GetMeasurement(string letter)
		{
			string fileLocation;

			switch(letter = letter.ToUpper()) 
			{
				case "U":
					fileLocation = "/proc/uptime";
					Console.WriteLine("Now sending uptime...");
					break;
				case "L":
					fileLocation = "/proc/loadavg";
					Console.WriteLine("Now sending loadavg...");
					break;
				default:
					Console.WriteLine("Incorrect input. Now sending error message...");
					return "Incorrect input. Acceptable inputs are l (L) or u (U)\n";
			}
			return "Reading from " + fileLocation + ": " + File.ReadAllText(fileLocation);
		}
	
		static void Main(string[] args)
		{
			Console.WriteLine("Server starts...");
			new UDPServer();
		}
	}
}
