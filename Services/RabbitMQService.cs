using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using PublisherAPI.Models;
using PublisherAPI.Config;
using Microsoft.Extensions.Options;

namespace PublisherAPI.Services
{
    public class RabbitMQService<TClass> : IRabbitMQService<TClass>
        where TClass : ClassMessageRMQ
    {
        private ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _config;
        private string? QueueName { get; set; }

        public RabbitMQService(IOptions<RabbitMQSettings> config, string? QueueName = null)
        {
            _config = config.Value;

            _connectionFactory = new ConnectionFactory()
            {
                //Uri = new Uri(_config.RabbitMQUrl),     //RabbitMQUrl = _config.RabbitMQUrl,
                UserName = _config.UserName,
                Password = _config.Password,
                HostName = _config.HostName,
                Port = int.Parse(_config.Port)
            };

            //string rabbitUrl = _config.RabbitMQUrl;
            //_connection = _connectionFactory.CreateConnection(rabbitUrl);
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ConfirmSelect();

            this.QueueName = QueueName != null ? QueueName : "QDefault";

            _channel.QueueDeclare(queue: this.QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        /*
        * Enviar un mensaje a la cola Rabbit sin confirmacion de recepcion en RabbitMQ
        * MessageRMq : clase con el nombre de la queue y el mensaje como string
        *  
        */
        public void sendMessage(TClass mensaje)
        {
            string jsonMessage = JsonConvert.SerializeObject(mensaje);
            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            string _queueName = mensaje.QueueName != "" ? mensaje.QueueName : this.QueueName;

            _channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
        }

        /*
        * Envia un mensaje y retorna true si llega la confirmacion de recepcion desde RabbitMQ
        */
        public bool SendMessageWithConfirmation(TClass mensaje)
        {
            bool ret = false;
            string? _queueName = mensaje.QueueName != "" ? mensaje.QueueName : this.QueueName;
            string jsonMessage = JsonConvert.SerializeObject(mensaje);
            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);

            // esperar confirmacion del servidor RabbitMQ
            if (_channel.WaitForConfirms(TimeSpan.FromSeconds(1)))
                ret = true;
            else
                ret = false;

            return ret;
        }

         public void CloseConnection()
        {
            _channel.Close();
            _connection.Close();
        }
        public int GetMessageCount(string queuename)
        {
            QueueDeclareOk cola = _channel.QueueDeclarePassive(queuename);
            return (int)cola.MessageCount;
        }  

    }
}