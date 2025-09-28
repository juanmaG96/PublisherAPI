using PublisherAPI.Models;

namespace PublisherAPI.Services
{
    public interface IRabbitMQService<TClass> where TClass : ClassMessageRMQ
    {
        void sendMessage(TClass mensaje);
        bool SendMessageWithConfirmation(TClass mensaje);
    }
}