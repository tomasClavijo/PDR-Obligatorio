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
                    for (int i = 4; i < recibo.Count; i++)
                    {
                        mensajeString += "|"+recibo[i];
                    }
                    
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
                                try
                                {
                                    nombre = mensajeDescomprimido[0];
                                    password = mensajeDescomprimido[1];
                                    userName = mensajeDescomprimido[2];
                                    (Guid, String) resultado = control.AltaUsuario(nombre, password, userName);
                                    guid = resultado.Item1;
                                    respuesta = guid.ToString() + "|" + resultado.Item2;
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    respuesta = Guid.Empty.ToString() + "|" + "Faltaron datos";
                                }
                                
                                tipo = "RES";

                                break;
                            case "03":
                                try
                                {
                                    String descripcion = mensajeDescomprimido[0];
                                    guid = Guid.Parse(mensajeDescomprimido[1]);

                                    List<String> habilidades = new List<string>();
                                    for (int i = 2; i < mensajeDescomprimido.Length; i++)
                                    {
                                        habilidades.Add(mensajeDescomprimido[i]);
                                    }
                                    Usuario usuario = control.BuscarUsuarioGuid(guid);
                                    control.CrearPerfil(usuario, descripcion, habilidades);
                                    respuesta = "Perfil creado correctamente";
                                }catch(ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    respuesta = "Faltaron datos";
                                }
                                tipo = "STT";
                                
                                break;
                            case "04":
                                
                                try
                                {
                                    guid = Guid.Parse(mensajeDescomprimido[0]);
                                    Usuario usuario = control.BuscarUsuarioGuid(guid);
                                    GestorArchivos gestor = new GestorArchivos(socketCliente);
                                    gestor.ReceiveFile(usuario.UserName);
                                    respuesta = "Imagen cargada correctamente";
                                }catch(ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }
                                catch (Exception e)
                                {
                                    respuesta = e.Message;
                                }
                                tipo = "STT";
                                
                                break;
                            case "51":
                                nombre = mensajeString;
                                respuesta = control.BuscarUsuarioNombre(nombre);
                                tipo = "RES";
                                break;
                            case "52":
                                respuesta = control.BuscarPorHabilidad(mensajeDescomprimido);
                                tipo = "RES";
                                break;
                                
                            case "53":
                                String idP = mensajeString;
                                try
                                {
                                    Perfil perfiABuscar = control.BuscarPerfilUserId(idP);
                                    String perfilesId = control.BuscarPerfilId(idP);
                                    respuesta = perfilesId;
                                }catch(ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }

                                tipo = "RES";
                                break;
                            case "61":
                                try
                                {
                                    String nombreUsuario = mensajeDescomprimido[0];
                                    Perfil perfilReceptor = control.BuscarPerfilUserId(nombreUsuario);
                                    String mensajeUsuario = mensajeDescomprimido[1];
                                    Perfil perfilEmsior = control.BuscarPerfilGuid(Guid.Parse(mensajeDescomprimido[2]));
                                    control.EnviarMensaje(mensajeUsuario, perfilEmsior, perfilReceptor);
                                    respuesta = "Mensaje enviado";
                                }
                                catch (ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }

                                tipo = "STT";
                                break;
                            case "62":
                                try
                                {
                                    Perfil perfilRecepcion = control.BuscarPerfilGuid(Guid.Parse(mensajeString));
                                    respuesta = control.MensajesRecibidos(perfilRecepcion);
                                }
                                catch(ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }
                                
                                tipo = "RES";
                                break;

                        }
                        respuestaLargo = respuesta.Length;
                        EstructuraDeProtocolo.envio(tipo, comando, respuestaLargo, respuesta, manejo);
                    }
                }catch(SocketException e)
                {
                    clienteConectado = false;
                }
                catch (Exception)
                {
                    clienteConectado = false;
                }
            }
            Console.WriteLine("Cliente desconectado");
        }
        

    }
}
