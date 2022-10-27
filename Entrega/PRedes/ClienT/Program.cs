using Protocolo;
using System;

namespace ClienT
{
    internal class Program
    {
        static readonly GestorConfig gestorConfig = new GestorConfig();
        static void Main(string[] args)
        {
            try
            {
                string serverIp = gestorConfig.ReadSettings(ConfigServidor.ServerIpConfig);
                int serverPort = int.Parse(gestorConfig.ReadSettings(ConfigServidor.ServerPortConfig));
                string localIp = gestorConfig.ReadSettings(ConfigServidor.LocalIpConfig);
                string rutaImagenes = gestorConfig.ReadSettings(ConfigServidor.PictureFolder);
                Console.WriteLine("Inicia cliente");
                Cliente cliente = new Cliente(serverIp, serverPort, localIp, rutaImagenes);
            }
            catch (Exception)
            {
                Console.WriteLine("Error interno");
                Console.WriteLine("Presione cualquier tecla para salir");
                Console.ReadLine();
            }

        }
    }
}
