using LogsEnvios;
using Protocolo;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Servidor
    {
        const int maximoEnBuffer = 1000;
        static TcpListener tcpServidor;
        int puerto;
        String ip;
        String rutaFotos;
        Controlador controlador;
        bool corriendo = true;
        static Logs logs = new Logs();


        public Servidor(Controlador controlador, string ip, int puerto, string pictureFolder)
        {
            this.ip = ip;
            this.puerto = puerto;
            this.controlador = controlador;
            this.rutaFotos = pictureFolder;
            try
            {
                Configurar();
                Task recibir = RecibirClientesAsync();
                Task.WaitAny(recibir);
            }
            catch (SocketException)
            {
                Console.WriteLine("Ya hay un servidor corriendo en este socket\nPresione cualquier tecla para salir");
                Console.ReadLine();
            }

        }

        public void Configurar() { 
            
            var endPointServidor = new IPEndPoint(IPAddress.Parse(ip), puerto);
            tcpServidor = new TcpListener(endPointServidor);
            tcpServidor.Start();
        }

        public async Task RecibirClientesAsync()
        {
            while (corriendo)
            {
                var tcpCliente = await tcpServidor.AcceptTcpClientAsync();
                Console.WriteLine("Cliente conectado");
                var clienteManejoSocket = new ManejoDataSocket(tcpCliente);
                var task = Task.Run(async () => await 
                        ManejarCliente(tcpCliente, clienteManejoSocket, controlador, rutaFotos));
            }
        }

        public static void EnviarLog(string mensaje)
        {
            logs.EnvioLogs(mensaje);
        }
        
        static async Task ManejarCliente(TcpClient cliente, ManejoDataSocket manejo, Controlador control, String rutaImagenes)
        {
            bool clienteConectado = true;
            while (clienteConectado)
            {
                try
                {
                    List<String> recibo = await EstructuraDeProtocolo.reciboAsync(manejo);
                    String tipo = recibo[0];
                    String comando = recibo[1];
                    String mensajeString = recibo[3];
                    for (int i = 4; i < recibo.Count; i++)
                    {
                        mensajeString += "|" + recibo[i];
                    }

                    var mensajeDescomprimido = mensajeString.Split("|");

                    String nombre;
                    String password;
                    String userName;
                    Guid guid;
                    int respuestaLargo = 0;
                    String respuesta = String.Empty;
                    String mensajeLog = String.Empty;

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
                                    Guid resultado = control.AltaUsuario(nombre, password, userName);
                                    guid = resultado;
                                    respuesta = guid.ToString() + "|" + "Usuario creado correctamente";
                                    mensajeLog = "Se creo el usuario " + userName + " con el nombre " + nombre;
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    respuesta = Guid.Empty.ToString() + "|" + "Faltaron datos";
                                    mensajeLog = "Se intentó crear un usuario sin exito";
                                }
                                catch (ArgumentException)
                                {
                                    respuesta = Guid.Empty.ToString() + "|" + "El usuario ya existe";
                                    mensajeLog = "Se intentó crear un usuario sin exito";
                                    
                                }

                                tipo = "RES";

                                break;
                            case "03":
                                try
                                {
                                    mensajeLog = "Se creo el perfil: ";
                                    String descripcion = mensajeDescomprimido[0];
                                    guid = Guid.Parse(mensajeDescomprimido[1]);
                                    mensajeLog = "Descripción: " + descripcion;

                                    List<String> habilidades = new List<string>();
                                    for (int i = 2; i < mensajeDescomprimido.Length; i++)
                                    {
                                        mensajeLog = "\n Habilidad: " + mensajeDescomprimido[i];
                                        habilidades.Add(mensajeDescomprimido[i]);
                                    }
                                    Usuario usuario = control.BuscarUsuarioGuid(guid);
                                    control.CrearPerfil(usuario, descripcion, habilidades);
                                    respuesta = "Perfil creado correctamente";

                                }
                                catch (ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    respuesta = "Faltaron datos";
                                    mensajeLog = "Se intento crear un Perfil";
                                }
                                tipo = "STT";

                                break;
                            case "04":

                                try
                                {
                                    guid = Guid.Parse(mensajeDescomprimido[0]);
                                    Usuario usuario = control.BuscarUsuarioGuid(guid);
                                    GestorArchivos gestor = new GestorArchivos(cliente);
                                    PropiedadesArchivo pA = new PropiedadesArchivo();
                                    String rutaPerfilFoto = rutaImagenes + "\\" + usuario.Name + ".jpg";
                                    bool tieneFoto = pA.FileExists(rutaPerfilFoto);
                                    if (tieneFoto)
                                    {
                                        File.Delete(rutaPerfilFoto);
                                    }
                                    gestor.ReceiveFile(rutaImagenes + "\\" + usuario.UserName);
                                    respuesta = "Imagen cargada correctamente";
                                    mensajeLog = "El usuario " + usuario.UserName + " asocio foto de perfil";
                                }
                                catch (ArgumentException e)
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
                                mensajeLog = "Se realizó busqueda de perfil por id: " + idP;
                                try
                                {
                                    Perfil perfiABuscar = control.BuscarPerfilUserId(idP);
                                    String perfilesId = control.BuscarPerfilId(idP);
                                    PropiedadesArchivo pA = new PropiedadesArchivo();
                                    String rutaPerfilFoto = rutaImagenes + "\\" + perfiABuscar.Name + ".jpg";
                                    bool tieneFoto = pA.FileExists(rutaPerfilFoto);
                                    respuesta = perfilesId + "|" + tieneFoto.ToString();

                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    respuesta = "No se encontraron perfiles|False";
                                }
                                catch (ArgumentException e)
                                {
                                    respuesta = e.Message+"|False";
                                }

                                tipo = "RES";
                                break;
                            case "54":
                                String perfilId = mensajeDescomprimido[0];
                                try
                                {
                                    Perfil perfiABuscar = control.BuscarPerfilUserId(perfilId);
                                    String rutaPerfilFoto = rutaImagenes + "\\" + perfiABuscar.Name + ".jpg";
                                    GestorArchivos fileCommsHandler = new GestorArchivos(cliente);
                                    await fileCommsHandler.SendFileAsync(rutaPerfilFoto);
                                    respuesta = "OK";
                                }
                                catch (ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }
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
                                    mensajeLog = "Se envio el mensaje " + mensajeUsuario + " al usuario " + nombreUsuario;
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

                                    Perfil perfilRecepcion = control.BuscarPerfilGuid(Guid.Parse(mensajeDescomprimido[0]));
                                    if (mensajeDescomprimido[1] == "1")
                                    {
                                        respuesta = control.MensajesRecibidos(perfilRecepcion, true);
                                    }
                                    else
                                    {
                                        respuesta = control.MensajesRecibidos(perfilRecepcion, false);
                                    }
                                    mensajeLog = "El usuario" + perfilRecepcion.Name + " consulto sus mensajes";
                                }
                                catch (ArgumentException e)
                                {
                                    respuesta = e.Message;
                                }

                                tipo = "RES";
                                break;

                        }
                        respuestaLargo = respuesta.Length;

                        EnviarLog(mensajeLog);
                        await EstructuraDeProtocolo.envioAsync(tipo, comando, respuestaLargo, respuesta, manejo);
                    }
                }
                catch (SocketException)
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
