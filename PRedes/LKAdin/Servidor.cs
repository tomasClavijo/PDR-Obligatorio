using Protocolo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Servidor
    {
        static Socket socketServidor;
        EndPoint endPointServidor;
        ManejoDataSocket manejoDataSocket;
        int puerto;
        String ip;
        Controlador controlador;

        public Servidor(Controlador controlador)
        {
            this.controlador = controlador;
            Configurar();
            manejoDataSocket = new ManejoDataSocket(socketServidor);
            RecibirClientes();

        }

        public void Configurar()
        {
            puerto = 20000;
            ip = "127.0.0.1";
            socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPointServidor = new IPEndPoint(IPAddress.Parse(ip), puerto);
            socketServidor.Bind(endPointServidor);
            socketServidor.Listen(1000);
        }

        public void RecibirClientes()
        {
            while (true)
            {
                var socketCliente = socketServidor.Accept();
                Console.WriteLine("Cliente conectado");
                var clienteManejoSocket = new ManejoDataSocket(socketCliente);
                Thread t1 = new Thread(() => ManejarCliente(socketCliente, clienteManejoSocket, controlador));
                t1.IsBackground = true;
                t1.Start();
            }
        }

        static void ManejarCliente(Socket socketCliente, ManejoDataSocket manejo, Controlador control)
        {
            bool clienteConectado = true;
            while (clienteConectado)
            {
                
                
                    
                    byte[] header = manejo.Recive(9);
                    String tipo = Encoding.UTF8.GetString(header, 0, 3);
                    String opcion = Encoding.UTF8.GetString(header, 3, 2);

                    int largo = Int32.Parse(Encoding.UTF8.GetString(header, 5, 4));
                    byte[] mensajeBytes = manejo.Recive(largo);
                    String mensaje = Encoding.UTF8.GetString(mensajeBytes);
                    Console.WriteLine(tipo + "#" + opcion + "#" + largo + "#" + mensaje);
                    if (tipo == "REQ")
                    {
                        switch (opcion)
                        {
                            case "02":
                                var usuario = mensaje.Split('#');
                                String username = usuario[0];
                                String password = usuario[1];
                                String id = usuario[2];
                                Guid guid = control.AltaUsuario(username, password, id);
                                String resCodigo = "RES020016";
                                String resMensaje = guid.ToString();
                                byte[] resBytes = Encoding.UTF8.GetBytes(resCodigo);
                                byte[] resMensajeBytes = Encoding.UTF8.GetBytes(resMensaje);
                                manejo.Send(resBytes);
                                manejo.Send(resMensajeBytes);
                                break;
                        }
                    }
                

            }
            Console.WriteLine("Cliente desconectado");
        }


    }
}
