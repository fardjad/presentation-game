using System;
using NetMQ;

namespace Utils.Network
{
    public class NetMQMessageEventArgs : EventArgs
    {
        public NetMQMessageEventArgs(NetMQMessage message)
        {
            Message = message;
        }

        public NetMQMessage Message { get; set; }
    }
}