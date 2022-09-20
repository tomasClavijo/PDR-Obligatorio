using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LKdin
{
    public class Servidor
    {
        Socket socketServidor;
        EndPoint endPointServidor;
        int puerto;
        String ip;

        public Servidor()
        {
            Configurar();
            RecibirClientes();
        }

        public void Configurar()
        {
            puerto = 20000;
            ip = "127.0.0.1";
            var socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Parse(ip), puerto);
            socketServidor.Bind(endpoint);
            socketServidor.Listen(1000);
        }

        public void RecibirClientes()
        {
            while (true)
            {
                var socketCliente = socketServidor.Accept();
                Console.WriteLine("Cliente conectado");
                Thread t1 = new Thread(() => ManejarCliente(socketCliente));
                t1.IsBackground = true;
                t1.Start();
            }
        }

        static void ManejarCliente(Socket socketCliente)
        {
            bool clienteConectado = true;
            while (clienteConectado)
            {
                try
                {
                    byte[] largoData = new byte[4];
                    socketCliente.Receive(largoData);
                    int largo = BitConverter.ToInt32(largoData, 0);
                    byte[] buffer = new byte[largo];
                    int CantDatos = socketCliente.Receive(buffer);
                    String mensaje = Encoding.UTF8.GetString(buffer);
                    int eleccion = Int32.Parse(mensaje);
                    if (CantDatos == 0)
                    {
                        clienteConectado = false;
                    }
                }
                catch (SocketException e)
                {
                    clienteConectado = false;
                }
                catch (FormatException e)
                {
                    clienteConectado = false;
                }

            }
            Console.WriteLine("Cliente desconectado");
        }


    }
}
