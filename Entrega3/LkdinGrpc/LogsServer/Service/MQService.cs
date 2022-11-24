using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LogsServer.IService;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace WebApiRabbitMQ.Service
{
    public class mensajeDTO
    {
        public string mensaje { get; set; }
        public string fecha { get; set; }
        public string username { get; set; }
    }
    public class MQService : IMQService
    {
        List<mensajeDTO> mensajes;

        public MQService() {
            mensajes = new List<mensajeDTO>();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "lkdin",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    mensajeDTO mensaje = JsonSerializer.Deserialize<mensajeDTO>(message);
                    mensajes.Add(mensaje);
                };
                channel.BasicConsume(queue: "lkdin",
                                     autoAck: false,
                                     consumer: consumer);

            }
        }

        public List<string> GetMessages(string username, DateTime date, string word)
        {
            List<string> respuesta = new List<string>();
            foreach (var mensaje in mensajes)
            {
                if (mensaje.username.Contains(username) 
                    && DateTime.Parse(mensaje.fecha) > date
                    && mensaje.mensaje.Contains(word))
                {
                    respuesta.Add(mensaje.mensaje);
                }
            }
            if(respuesta.Count == 0)
            {
                respuesta.Add("No hay mensajes disponibles para esta consulta");
            }
            return respuesta;
        }
    }
}
