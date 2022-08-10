using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection? _connection;
        private IModel? _channel;
        private string? _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = _configuration.GetValue<int>("RabbitMQPort")
            };

            try
            {
                _connection = connectionFactory.CreateConnection();

                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare("trigger", ExchangeType.Fanout);

                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(_queueName, "trigger", "");

                Console.WriteLine("--> Listening on the MessageBus");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the MessageBus: {ex.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += Consumer_Received;

            _channel.BasicConsume(_queueName, true, consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine("--> Event received");

            var body = e.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            await _eventProcessor.ProcessEvent(notificationMessage);
        }

        public override void Dispose()
        {
            if (_channel?.IsOpen == true)
            {
                _channel.Close();
            }
            if (_connection?.IsOpen == true)
            {
                _connection.Close();
            }

            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
