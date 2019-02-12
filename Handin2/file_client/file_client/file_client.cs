using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{   class file_client
    {
        /// <summary>
        /// The BUFSIZE.
        /// </summary>
        private const int BUFSIZE = 1000;
        private const string APP = "FILE_CLIENT";

        /// <summary>
        /// Initializes a new instance of the <see cref="file_client"/> class.
        /// 
        /// file_client metoden opretter en peer-to-peer forbindelse
        /// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
        /// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
        /// Lukker alle streams og den modtagede fil
        /// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
        /// </summary>
        /// <param name='args'>
        /// Filnavn med evtuelle sti.
        /// </param>
        private file_client(String[] args)
        {   byte[] buff = new byte[BUFSIZE];
            string patchfil = args[0], fileName = LIB.extractFileName(patchfil);
            Transport tran = new Transport(BUFSIZE, APP); // oprettes transport objekt
            buff = Encoding.ASCII.GetBytes(patchfil);
            tran.send(buff, buff.Length);
            receiveFile(fileName, tran);
        }
        /// <summary>
        /// Receives the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='transport'>
        /// Transportlaget
        /// </param>
        private void receiveFile(String fileName, Transport transport)
        {
            byte[] buff = new byte[BUFSIZE];
            int sizebuff = transport.receive(ref buff);
            long lengtfil = BitConverter.ToInt64(buff, 0);
            // nedenunder ses filestream(), når den henter data fra h1, vil den gemmes i root/download.
            var fstream = new FileStream("/root/Downloads/" + fileName, FileMode.Create, FileAccess.Write);
            Console.WriteLine("the Fil to receive: {0}", lengtfil);
            while (lengtfil > 0) // benyttes en while loop til længden af filen
            {   sizebuff = transport.receive(ref buff);
                fstream.Write(buff, 0, sizebuff);
                lengtfil -= sizebuff;
            }
            fstream.Close();
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// First argument: Filname
        /// </param>
        public static void Main(string[] args)
        { new file_client(args);}
    }
}