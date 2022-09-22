using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Protocolo;

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
                        //ConectarAlServidor();
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
                        String descripcion = Console.ReadLine();

                        String habilidad;
                        List<String> habilidades = new List<string>();
                        do
                        {
                            Console.WriteLine("Ingrese una habilidad o (X) para salir");
                            habilidad = Console.ReadLine();
                            if (habilidad != "X")
                            {
                                habilidades.Add(habilidad);
                            }
                        } while (habilidad != "X");
                        //CrearPerfil(descripcion, habilidades);
                        break;
                    case "4":
                        Console.WriteLine("Asociar foto al perfil, \n Introduzca ruta de la misma");
                        string ruta = Console.ReadLine();
                        //AsociarFoto(imagen);
                        break;
                    case "5":
                        Console.WriteLine("Consultar perfiles existentes");
                        Console.WriteLine("1)Consultar por nombre \n2)Consultar por palabra clave");
                        int buscarPor = Int32.Parse(Console.ReadLine());
                        switch (buscarPor)
                            
                        {
                            case 1:
                                Console.WriteLine("Introduzca nombre del usuario");
                                String nombre = Console.ReadLine();
                                //BuscarPorNombre(nombre);
                                break;
                            case 2:
                                String habilidadABuscar;
                                List<String> habilidadesABuscar = new List<string>();
                                do
                                {
                                    Console.WriteLine("Ingrese una habilidad o (X) para salir");
                                    habilidadABuscar = Console.ReadLine();
                                    if (habilidadABuscar != "X")
                                    {
                                        habilidadesABuscar.Add(habilidadABuscar);
                                    }
                                } while (habilidadABuscar != "X");
                                //BuscarPorHAbilidades(habilidadesABuscar);
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
                                //EnviarMensaje(nombre, mensaje);
                                break;
                            case 2:
                                //RecibirMensajes();
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
            int largo = usernameS.Length + passwordS.Length + id.Length +  2;
            String header = "REQ" + "02" + largo.ToString().PadLeft(4, '0');
            String mensaje = usernameS + "#" + passwordS + "#" + id;
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);

            /*
            String tipoDeMensaje = "REQ";
            byte[] tipoEnBytes = Encoding.UTF8.GetBytes(header);
            int codigoDeMensaje = 2;
            byte[] codigoEnBytes = BitConverter.GetBytes(codigoDeMensaje);
            byte[] largoEnBytes = BitConverter.GetBytes(largo);
            byte[] mensajeEnBytes = Encoding.UTF8.GetBytes(mensaje);
            manejoDataSocket.Send(tipoEnBytes);
            manejoDataSocket.Send(codigoEnBytes);
            manejoDataSocket.Send(largoEnBytes);
            manejoDataSocket.Send(mensajeEnBytes);
            */

            manejoDataSocket.Send(headerBytes);
            manejoDataSocket.Send(mensajeBytes);
            byte[] recive = manejoDataSocket.Recive(9);
            byte[] guid = manejoDataSocket.Recive(36); //Esto podria ser variable
            String guidString = Encoding.UTF8.GetString(guid);
            session = Guid.Parse(guidString);
            Console.WriteLine("Sesion iniciada con el id: " + session);
        }

        public void CerrarConexion()
        {
            socketCliente.Shutdown(SocketShutdown.Both);
            socketCliente.Close();
        }
    }


}

