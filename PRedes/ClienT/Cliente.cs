using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Protocolo;
using System.Threading;

namespace ClienT
{
    public class Cliente
    {

        static Socket socketCliente;
        EndPoint endPointLocal;
        EndPoint endPointRemoto;
        Guid session;
        ManejoDataSocket manejoDataSocket;
        

        public Cliente(String ServerIp, int ServerPort, String LocalIp)
        {
            ConfigurarConexion(ServerIp, ServerPort, LocalIp);
            manejoDataSocket = new ManejoDataSocket(socketCliente);
            
            Interfaz();
            CerrarConexion();
        }

        public void ConfigurarConexion(String ServerIp, int ServerPort, String LocalIp)
        {
            socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPointLocal = new IPEndPoint(IPAddress.Parse(LocalIp), 0);
            socketCliente.Bind(endPointLocal);
            endPointRemoto = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
            socketCliente.Connect(endPointRemoto);
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
                if (habilidad != "X" && habilidad != "x")
                {
                    habilidades.Add(habilidad);
                }
            } while (habilidad != "X" && habilidad != "x");
            return habilidades;
        }
        
        public String Interfaz()
        {
            string opcion = "#";
            while (!opcion.Contains("7"))
            {
                Menu();
                opcion = Console.ReadLine();
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
                        Console.WriteLine("Ingrese nombre de usuario");
                        String userID = Console.ReadLine();
                        Console.WriteLine("Ingrese contraseña");
                        String password = Console.ReadLine();
                        AltaUsuario(username, password, userID);
                        break;
                    case "3":
                        Console.WriteLine("Crear su perfil de usuario");
                        Console.WriteLine("Ingrese descripcion de su perfil");
                        String descripcion = Console.ReadLine();
                        List <String> habilidades= CargarHabilidades();
                        CrearPerfil(descripcion, habilidades);
                        break;
                    case "4":
                        Console.WriteLine("Asociar foto al perfil, \n Introduzca ruta de la misma");
                        string ruta = Console.ReadLine();
                        AsociarFoto(ruta);
                        break;
                    case "5":
                        Console.WriteLine("Consultar perfiles existentes");
                        Console.WriteLine("1)Consultar por nombre \n2)Consultar por palabra clave" +
                            " \n3)Consultar por id");
                        int buscarPor = Int32.Parse(Console.ReadLine());
                        switch (buscarPor)
                            
                        {
                            case 1:
                                Console.WriteLine("Introduzca nombre del usuario");
                                String nombre = Console.ReadLine();
                                Console.WriteLine("Perfiles:");
                                BuscarPorNombre(nombre);
                                break;
                            case 2:
                                List<String> habilidadesABuscar = CargarHabilidades();
                                Console.WriteLine("Perfiles:");
                                BuscarPorHabilidades(habilidadesABuscar);
                                break;
                            case 3:
                                Console.WriteLine("Introduzca el id a buscar");
                                String id = Console.ReadLine();
                                Console.WriteLine("Perfiles:");
                                BuscarPorId(id);
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
                                Console.WriteLine("Introduzca nombre del usuario");
                                String nombre = Console.ReadLine();
                                Console.WriteLine("Introduzca mensaje");
                                String mensaje = Console.ReadLine();
                                EnviarMensaje(nombre, mensaje);
                                break;
                            case 2:
                                RecibirMensajes();
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
            }

            return opcion;
        }

        public void AltaUsuario(String usernameS, String passwordS, String id)
        {
            int largo = usernameS.Length + passwordS.Length + id.Length + 2;
            String mensaje = usernameS + "|" + passwordS + "|" + id;

            EstructuraDeProtocolo.envio("REQ", "02", largo, mensaje, manejoDataSocket);

            List<String> retorno = EstructuraDeProtocolo.recibo(manejoDataSocket);
            String[] descomprimido = retorno[3].Split('|');
            session = Guid.Parse(descomprimido[0]);
            Console.WriteLine(retorno[1]);
        }

        public void CrearPerfil(String descripcion, List<String> habilidades)
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

            EstructuraDeProtocolo.envio("REQ", "03", largoHabilidades, skills, manejoDataSocket);
            List<String> retorno = EstructuraDeProtocolo.recibo(manejoDataSocket);
            Console.WriteLine(retorno[3]);

        }

        public void AsociarFoto(String ruta)
        {
            String envio = session.ToString();
            int largo = envio.Length;
            EstructuraDeProtocolo.envio("REQ", "04", largo, envio, manejoDataSocket);
            GestorArchivos fileCommsHandler = new GestorArchivos(socketCliente);
            fileCommsHandler.SendFile(ruta);
            List<String> retorno = EstructuraDeProtocolo.recibo(manejoDataSocket);
            Console.WriteLine(retorno[3]);
        }


        public void BuscarPorNombre(String nombre)
        {
            int largo = nombre.Length;
            EstructuraDeProtocolo.envio("REQ", "51", largo, nombre, manejoDataSocket);
            List<String> respuesta = EstructuraDeProtocolo.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }
        public void BuscarPorHabilidades(List<String> habilidades)
        {
            StringBuilder habilidad = new StringBuilder();


            for (int i = 0; i < habilidades.Count - 1; i++)
            {

                habilidad.Append(habilidades[i] + "|");

            }

            habilidad.Append(habilidades[habilidades.Count - 1]);

            String skills = habilidad.ToString();

            int largoHabilidades = skills.Length;

            EstructuraDeProtocolo.envio("REQ", "52", largoHabilidades, skills, manejoDataSocket);
            List<String> respuesta = EstructuraDeProtocolo.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }

        public void BuscarPorId(String id)
        {
            int largo = id.Length;
            EstructuraDeProtocolo.envio("REQ", "53", largo, id, manejoDataSocket);
            List<String> respuesta = EstructuraDeProtocolo.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }

        public void EnviarMensaje(String userName, String mensaje)
        {
            String envio = userName + "|" + mensaje + "|" + session.ToString();
            int largo = envio.Length;
            EstructuraDeProtocolo.envio("REQ", "61", largo, envio, manejoDataSocket);
            List<String> respuesta = EstructuraDeProtocolo.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }

        public void RecibirMensajes()
        {
            String envio = session.ToString();
            int largo = envio.Length;
            EstructuraDeProtocolo.envio("REQ", "62", largo, envio, manejoDataSocket);
            List<String> respuesta = EstructuraDeProtocolo.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);

        }

        public void CerrarConexion()
        {
            socketCliente.Shutdown(SocketShutdown.Both);
            socketCliente.Close();
        }
    }


}

