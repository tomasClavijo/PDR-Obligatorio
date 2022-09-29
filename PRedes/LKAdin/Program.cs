using LKAdin;
using Protocolo;
using System;

namespace LKAdin
{
    internal class Program
    {
        static readonly GestorConfig settingsManager = new GestorConfig();
        static void Main(string[] args)
        {
            string serverIp = settingsManager.ReadSettings(ConfigServidor.ServerIpConfig);
            int serverPort = int.Parse(settingsManager.ReadSettings(ConfigServidor.ServerPortConfig));
            string rutaImagenes = settingsManager.ReadSettings(ConfigServidor.PictureFolder);
            Controlador controlador = new Controlador();
            Servidor servidor = new Servidor(controlador, serverIp, serverPort, rutaImagenes);
        }
    }
}
