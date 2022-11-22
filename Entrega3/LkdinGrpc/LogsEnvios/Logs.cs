using System;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace LogsEnvios
{
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
        
        public void EnvioLogs(string mensaje)
        {
            Message(mensaje, channel);
        }
        
        
        private static string Message(string mensaje, IModel channel)
        {
            var body = Encoding.UTF8.GetBytes(mensaje);
            channel.BasicPublish(exchange: "",
                routingKey: "lkdin",
                basicProperties: null,
                body: body);
            return mensaje;
        }

    }
}
