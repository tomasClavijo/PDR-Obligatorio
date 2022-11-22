﻿using System;
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
    public class MQService : IMQService
    {
        List<String> mensajes;

        public MQService() {
            mensajes = new List<string>();
            // Conexión con RabbitMQ local: 
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Defino la conexion

             var connection = factory.CreateConnection();
             var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "lkdin", // en el canal, definimos la Queue de la conexion
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            //Defino el mecanismo de consumo
            var consumer = new EventingBasicConsumer(channel);
            //Defino el evento que sera invocado cuando llegue un mensaje 
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                mensajes.Add(message);

            };
            
            //"PRENDO" el consumo de mensajes
            channel.BasicConsume(queue: "lkdin",
                autoAck: false,
                consumer: consumer);
        }

        public List<String> GetMessages()
        {
            return mensajes;
        }
    }
}
