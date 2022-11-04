using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Protocolo;
using System.Threading;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace ClienT
{
    public class Cliente
    {

        static TcpClient _cliente;
        Guid session;
        ManejoDataSocket manejoDataSocket;
        String rutaImagen;

        public Cliente(String ServerIp, int ServerPort, String LocalIp, String rutaImagenes)
        {
            try
            {
                rutaImagen = rutaImagenes;
            }
            catch (SocketException)
            {
                Console.WriteLine("El cliente no pudo conectarse al servidor\nPresione cualquier tecla para salir");
                Console.ReadLine();
            }
            
        }

        public async Task ConfigurarConexion(String ServerIp, int ServerPort, String LocalIp)
        {
            var endPointLocal = new IPEndPoint(IPAddress.Parse(LocalIp), 0);
            _cliente = new TcpClient(endPointLocal);
            await _cliente.ConnectAsync(IPAddress.Parse(ServerIp), ServerPort);
            manejoDataSocket = new ManejoDataSocket(_cliente);
            await InterfazAsync();
            CerrarConexion();
        }

        public void Menu()
        {
            Console.WriteLine("1. Conexión de un cliente al servidor");
            Console.WriteLine("2. Dar de alta a un usuario");
            Console.WriteLine("3. Crear perfil de trabajo de usuario");
            Console.WriteLine("4. Asociar foto al perfil");
            Console.WriteLine("5. Consultar perfiles existentes");
            Console.WriteLine("6. Enviar y recibir mensajes");
            Console.WriteLine("7. Fin de conexion");
        }

        public List<String> CargarHabilidades()
        {

            String habilidad;
            List<String> habilidades = new List<string>();
            do
            {
                Console.WriteLine("Ingrese una habilidad o (X) para salir");
                habilidad = Console.ReadLine();
                if (habilidad != "X" && habilidad != "x" && habilidad != "")
                {
                    habilidades.Add(habilidad);
                }
            } while ((habilidad != "X" && habilidad != "x") || habilidades.Count == 0);
            return habilidades;
        }

        public async Task InterfazAsync()
        {
            
            string opcion = "#";
            
            try
            {
                
                while (!opcion.Contains("7"))
                {

                    Menu();
                    opcion = Console.ReadLine();
                    Task tareaUsuario = Task.CompletedTask;
                    switch (opcion)
                    {
                        case "1":
                            Console.WriteLine("Conectando al servidor a través de datos asociados...");
                            Thread.Sleep(10000);
                            Console.WriteLine("Conexión establecida");
                            break;
                        case "2":
                            Console.WriteLine("Ingrese su nombre");
                            String username = Console.ReadLine();
                            Console.WriteLine("Ingrese su username");
                            String userID = Console.ReadLine();
                            Console.WriteLine("Ingrese contraseña");
                            String password = Console.ReadLine();
                            tareaUsuario = AltaUsuarioAsync(username, password, userID);
                            break;
                        case "3":
                            Console.WriteLine("Crear su perfil de usuario");
                            Console.WriteLine("Ingrese descripcion de su perfil");
                            String descripcion = Console.ReadLine();
                            List<String> habilidades = CargarHabilidades();
                            tareaUsuario = CrearPerfilAsync(descripcion, habilidades);
                            break;
                        case "4":
                            Console.WriteLine("Asociar foto al perfil, \n Introduzca ruta de la misma");
                            string ruta = Console.ReadLine();
                            tareaUsuario = AsociarFotoAsync(ruta);
                            break;
                        case "5":
                            Console.WriteLine("Consultar perfiles existentes");
                            Console.WriteLine("1)Consultar por nombre \n2)Consultar por palabra clave" +
                                " \n3)Consultar por username");
                            int buscarPor = Int32.Parse(Console.ReadLine());
                            switch (buscarPor)

                            {
                                case 1:
                                    Console.WriteLine("Introduzca nombre del usuario");
                                    String nombre = Console.ReadLine();
                                    Console.WriteLine("Perfiles:");
                                    tareaUsuario = BuscarPorNombreAsync(nombre);
                                    break;
                                case 2:
                                    List<String> habilidadesABuscar = CargarHabilidades();
                                    Console.WriteLine("Perfiles:");
                                    tareaUsuario = BuscarPorHabilidadesAsync(habilidadesABuscar);
                                    break;
                                case 3:
                                    Console.WriteLine("Introduzca el username a buscar");
                                    String id = Console.ReadLine();
                                    Console.WriteLine("Perfiles:");
                                    tareaUsuario = BuscarPorIdAsync(id);
                                    break;
                                default:
                                    Console.WriteLine("Opcion incorrecta");
                                    break;
                            }
                            break;
                        case "6":
                            Console.WriteLine("Enviar y recibir mensajes");
                            Console.WriteLine("1)Enviar mensaje \n2)Recibir mensaje");
                            int opcionMensaje = Int32.Parse(Console.ReadLine());
                            switch (opcionMensaje)
                            {
                                case 1:
                                    Console.WriteLine("Introduzca username del receptor");
                                    String nombre = Console.ReadLine();
                                    Console.WriteLine("Introduzca mensaje");
                                    String mensaje = Console.ReadLine();
                                    tareaUsuario = EnviarMensajeAsync(nombre, mensaje);
                                    break;
                                case 2:
                                    int opcionRecibir = 0;
                                    while (opcionRecibir != 1 && opcionRecibir != 2)
                                    {
                                        Console.WriteLine("1)Mostrar nuevos mensajes \n2)Mostrar mensajes anteriores");
                                        opcionRecibir = int.Parse(Console.ReadLine());
                                    }

                                    tareaUsuario = RecibirMensajesAsync(opcionRecibir);
                                    break;
                                default:
                                    Console.WriteLine("Opcion incorrecta");
                                    break;
                            }

                            break;
                        case "7":
                            Console.WriteLine("Sesion finalizada");
                            break;
                        default:
                            Console.WriteLine("Opcion no valida");
                            break;
                    }
                    await tareaUsuario;
                }


            }
            catch (SocketException)
            {
                Console.WriteLine("Servidor desconectado, pulse cualquier tecla para salir");
                Console.ReadLine();
            }
            finally
            {
                manejoDataSocket.Close();
            }

        }

        public async Task AltaUsuarioAsync(String usernameS, String passwordS, String id)
        {
            int largo = usernameS.Length + passwordS.Length + id.Length + 2;
            String mensaje = usernameS + "|" + passwordS + "|" + id;

            await EstructuraDeProtocolo.envioAsync("REQ", "02", largo, mensaje, manejoDataSocket);

            List<String> retorno = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);

            session = Guid.Parse(retorno[3]);
            Console.WriteLine(retorno[4]);
        }

        public async Task CrearPerfilAsync(String descripcion, List<String> habilidades)
        {

            int largoDescripcion = descripcion.Length;

            StringBuilder habilidad = new StringBuilder();

            habilidad.Append(descripcion + "|");
            habilidad.Append(session.ToString() + "|");

            for (int i = 0; i < habilidades.Count - 1; i++)
            {

                habilidad.Append(habilidades[i] + "|");

            }

            habilidad.Append(habilidades[habilidades.Count - 1]);

            String skills = habilidad.ToString();

            int largoHabilidades = skills.Length;

            await EstructuraDeProtocolo.envioAsync("REQ", "03", largoHabilidades, skills, manejoDataSocket);
            List<String> retorno = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
            Console.WriteLine(retorno[3]);

        }

        public async Task AsociarFotoAsync(String ruta)
        {
            String envio = session.ToString();
            int largo = envio.Length;
            await EstructuraDeProtocolo.envioAsync("REQ", "04", largo, envio, manejoDataSocket);
            GestorArchivos fileCommsHandler = new GestorArchivos(_cliente);
            try
            {
                await fileCommsHandler.SendFileAsync(ruta);
                List<String> retorno = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
                Console.WriteLine(retorno[3]);
            }
            catch (Exception)
            {
                Console.WriteLine("Imagen invalida");
            }


        }


        public async Task BuscarPorNombreAsync(String nombre)
        {
            int largo = nombre.Length;
            await EstructuraDeProtocolo.envioAsync("REQ", "51", largo, nombre, manejoDataSocket);
            List<String> respuesta = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }
        public async Task BuscarPorHabilidadesAsync(List<String> habilidades)
        {
            StringBuilder habilidad = new StringBuilder();


            for (int i = 0; i < habilidades.Count - 1; i++)
            {

                habilidad.Append(habilidades[i] + "|");

            }

            habilidad.Append(habilidades[habilidades.Count - 1]);

            String skills = habilidad.ToString();

            int largoHabilidades = skills.Length;

            await EstructuraDeProtocolo.envioAsync("REQ", "52", largoHabilidades, skills, manejoDataSocket);
            List<String> respuesta = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
            
            Console.WriteLine(respuesta[3]);
        }

        public async Task BuscarPorIdAsync(String id)
        {
            int largo = id.Length;
            await EstructuraDeProtocolo.envioAsync("REQ", "53", largo, id, manejoDataSocket);
            List<String> respuesta = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
            if (respuesta[4] == "True")
            {
                Console.WriteLine("Desea descargar imagen del perfil?(S/_)");
                if (Console.ReadLine() == "S")
                {
                    String envio = id + "|" + rutaImagen;
                    largo = envio.Length;
                    await EstructuraDeProtocolo.envioAsync("REQ", "54", largo, envio, manejoDataSocket);
                    GestorArchivos fileCommsHandler = new GestorArchivos(_cliente);
                    fileCommsHandler.ReceiveFile(rutaImagen + "\\" + id);
                    respuesta = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
                    Console.WriteLine(respuesta[3]);
                }
            }
        }

        public async Task EnviarMensajeAsync(String userName, String mensaje)
        {
            String envio = userName + "|" + mensaje + "|" + session.ToString();
            int largo = envio.Length;
            await EstructuraDeProtocolo.envioAsync("REQ", "61", largo, envio, manejoDataSocket);
            List<String> respuesta = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }

        public async Task RecibirMensajesAsync(int opcion)
        {
            String envio = session.ToString() + "|" + opcion;
            int largo = envio.Length;
            await EstructuraDeProtocolo.envioAsync("REQ", "62", largo, envio, manejoDataSocket);
            List<String> respuesta = await EstructuraDeProtocolo.reciboAsync(manejoDataSocket);
            Console.WriteLine(respuesta[3]);

        }
        public void CerrarConexion()
        {
            _cliente.Close();
        }
    }
}

