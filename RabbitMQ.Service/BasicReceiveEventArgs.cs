using System;

namespace RabbitMQ.Service
{
    public class BasicReceiveEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
