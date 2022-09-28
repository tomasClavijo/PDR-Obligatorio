using Protocolo;
using System;

namespace ClienT
{
    internal class Program
    {
        static readonly SettingsManager settingsManager = new SettingsManager();
        static void Main(string[] args)
        {
            string serverIp = settingsManager.ReadSettings(ConfigServidor.ServerIpConfig);
            int serverPort = int.Parse(settingsManager.ReadSettings(ConfigServidor.ServerPortConfig));
            string localIp = settingsManager.ReadSettings(ConfigServidor.LocalIpConfig);
            Console.WriteLine("Inicia cliente");
            Cliente cliente = new Cliente(serverIp, serverPort, localIp);
        }
    }
}
