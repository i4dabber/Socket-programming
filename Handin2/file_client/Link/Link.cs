using System;
using System.IO;
using System.Text;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
    /// <summary>
    /// Link.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The DELIMITE for slip protocol.
        /// </summary>
        const byte DELIMITER = (byte)'A';
        /// <summary>
        /// The buffer for link.
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// The serial port.
        /// </summary>
        SerialPort serialPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="link"/> class.
        /// </summary>
        public Link(int BUFSIZE, string APP)
        {
            // Create a new SerialPort object with default settings.
#if DEBUG
            if (APP.Equals("FILE_SERVER"))
            {
                serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);
            }
            else
            {
                serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);
            }
#else
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
#endif
            if (!serialPort.IsOpen)
                serialPort.Open();

            buffer = new byte[(BUFSIZE * 2)];

            // Uncomment the next line to use timeout
            //serialPort.ReadTimeout = 500;

            serialPort.ReadTimeout = 500;

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        /// <summary>
        /// Send the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public void send(byte[] buf, int size)
        {
            int index_buffer = 0;
            buffer[index_buffer++] = DELIMITER;
            for (int m = 0; m < size; m++, index_buffer++)
            {
                switch (buf[m])
                // Der er benyttet switch case metode, man kunne også bruge if-else statement.
                {
                    case (byte)'A':
                        buffer[index_buffer++] = (byte)'B';
                        buffer[index_buffer] = (byte)'C';
                        break;
                    case (byte)'B':
                        buffer[index_buffer++] = (byte)'B';
                        buffer[index_buffer] = (byte)'D';
                        break;
                    default:
                        buffer[index_buffer] = buf[m];
                        break;
                }
            }
            buffer[index_buffer++] = DELIMITER;
            serialPort.Write(buffer, 0, index_buffer); // Udskriver til serial port.
        }
        /// <summary>
        /// Receive the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public int receive(ref byte[] buf)
        {
            byte d = new byte();
            int index_buffer = 0;
            int SIZE;
            try
            {
                while ((d = (byte)serialPort.ReadByte()) != DELIMITER)
                { }
                while ((d = (byte)serialPort.ReadByte()) != DELIMITER)
                    buffer[index_buffer++] = d;
            }
            catch (TimeoutException)
            { return -1; }
            SIZE = index_buffer;
            index_buffer = 0;
            for (int b = 0; b < SIZE; b++, index_buffer++)
            {
                switch (buffer[b])
                {
                    case (byte)'B':
                        switch (buffer[++b])
                        // Der bliver tjekker om index insætter a/b
                        {
                            case (byte)'C':

                                buf[index_buffer] = (byte)'A';
                                break;
                            case (byte)'D':
                                buf[index_buffer] = (byte)'B';
                                break;
                            default:
                                index_buffer--;
                                break;
                        }
                        break;
                    default:
                        buf[index_buffer] = buffer[b];
                        break;
                }
            }
            return index_buffer;
        }
    }
}
