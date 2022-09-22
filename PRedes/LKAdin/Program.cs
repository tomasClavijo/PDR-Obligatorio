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
            Console.WriteLine("Server IP: " + serverIp);
            Console.WriteLine("Inicia Servidor");
            Controlador controlador = new Controlador();
            Servidor servidor = new Servidor(controlador);
        }
    }
}
