using RabbitMQ.Client.Events;

using System;

namespace RabbitMQ.Service
{
    public interface IRabbitService : IDisposable
    {
        string ExchangeName { get; set; }
        string UserName { get; set; }

        event EventHandler<BasicReceiveEventArgs> Received;

        void SendMessage(string message);
    }
}