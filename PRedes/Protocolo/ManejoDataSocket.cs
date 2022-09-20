using System;
using System.Net.Sockets;

namespace Protocolo
{
    public class ManejoDataSocket
    {
        private readonly Socket _socket;
        public ManejoDataSocket(Socket socket)
        {
            _socket = socket;
        }
        public void Send(byte[] buffer)
        {
            int offset = 0;
            int size = buffer.Length;
            while (offset < size)
            {
                int sent = _socket.Send(buffer, offset - size, SocketFlags.None);
                if (sent == 0)
                {
                    throw new SocketException();
                }
                offset += sent;
            }
        }

        public byte[] Recive(int size)
        {
            byte[] buffer = new byte[size];
            int offset = 0;
            while (offset < size)
            {
                int recived = _socket.Receive(buffer, offset - size, SocketFlags.None);
                if (recived == 0)
                {
                    throw new SocketException();
                }
                offset += recived;
            }
            return buffer;
        }
    }
}
