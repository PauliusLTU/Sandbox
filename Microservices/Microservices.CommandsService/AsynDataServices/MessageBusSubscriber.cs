using Microservices.CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Microservices.CommandsService.AsynDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            Console.WriteLine("--> Listening on message bus...");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitializeRabbitMQ();

            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (s, e) =>
            {
                Console.WriteLine("--> Event received");

                ReadOnlyMemory<byte> body = e.Body;
                string notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(_queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.Write("--> Rabbit MQ connection shut down");
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}
