using System;

namespace PublisherAPI.Models
{
    [Serializable]
    public class ClassMessageRMQ
    {
        public string QueueName { get; set; }
    }
}