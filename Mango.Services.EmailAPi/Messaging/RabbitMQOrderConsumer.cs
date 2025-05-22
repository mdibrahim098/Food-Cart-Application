using System.Text;
using Mango.Services.EmailAPi.Message;
using Mango.Services.EmailAPi.Serviecs;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.EmailAPi.Messaging
{
    public class RabbitMQOrderConsumer : BackgroundService
    {

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private IModel _channel;
        private const string OrderCreated_EmailUpdatedQueue = "EmailUpdateQueue";
        private string ExchangeName = "";
        public RabbitMQOrderConsumer(IConfiguration configuration,
               EmailService emailService)
        {

            _configuration = configuration;
            _emailService = emailService;
            ExchangeName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(OrderCreated_EmailUpdatedQueue, false,false,false,null);
            _channel.QueueBind(OrderCreated_EmailUpdatedQueue, ExchangeName, "EmailUpdate");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(content);
                HandleMessage(rewardsMessage).GetAwaiter().GetResult();


                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(OrderCreated_EmailUpdatedQueue, false, consumer);

            return Task.CompletedTask;
        }


        private async Task HandleMessage(RewardsMessage rewardsMessage)
        {
          _emailService.LogOrderPlaced(rewardsMessage).GetAwaiter().GetResult();

        }



    }
}
