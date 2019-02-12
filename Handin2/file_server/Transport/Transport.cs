using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
    /// <summary>
    /// Transport.
    /// </summary>
    public class Transport
    {
        /// <summary>
        /// The link.
        /// </summary>
        private Link link;
        /// <summary>
        /// The 1' complements checksum.
        /// </summary>
        private Checksum checksum;
        /// <summary>
        /// The buffer.
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// The seq no.
        /// </summary>
        private byte seqNo;
        /// <summary>
        /// The old_seq no.
        /// </summary>
        private byte old_seqNo;
        /// <summary>
        /// The error count.
        /// </summary>
        private int errorCount;
        /// <summary>
        /// The DEFAULT_SEQNO.
        /// </summary>
        private const int DEFAULT_SEQNO = 2;
        /// <summary>
        /// The data received. True = received data in receiveAck, False = not received data in receiveAck
        /// </summary>
        private bool dataReceived;
        /// <summary>
        /// The number of data the recveived.
        /// </summary>
        private int recvSize = 0;
        // Her angiver vi defaulft vaerdier til sender og receiver
        int counter_send = 0;
        int counter_received = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transport"/> class.
        /// </summary>
        public Transport(int BUFSIZE, string APP)
        {
            link = new Link(BUFSIZE + (int)TransSize.ACKSIZE, APP);
            checksum = new Checksum();
            buffer = new byte[BUFSIZE + (int)TransSize.ACKSIZE];
            seqNo = 0;
            old_seqNo = DEFAULT_SEQNO;
            errorCount = 0;
            dataReceived = false;
        }

        /// <summary>
        /// Receives the ack.
        /// </summary>
        /// <returns>
        /// The ack.
        /// </returns>
        private bool receiveAck()
        {
            recvSize = link.receive(ref buffer);
            dataReceived = true;

            if (recvSize == (int)TransSize.ACKSIZE)
            {   dataReceived = false;
                if (!checksum.checkChecksum(buffer, (int)TransSize.ACKSIZE) ||
                    buffer[(int)TransCHKSUM.SEQNO] != seqNo ||
                    buffer[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
                {  return false; }
                seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            }
            return true;
        }

        /// <summary>
        /// Sends the ack.
        /// </summary>
        /// <param name='ackType'>
        /// Ack type.
        /// </param>
        private void sendAck(bool ackType)
        {
            byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
            ackBuf[(int)TransCHKSUM.SEQNO] = (byte)
                (ackType ? (byte)buffer[(int)TransCHKSUM.SEQNO] : (byte)(buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            ackBuf[(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
            checksum.calcChecksum(ref ackBuf, (int)TransSize.ACKSIZE);
            if ((++errorCount) == 3) // der er brugt if-statement til at tælle antal error.
            {   ackBuf[2]++;
                Console.WriteLine($"Byte: (2) --> {errorCount} Transmitted ACK package)");
            }
            link.send(ackBuf, (int)TransSize.ACKSIZE);
        }

        /// <summary>
        /// Send the specified buffer and size.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public void send(byte[] buf, int size)
        {
            int bufflenght = buffer.Length - (int)TransSize.ACKSIZE;
            var sendsize = (int)Math.Min(bufflenght, size);
            var index_fil = 0;
            while (true)
            {
                do
                {
                    buffer[(int)TransCHKSUM.SEQNO] = seqNo; // Seq number
                    buffer[(int)TransCHKSUM.TYPE] = (int)TransType.DATA;
                    Array.Copy(buf, 
                        index_fil, 
                        buffer, (int)TransSize.ACKSIZE, sendsize);
                    checksum.calcChecksum(ref buffer, sendsize + (int)TransSize.ACKSIZE);

                    // Fejltjek 
                    if ((++errorCount) == 3) //Hvis at errorCount bliver 3
                    {   buffer[2]++;
                        // Udskriver antal fejl 
                        Console.WriteLine($" OBS byte 2 --> {errorCount},(Transmission)");
                    }
                    link.send(buffer, sendsize + (int)TransSize.ACKSIZE);
                } while (receiveAck() == false || dataReceived == true);

                // Udskriver i consol: sendsize og counter_send
                Console.WriteLine($"Size sent: {sendsize}, {counter_send}");
                counter_send++; // conter_send inkrementeres
                old_seqNo = DEFAULT_SEQNO; // Opdaterer 
                size -= sendsize;
                sendsize = Math.Min(bufflenght, size);
                // Fejltjek: hvis sendsize er mindre eller lig 0 kaldes break. 
                if (sendsize <= 0) 
                    break;

                index_fil += sendsize;
            }
        }

        /// <summary>
        /// Receive the specified buffer.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        public int receive(ref byte[] buf)
        {
            int size_return;
            bool checksum_done = false; // skal initieres som false 
            do
            {   do
                {   size_return = link.receive(ref buffer);
                } while (size_return == -1);
                if (checksum_done = checksum.checkChecksum(buffer, size_return)) // Her tjekkes om checksom kan kaldes Done. 
                {  if (old_seqNo != buffer[(int)TransCHKSUM.SEQNO]) // hvis ja 
                    {   sendAck(true);
                        old_seqNo = buffer[(int)TransCHKSUM.SEQNO];
                    }
                    else
                    {   sendAck(false);}
                }
                else
                {   sendAck(true);}
            } while (!checksum_done);
            Console.WriteLine($"received the data: {size_return}, {counter_received}"); // Her udskrives data til consollen 
            counter_received++; //inkrementering af tælleren 
            size_return -= (int)TransSize.ACKSIZE; 
            Array.Copy(buffer, (int)TransSize.ACKSIZE, buf, 0, size_return);
            return size_return; // hvorefter at størrelsen returneres 
        }
    }
}