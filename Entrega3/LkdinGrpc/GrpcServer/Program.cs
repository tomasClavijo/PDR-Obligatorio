using LKAdin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Protocolo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer
{
    public class Program
    {
        static readonly GestorConfig settingsManager = new GestorConfig();

        public static void Main(string[] args)
        {
            try
            {
                string serverIp = settingsManager.ReadSettings(ConfigServidor.ServerIpConfig);
                int serverPort = int.Parse(settingsManager.ReadSettings(ConfigServidor.ServerPortConfig));
                string rutaImagenes = settingsManager.ReadSettings(ConfigServidor.PictureFolder);
                Controlador controlador = Controlador.GetInstance();
                Servidor servidor = new Servidor(controlador, serverIp, serverPort, rutaImagenes);
                LevantarServidor(servidor);
            }
            catch (Exception)
            {
                Console.WriteLine("Error interno");
                Console.WriteLine("Presione cualquier tecla para salir");
                Console.ReadLine();
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static async Task LevantarServidor(Servidor servidor)
        {
            await Task.Run(() =>servidor.RecibirClientesAsync());
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://localhost:5000");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
