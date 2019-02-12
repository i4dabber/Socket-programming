using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace tcp
{
    class file_server
    {
        /// <summary>
        /// The PORT
        /// </summary>
        const int PORT = 9000;
        // const string = "HANS";
        /// <summary>
        /// The BUFSIZE
        /// </summary>
        const int BUFSIZE = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// </summary>
        private file_server()
        {
            // Starts server and accepts a client
            Console.WriteLine("Starting server - waiting for client --->");

            //Opretter en socket
            TcpListener Server_Socket = new TcpListener(IPAddress.Any, PORT);
            TcpClient Client_Socket = default(TcpClient);

            //Starter serveren og connectecter til client vha Accept funktionen
            Server_Socket.Start();
            Client_Socket = Server_Socket.AcceptTcpClient();

            Console.WriteLine("Connected to client!");

            // getter client stream
            NetworkStream client = Client_Socket.GetStream();

            //Istantiering af filename til at læse og checke om file size eksistere
            string File_Name = LIB.readTextTCP(client);
            long File_Size = LIB.check_File_Exists(File_Name);

            // loop - der tjekker om filen er fundet.
            while (File_Size == 0)
            {
                string msg = "File '" + File_Name + "' not found";
                Console.WriteLine(msg);

                LIB.writeTextTCP(client, File_Size.ToString());

                File_Name = LIB.readTextTCP(client);
                File_Size = LIB.check_File_Exists(File_Name);

            }

            //Udskriver filnavn og størrelsen som er initialiseret
            Console.WriteLine("Filename: " + File_Name);
            Console.WriteLine("Size: " + File_Size);
            //Skriver texten til client stream
            LIB.writeTextTCP(client, File_Size.ToString());

            sendFile(File_Name, client);

            //Lukker programmet
            Client_Socket.Close();
            Server_Socket.Stop();
        }

       
        /// <param name='fName'>
        /// </param>
        /// <param name='fSize'> 
        /// </param>
        /// <param name='output'>
        /// </param>
        private void sendFile(String fName, NetworkStream output)
        {
            Console.WriteLine("Sending the requested file --->");

			FileStream fStream = new FileStream(fName, FileMode.Open, FileAccess.Read);
            Byte[] fSend = new Byte[BUFSIZE]; //bytes som 

            int bytes = 0;

            //Bliver ved med at sende indtil intet bytes er tilbage
            while ((bytes = fStream.Read(fSend, 0, fSend.Length)) > 0) 
            {
                //outputter clientstream byted som er sendt
				output.Write(fSend, 0, bytes); 

                Console.WriteLine($"Send {bytes} bytes");

            }
            Console.WriteLine("File has been sent <---");
        }


        /// <param name='args'>
        /// </param>
        static void Main(string[] args)
        {
            Console.WriteLine("Server starts...");
            //løkken der kører vores funktioner
            while (true)
            {
                new file_server();
            }
        }

    }
}


