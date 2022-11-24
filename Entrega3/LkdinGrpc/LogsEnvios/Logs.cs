using System;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using LogsEnvios;

namespace LogsEnvios
{
    public class mensajeDTO
    {
        public string mensaje { get; set; }
        public string fecha { get; set; }
        public string username { get; set; }
    }

    public class Logs
    {
        ConnectionFactory factory;
        IConnection connection;
        IModel channel;

        public Logs()
        {
            initLogs();
        }

        public void initLogs()
        {
            factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "lkdin",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }

        public void EnvioLogs(string mensaje, string username)
        {
            mensajeDTO mensajeEnvio = new mensajeDTO();
            mensajeEnvio.mensaje = mensaje;
            mensajeEnvio.username = username;
            mensajeEnvio.fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            Message(mensajeEnvio, channel);
        }


        private static string Message(mensajeDTO mensaje, IModel channel)
        {
            string messsage = JsonSerializer.Serialize(mensaje);
            var body = Encoding.UTF8.GetBytes(messsage);
            channel.BasicPublish(exchange: "",
                routingKey: "lkdin",
                basicProperties: null,
                body: body);
            return messsage;
        }

    }
}

