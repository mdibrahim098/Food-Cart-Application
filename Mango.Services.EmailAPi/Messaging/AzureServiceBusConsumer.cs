using System.Text;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPi.Models.Dto;
using Mango.Services.EmailAPi.Serviecs;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration,EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            _emailService = emailService;
            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
        }

        public async Task Start()
        {
           _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
           _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            // this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto ObjMessage = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                //TODO - try to log email
                await _emailService.EmailCartAndLog(ObjMessage);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {

                throw;
            }
 

            }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
           Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }


    }
}
