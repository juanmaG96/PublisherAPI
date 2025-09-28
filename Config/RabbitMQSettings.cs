using System;

namespace PublisherAPI.Config
{
    public class RabbitMQSettings
    {
        public string RabbitMQUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string Port { get; set; }
    }
}