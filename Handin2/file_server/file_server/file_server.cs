using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
    class file_server
    {
        /// <summary>
        /// The BUFSIZE
        /// </summary>
        private const int BUFSIZE = 1000;
        private const string APP = "FILE_SERVER";

        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// </summary>
        private file_server()
        {   byte[] buf = new byte[BUFSIZE];
            int sizeofbuf;
            string pathfill;
            long lenghtfill;
            Transport trp = new Transport(BUFSIZE, APP); //oprettes transport objekt.
            sizeofbuf = trp.receive(ref buf);
            Console.WriteLine(Encoding.ASCII.GetString(buf) + ", File of size: " + sizeofbuf);
            pathfill = Encoding.ASCII.GetString(buf, 0, sizeofbuf);
            Console.WriteLine(pathfill);
            lenghtfill = LIB.check_File_Exists(pathfill);
            if (lenghtfill == 0) //  hvis vaerdien er 0, så vil den udskrive fejl
            {   Console.WriteLine("FAILED!!!");
                return;
            }
            buf = BitConverter.GetBytes(lenghtfill);
            sendFile(pathfill, lenghtfill, trp);
        }

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='fileSize'>
        /// File size.
        /// </param>
        /// <param name='tl'>
        /// Tl.
        /// </param>
        private void sendFile(String fileName, long fileSize, Transport transport)
        {   byte[] buff;
            int Size;
            var fstream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            buff = BitConverter.GetBytes(fileSize);
            transport.send(buff, (int)Math.Floor(Math.Log10(fileSize)));
            buff = new byte[BUFSIZE];
            while (fileSize > 0) // der er benyttet while loop til at sende size.
            {   Size = fstream.Read(buff, 0, BUFSIZE);
                transport.send(buff, Size);
                fileSize -= Size;
            }
            fstream.Close();
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {   new file_server();}
    }
}