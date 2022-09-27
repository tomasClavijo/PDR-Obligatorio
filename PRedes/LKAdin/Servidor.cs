using Communication;
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
                            StructuralMessage.envio("RES", "02", guidLargo, guid.ToString(), manejo);
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
                            StructuralMessage.envio("RES", "03", 2, "OK", manejo);
                            break;
                        case "04":
                            FileCommsHandler fileCommsHandler = new FileCommsHandler(socketCliente);
                            fileCommsHandler.ReceiveFile();
                            break;
                        case "51":
                            String nombre = mensajeString;
                            String perfiles = control.BuscarUsuarioNombre(nombre);
                            int largoPerfiles = perfiles.Length;
                            StructuralMessage.envio("RES", "51", largoPerfiles, perfiles, manejo);
                            break;
                        case "52":

                            String perfilesHallados = control.BuscarPorHabilidad(mensajeDescomprimido);
                            int largoPerfilesHallados = perfilesHallados.Length;
                            StructuralMessage.envio("RES", "52", largoPerfilesHallados, perfilesHallados, manejo);
                            break;
                        case "53":
                            String idP = mensajeString;
                            String perfilesId = control.BuscarPerfilId(idP);
                            int largoPerfilesId = perfilesId.Length;
                            StructuralMessage.envio("RES", "51", largoPerfilesId, perfilesId, manejo);
                            break;
                        case "61":
                            String nombreUsuario = mensajeDescomprimido[0];
                            Perfil perfilReceptor = control.BuscarPerfilUserId(nombreUsuario);
                            String mensajeUsuario = mensajeDescomprimido[1];
                            Perfil perfilEmsior = control.BuscarPerfilGuid(Guid.Parse(mensajeDescomprimido[2]));
                            control.EnviarMensaje(mensajeUsuario, perfilEmsior, perfilReceptor);
                            String respuesta = "Mensaje enviado";
                            StructuralMessage.envio("RES", "61", respuesta.Length, respuesta, manejo);
                            break;
                        case "62":
                            Perfil perfilRecepcion = control.BuscarPerfilGuid(Guid.Parse(mensajeString));
                            String mensajes = control.MensajesRecibidos(perfilRecepcion);
                            StructuralMessage.envio("RES", "52", mensajes.Length, mensajes, manejo);
                            break;

                    }
                }


            }
            Console.WriteLine("Cliente desconectado");
        }


    }
}
