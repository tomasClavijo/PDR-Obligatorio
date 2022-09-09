using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LKdin
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciar Servidor");
            //1-Creamos un nuevo socjet
            var socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //2-Creamos un endpoint con IP local y puerto
            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
            //3-Asociacón entre el socket y el endpoint
            socketServidor.Bind(endpoint);
            //4-Ponemos el socket en modo escucha
            socketServidor.Listen(1000);
            //5-Aceptamos conexiones
            int contadorClientesConectados = 0;
            //contadorClientesConectados < cantClientes
            while (true)
            {
                var socketCliente = socketServidor.Accept(); //Metodo bloqueante
                contadorClientesConectados++;
                Console.WriteLine("Cliente conectado");
                Thread t1 = new Thread(() => ManejarCliente(socketCliente));
                t1.IsBackground = true;
                t1.Start();
            }

        }

        static string Menu(int eleccion)
        {
            string respuesta;
            switch (eleccion)
            {
                case 1:
                    respuesta = "Levantado";
                    break;
                case 2:
                    respuesta = "Cerrado";
                    break;
                default :
                    respuesta = "error";
                    break;
            }
            return respuesta;
        }

        static void ManejarCliente(Socket socket)
        {
            bool clienteConectado = true;
            while (clienteConectado)
            {
                try
                {
                    byte[] largoData = new byte[4];
                    socket.Receive(largoData);
                    int largo = BitConverter.ToInt32(largoData, 0);
                    byte[] buffer = new byte[largo];
                    int CantDatos = socket.Receive(buffer);
                    String mensaje = Encoding.UTF8.GetString(buffer);
                    int eleccion = Int32.Parse(mensaje);
                    byte[] data = Encoding.UTF8.GetBytes(Menu(eleccion));
                    socket.Send(data);
                    if (CantDatos == 0)
                    {
                        clienteConectado = false;
                    }
                    Thread.Sleep(100);
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
