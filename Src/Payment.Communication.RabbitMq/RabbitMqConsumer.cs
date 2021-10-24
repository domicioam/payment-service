using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Payment.Communication.RabbitMq
{
    public delegate void DoWork(string message);
    public class RabbitMqConsumer
    {
        private IConnection _connection;
        private IModel _channel;
        // private const string QUEUE_NAME = "payment_queue";
        private readonly RabbitMqConfig _rabbitMqConfig;

        public event DoWork MessageReceived;

        public Task StartListeningForRequestsAsync(string queueName)
        {
            return Task.Run(() =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMqConfig.HostName,
                    UserName = _rabbitMqConfig.UserName,
                    Password = _rabbitMqConfig.Password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false,
                                        autoDelete: false, arguments: null);

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.Span);

                    MessageReceived(message);

                    _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: true);
                };

                _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            });
        }

        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IOptions<RabbitMqConfig> rabbitMqConfig) : this()
        {
            _rabbitMqConfig = rabbitMqConfig.Value;
        }

        // Necessary for mocking
        protected RabbitMqConsumer() : base()
        {

        }
    }
}
