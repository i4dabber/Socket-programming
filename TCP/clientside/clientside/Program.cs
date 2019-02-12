using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp
{
    class file_client
    {
        /// <summary>
        /// The PORT.
        /// </summary>
        const int PORT = 9000;
        /// <summary>
        /// The BUFSIZE.
        /// </summary>
        const int BUFSIZE = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="file_client"/> class.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments. First ip-adress of the server. Second the filename
        /// </param>
        private file_client(string[] args)
        {
            TcpClient client = new TcpClient();
            
            //Sikrer connection til det angivne Port
            client.Connect(args[0], PORT);
            Console.WriteLine($"'{args[0]}' Has been connected <---");

			//Requester client streamet
            NetworkStream server = client.GetStream(); 
            Console.WriteLine($"'{args[1]}'Filename requested <---");

            string File_Name = args[1];
            LIB.writeTextTCP(server, File_Name);

            long File_Size = LIB.getFileSizeTCP(server);

            //Sædvanlige check om filen eksistere eller ej
            while (File_Size == 0)
            {
                Console.WriteLine("File not found!");
				File_Name = Console.ReadLine();

                Console.WriteLine($"Requesting this file ---> '{File_Name}'");

                LIB.writeTextTCP(server, File_Name);
                File_Size = LIB.getFileSizeTCP(server);
            }

            Console.WriteLine("Size of file ---> " + File_Size);

            receiveFile(File_Name, server, File_Size);
        }

        
        /// <param name='fname'>
        /// File name.
        /// </param>
        /// <param name='output'>
        /// Network stream for reading from the server
        /// </param>
        private void receiveFile(String fname, NetworkStream output, long file_size)
        {
            fname = LIB.extractFileName(fname);

            //Opretter mappe og putter ind til denne directory
            string data_directory = "/root/Desktop/TilTorben/";
            Directory.CreateDirectory(data_directory);
            FileStream file = new FileStream(data_directory + fname, FileMode.Create, FileAccess.Write);
            
			byte[] data = new byte[BUFSIZE];

            
            int bytes_total = 0;
            int bytes_read;

            Console.WriteLine("Reading file! " + fname);

           //Løkke - der tjekker størrelsen af filen og skriver til bytes read - som læses fra i cw
            while (file_size > bytes_total)
            {
                bytes_read = output.Read(data, 0, data.Length);
                file.Write(data, 0, bytes_read);

                bytes_total += bytes_read;

                Console.WriteLine("Reading bytes = " + bytes_read.ToString() + "\t Total bytes read = " + bytes_total);

            }

            Console.WriteLine("File received");
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Client...");
            new file_client(args);
        }
    }
}
