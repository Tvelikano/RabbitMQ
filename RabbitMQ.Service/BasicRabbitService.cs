using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Service
{
    public class BasicRabbitService : IRabbitService
    {
        private readonly IConnection _connection;
        private readonly IModel _channelSend;
        private readonly IModel _channelReceive;

        private const string HostName = "localhost";
        private const bool AutoDelete = false;
        private const bool Exclusive = false;
        private const bool Durable = false;
        private const IDictionary<string, object> Arguments = null;

        public event EventHandler<BasicReceiveEventArgs> Received;

        public string ExchangeName { get; set; } = "chat_queue";
        public string UserName { get; set; }

        public BasicRabbitService(string userName = "anonymous")
        {
            UserName = userName;

            _connection = new ConnectionFactory
            {
                HostName = HostName,
            }.CreateConnection();

            _channelSend = _connection.CreateModel();
            _channelSend.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, Durable, AutoDelete, Arguments);

            SendJoinedMessage();

            _channelReceive = _connection.CreateModel();
            _channelReceive.QueueDeclare(UserName, Durable, Exclusive, AutoDelete, Arguments);
            _channelReceive.QueueBind(UserName, ExchangeName, "");

            var consumer = new EventingBasicConsumer(_channelReceive);
            _channelReceive.BasicConsume(UserName, true, consumer);

            consumer.Received += Consumer_Received;
        }

        public void SendMessage(string message)
        {
            _channelSend.BasicPublish(ExchangeName, "", null, Encoding.UTF8.GetBytes($"[{ DateTime.Now:yyyy - MM - dd HH: mm: ss}] { UserName}> { message}"));
        }

        public void Dispose()
        {
            SendLeavedMessage();

            _connection?.Close();
            _channelSend?.Close();
            _channelReceive?.Close();
        }

        private void SendJoinedMessage()
        {
            _channelSend.BasicPublish(ExchangeName, "", null, Encoding.UTF8.GetBytes($"User '{ UserName}' joined"));
        }

        private void SendLeavedMessage()
        {
            _channelSend.BasicPublish(ExchangeName, "", null, Encoding.UTF8.GetBytes($"User '{ UserName}' left"));
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            Received?.Invoke(this, new BasicReceiveEventArgs { Message = Encoding.UTF8.GetString(e.Body) });
        }
    }
}
