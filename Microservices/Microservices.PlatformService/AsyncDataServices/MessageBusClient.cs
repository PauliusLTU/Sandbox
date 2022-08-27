using Microservices.PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Microservices.PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            ConnectionFactory factory = new ConnectionFactory() 
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.Write("--> Connected to message bus");
            }
            catch (Exception e)
            {
                Console.Write($"--> Could not connect to the message bus: {e.Message}");
            }
        }

        void IMessageBusClient.PublishPlatform(PlatformPublishedDto platformPublishedDto)
        {
            string message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.Write("--> Rabbit MQ connection is open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.Write("--> Rabbit MQ connection is closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

            Console.Write($"--> We have send {message}");
        }

        public void Dispose()
        {
            Console.Write($"--> Message bus disposed");

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.Write("--> Rabbit MQ connection shut down");
        }
    }
}
