using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Protocolo;
using System.IO.IsolatedStorage;
using System.Threading;
using Communication;

namespace ClienT
{
    public class Cliente
    {
        int puertoRemoto;
        string ipRemota;
        string ipLocal;
        static Socket socketCliente;
        EndPoint endPointLocal;
        EndPoint endPointRemoto;
        Guid session;
        ManejoDataSocket manejoDataSocket;
        

        public Cliente()
        {
            ConfigurarConexion();
            manejoDataSocket = new ManejoDataSocket(socketCliente);
            
            Interfaz();
            CerrarConexion();
        }

        public void ConfigurarConexion()
        {
            //Datos de preuba
            //puertoRemoto = 2000;
            //ipRemota = "127.0.0.1";
            ipLocal = "127.0.0.1";
            //
            socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPointLocal = new IPEndPoint(IPAddress.Parse(ipLocal), 0);
            socketCliente.Bind(endPointLocal);
            endPointRemoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
            socketCliente.Connect(endPointRemoto);
        }

        public String Interfaz()
        {
            string opcion = "#";
            while (!opcion.Contains("7"))
            {

                Console.WriteLine("1. Conexión de un cliente al servidor");
                Console.WriteLine("2. Dar de alta a un usuario");
                Console.WriteLine("3. Crear perfil de trabajo de usuario");
                Console.WriteLine("4. Asociar foto al perfil");
                Console.WriteLine("5. Consultar perfiles existentes");
                Console.WriteLine("6. Enviar y recibir mensajes");
                Console.WriteLine("7. Fin de conexion");
                opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("Conectando al servidor a través de datos asociados...");
                        Thread.Sleep(10000);
                        Console.WriteLine("Conexión establecida");
                        break;
                    case "2":
                        Console.WriteLine("Ingrese nombre de usuario");
                        String username = Console.ReadLine();
                        Console.WriteLine("Ingrese id de usuario");
                        String userID = Console.ReadLine();
                        Console.WriteLine("Ingrese contraseña");
                        String password = Console.ReadLine();
                        AltaUsuario(username, password, userID);
                        break;
                    case "3":
                        Console.WriteLine("Crear su perfil de usuario");
                        Console.WriteLine("Ingrese descripcion de su perfil");

                        String descripcion = Console.ReadLine();

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
                        CrearPerfil(descripcion, habilidades);
                        break;
                    case "4":
                        Console.WriteLine("Asociar foto al perfil, \n Introduzca ruta de la misma");
                        string ruta = Console.ReadLine();
                        AsociarFoto(ruta);
                        break;
                    case "5":
                        Console.WriteLine("Consultar perfiles existentes");
                        Console.WriteLine("1)Consultar por nombre \n2)Consultar por palabra clave \n3)Consultar por id");
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
                                String habilidadABuscar;
                                List<String> habilidadesABuscar = new List<string>();
                                do
                                {
                                    Console.WriteLine("Ingrese una habilidad o (X) para salir");
                                    habilidadABuscar = Console.ReadLine();
                                    if (habilidadABuscar != "X" && habilidadABuscar != "x")
                                    {
                                        habilidadesABuscar.Add(habilidadABuscar);
                                    }
                                } while (habilidadABuscar != "X" && habilidadABuscar != "x");
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

            StructuralMessage.envio("REQ", "02", largo, mensaje, manejoDataSocket);

            List<String> retorno = StructuralMessage.recibo(manejoDataSocket);
            if(retorno[3] == Guid.Empty.ToString())
            {
                Console.WriteLine("El usuario ya existe");
            }
            else{
                session = Guid.Parse(retorno[3]);

                Console.WriteLine("Sesion iniciada");
            }
 
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

            StructuralMessage.envio("REQ", "03", largoHabilidades, skills, manejoDataSocket);
            List<String> respuesta = StructuralMessage.recibo(manejoDataSocket);
            Console.WriteLine("Perfil creado exitosamente");

        }

        public void AsociarFoto(String ruta)
        {
            FileCommsHandler fileCommsHandler = new FileCommsHandler(socketCliente);
            fileCommsHandler.SendFile(ruta);
            Console.WriteLine("Imagen asociada con exito");
        }


        public void BuscarPorNombre(String nombre)
        {
            int largo = nombre.Length;
            StructuralMessage.envio("REQ", "51", largo, nombre, manejoDataSocket);
            List<String> respuesta = StructuralMessage.recibo(manejoDataSocket);
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

            StructuralMessage.envio("REQ", "52", largoHabilidades, skills, manejoDataSocket);
            List<String> respuesta = StructuralMessage.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }

        public void BuscarPorId(String id)
        {
            int largo = id.Length;
            StructuralMessage.envio("REQ", "53", largo, id, manejoDataSocket);
            List<String> respuesta = StructuralMessage.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }

        public void EnviarMensaje(String userName, String mensaje)
        {
            String envio = userName + "|" + mensaje + "|" + session.ToString();
            int largo = envio.Length;
            StructuralMessage.envio("REQ", "61", largo, envio, manejoDataSocket);
            List<String> respuesta = StructuralMessage.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);
        }

        public void RecibirMensajes()
        {
            String envio = session.ToString();
            int largo = envio.Length;
            StructuralMessage.envio("REQ", "62", largo, envio, manejoDataSocket);
            List<String> respuesta = StructuralMessage.recibo(manejoDataSocket);
            Console.WriteLine(respuesta[3]);

        }

        public void CerrarConexion()
        {
            socketCliente.Shutdown(SocketShutdown.Both);
            socketCliente.Close();
        }
    }


}

