using Protocolo;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
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

                byte[] tiopoMensaje = manejo.Recive(3);
                String tipo = Encoding.UTF8.GetString(tiopoMensaje);
                byte[] comandMensaje = manejo.Recive(2);
                String comando = Encoding.UTF8.GetString(comandMensaje);
                byte[] largoMensaje = manejo.Recive(4);
                int largo = BitConverter.ToInt32(largoMensaje);
                byte[] mensaje = manejo.Recive(largo);
                String mensajeString = Encoding.UTF8.GetString(mensaje);
                var mensajeDescomprimido = mensajeString.Split("|");

                if (tipo == "REQ")
                {
                    switch (comando)
                    {
                        case "02":
                            String username = mensajeDescomprimido[0];
                            String password = mensajeDescomprimido[1];
                            String id = mensajeDescomprimido[2];
                            Guid guid = control.AltaUsuario(username, password, id);
                            int guidLargo = 36;
                            envio("RES", "02", guidLargo, guid.ToString(), manejo);
                            break;
                        case "03":
                            String descripcion = mensajeDescomprimido[0];
                            String guidUsuario = mensajeDescomprimido[1];
                            Guid usuarioGuid = Guid.Parse(guidUsuario);
                            List<String> habilidades = new List<string>();
                            for (int i = 2; i < mensajeDescomprimido.Length; i++)
                            {
                                habilidades.Add(mensajeDescomprimido[i]);
                            }
                            Usuario usuario = control.BuscarUsuarioGuid(usuarioGuid);

                            control.CrearPerfil(usuario, descripcion, habilidades);
                            envio("RES", "03", 2, "OK", manejo);

                            break;
                            
                    }
                }


            }
            Console.WriteLine("Cliente desconectado");
        }

        public static void envio(String tipo, String comando, int largo, String mensaje, ManejoDataSocket socket)
        {

            byte[] tipoEnBytes = Encoding.UTF8.GetBytes(tipo);
            byte[] codigoEnBytes = Encoding.UTF8.GetBytes(comando);
            byte[] largoEnBytes = BitConverter.GetBytes(largo);
            byte[] mensajeEnBytes = Encoding.UTF8.GetBytes(mensaje);

            socket.Send(tipoEnBytes);
            socket.Send(codigoEnBytes);
            socket.Send(largoEnBytes);
            socket.Send(mensajeEnBytes);
        }
    }
}
