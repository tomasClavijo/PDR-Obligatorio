using LKAdin;
using Protocolo;
using System;

namespace LKAdin
{
    internal class Program
    {
        static readonly SettingsManager settingsManager = new SettingsManager();
        static void Main(string[] args)
        {
            string serverIp = settingsManager.ReadSettings(ServerConfig.ServerIpConfig);
            int serverPort = int.Parse(settingsManager.ReadSettings(ServerConfig.ServerPortConfig));
            Controlador controlador = new Controlador();
            Servidor servidor = new Servidor(controlador, serverIp, serverPort);
        }
    }
}
