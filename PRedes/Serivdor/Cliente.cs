using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Serivdor
{
    internal class Cliente
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicia cliente");
            var socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpointLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            socketCliente.Bind(endpointLocal);
            var endpointRemoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
            socketCliente.Connect(endpointRemoto);
            String msj = "";
            while (msj != "7")
            {
                Console.WriteLine("1. Conexión de un cliente al servidor");
                Console.WriteLine("2. Dar de alta a un usuario");
                Console.WriteLine("3. Crear perfil de trabajo de usuario");
                Console.WriteLine("4. Asociar foto al perfil");
                Console.WriteLine("5. Consultar perfiles existentes");
                Console.WriteLine("6. Enviar y recibir mensajes");
                Console.WriteLine("7. Fin de conexion");

                msj = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(msj);
                //socketCliente.Send(data, data.Length, SocketFlags.None);
                socketCliente.Send(BitConverter.GetBytes(data.Length));
                socketCliente.Send(data);

                byte[] buffer = new byte[40];
                int CantDatos = socketCliente.Receive(buffer);
                String mensaje = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(mensaje);
            }
            socketCliente.Shutdown(SocketShutdown.Both);
            socketCliente.Close();
            Console.ReadLine();
        }
    }
}
