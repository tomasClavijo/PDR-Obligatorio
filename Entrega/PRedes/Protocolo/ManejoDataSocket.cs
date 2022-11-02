using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Protocolo
{
    public class ManejoDataSocket
    {
        private readonly Socket _socket;

        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public ManejoDataSocket(TcpClient tcp)
        {
            _client = tcp;
            _stream = _client.GetStream();
        }
        

        public void Close()
        {
            _stream.Close();
        }

        public async Task SendAsync(byte[] buffer)
        {
            await _stream.WriteAsync(buffer);
        }

        public async Task<byte[]> ReciveAsync(int size)
        {
            byte[] buffer = new byte[size];
            int offset = 0;
            while (offset < size)
            {
                int recived =  await _stream.ReadAsync(buffer, offset, size - offset);
                if (recived == 0)
                {
                    Close();
                    throw new SocketException();
                }
                offset += recived;
            }
            return buffer;
        }
    }
}
