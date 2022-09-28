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
        const int maximoEnBuffer = 1000;
        static Socket socketServidor;
        EndPoint endPointServidor;
        int puerto;
        String ip;
        Controlador controlador;

        public Servidor(Controlador controlador, string ip, int puerto)
        {
            this.ip = ip;
            this.puerto = puerto;
            this.controlador = controlador;
            Configurar();
            RecibirClientes();
        }

        public void Configurar()
        {
            socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPointServidor = new IPEndPoint(IPAddress.Parse(ip), puerto);
            socketServidor.Bind(endPointServidor);
            socketServidor.Listen(maximoEnBuffer);
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
                try
                {
                    List<String> recibo = EstructuraDeProtocolo.recibo(manejo);
                    String tipo = recibo[0];
                    String comando = recibo[1];
                    String mensajeString = recibo[3];
                    var mensajeDescomprimido = mensajeString.Split("|");
                    
                    String nombre;
                    String password; 
                    String userName;
                    Guid guid;
                    int respuestaLargo = 0;
                    String respuesta = String.Empty;

                    if (tipo == "REQ")
                    {
                        switch (comando)
                        {
                            case "02":
                                nombre = mensajeDescomprimido[0];
                                password = mensajeDescomprimido[1];
                                userName = mensajeDescomprimido[2];
                                (Guid, String) resultado = control.AltaUsuario(nombre, password, userName);
                                guid = resultado.Item1;
                                respuesta = guid.ToString()+"|"+ resultado.Item2;
                                tipo = "RES";

                                break;
                            case "03":
                                String descripcion = mensajeDescomprimido[0];
                                guid = Guid.Parse(mensajeDescomprimido[1]);

                                List<String> habilidades = new List<string>();
                                for (int i = 2; i < mensajeDescomprimido.Length; i++)
                                {
                                    habilidades.Add(mensajeDescomprimido[i]);
                                }
                                try
                                {
                                    Usuario usuario = control.BuscarUsuarioGuid(guid);
                                    control.CrearPerfil(usuario, descripcion, habilidades);
                                    respuesta = "Perfil creado correctamente";
                                }catch(ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }
                                tipo = "STT";
                                
                                break;
                            case "04":
                                try
                                {
                                    GestorArchivos gestor = new GestorArchivos(socketCliente);
                                    gestor.ReceiveFile();
                                    respuesta = "Imagen cargada correctamente";
                                }catch(ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }
                                tipo = "STT";
                                
                                break;
                            case "51":
                                nombre = mensajeString;
                                String perfiles = control.BuscarUsuarioNombre(nombre);
                                int largoPerfiles = perfiles.Length;
                                EstructuraDeProtocolo.envio("RES", "51", largoPerfiles, perfiles, manejo);
                                break;
                            case "52":

                                String perfilesHallados = control.BuscarPorHabilidad(mensajeDescomprimido);
                                int largoPerfilesHallados = perfilesHallados.Length;
                                EstructuraDeProtocolo.envio("RES", "52", largoPerfilesHallados, perfilesHallados, manejo);
                                break;
                            case "53":
                                String idP = mensajeString;
                                String perfilesId = control.BuscarPerfilId(idP);
                                int largoPerfilesId = perfilesId.Length;
                                EstructuraDeProtocolo.envio("RES", "51", largoPerfilesId, perfilesId, manejo);
                                break;
                            case "61":
                                String nombreUsuario = mensajeDescomprimido[0];
                                Perfil perfilReceptor = control.BuscarPerfilUserId(nombreUsuario);
                                String mensajeUsuario = mensajeDescomprimido[1];
                                Perfil perfilEmsior = control.BuscarPerfilGuid(Guid.Parse(mensajeDescomprimido[2]));
                                control.EnviarMensaje(mensajeUsuario, perfilEmsior, perfilReceptor);
                                respuesta = "Mensaje enviado";
                                EstructuraDeProtocolo.envio("RES", "61", respuesta.Length, respuesta, manejo);
                                break;
                            case "62":
                                Perfil perfilRecepcion = control.BuscarPerfilGuid(Guid.Parse(mensajeString));
                                String mensajes = control.MensajesRecibidos(perfilRecepcion);
                                EstructuraDeProtocolo.envio("RES", "52", mensajes.Length, mensajes, manejo);
                                break;

                        }
                        respuestaLargo = respuesta.Length;
                        EstructuraDeProtocolo.envio(tipo, comando, respuestaLargo, respuesta, manejo);
                    }
                }catch(SocketException e)
                {
                    clienteConectado = false;
                }
            }
            Console.WriteLine("Cliente desconectado");
        }


    }
}
