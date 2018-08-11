using System;
using NetMQ;

namespace Utils.Network
{
    public class NetMqMessageEventArgs : EventArgs
    {
        public NetMqMessageEventArgs(NetMQMessage message)
        {
            Message = message;
        }

        public NetMQMessage Message { get; private set; }
    }
}
