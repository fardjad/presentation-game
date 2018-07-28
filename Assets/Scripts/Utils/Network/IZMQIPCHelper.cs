using System;
using NetMQ;

namespace Utils.Network
{
    public interface IZMQIPCHelper : IDisposable
    {
        event EventHandler<NetMQMessageEventArgs> OnMessage;
        void Send(string message);
        void Send(NetMQMessage message);
        void Start();
        void Close();
    }
}