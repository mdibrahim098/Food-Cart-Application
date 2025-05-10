namespace Mango.Services.EmailAPi.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();

    }
}
