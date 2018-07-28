using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace Utils.Network
{
    public class ZMQIPCHelper : IZMQIPCHelper
    {
        private EventHandler<NetMQMessageEventArgs> _onMessage;
        private readonly List<EventHandler<NetMQMessageEventArgs>> _onMessagEventHandlers;

        private readonly DealerSocket _sock;
        private readonly Queue _messagesQueue;
        private readonly NetMQPoller _poller;

        private readonly object mutex = new object();
        private bool _disposing;
        private ISocketConfig _config;

        public ZMQIPCHelper(ISocketConfig config)
        {
            _config = config;
            _onMessagEventHandlers = new List<EventHandler<NetMQMessageEventArgs>>();

            NetMQConfig.Linger = TimeSpan.Zero;

            _sock = new DealerSocket(config.Address);

            _sock.Options.ReceiveHighWatermark = 0;
            _sock.Options.ReceiveLowWatermark = 1;

            _sock.Options.SendHighWatermark = 0;
            _sock.Options.SendLowWatermark = 1;

            _messagesQueue = Queue.Synchronized(new Queue());

            _sock.SendReady += (sender, args) =>
            {
                lock (mutex)
                {
                    if (_disposing) return;
                }

                Thread.Sleep(1); // slow down the poller a bit to decrease the CPU Usage

                while (_messagesQueue.Count != 0)
                {
                    var message = (NetMQMessage) _messagesQueue.Dequeue();
                    args.Socket.SendMultipartMessage(message);
                }
            };

            _sock.ReceiveReady += (sender, args) =>
            {
                lock (mutex)
                {
                    if (_disposing) return;
                }

                var message = _sock.ReceiveMultipartMessage();
                if (_onMessage != null)
                {
                    _onMessage.Invoke(this, new NetMQMessageEventArgs(message));
                }
            };

            _poller = new NetMQPoller {_sock};
        }

        public void Start()
        {
            _poller.RunAsync();
        }

        public event EventHandler<NetMQMessageEventArgs> OnMessage
        {
            add
            {
                _onMessage += value;
                _onMessagEventHandlers.Add(value);
            }
            remove
            {
                _onMessage -= value;
                _onMessagEventHandlers.Remove(value);
            }
        }

        public void Send(NetMQMessage message)
        {
            _messagesQueue.Enqueue(message);
        }

        public void Send(string message)
        {
            var frame = new NetMQFrame(message);
            _messagesQueue.Enqueue(new NetMQMessage(new List<NetMQFrame> {frame}));
        }

        public void Close()
        {
            Dispose();
        }

        ~ZMQIPCHelper()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            lock (mutex)
            {
                _disposing = disposing;
            }

            if (!disposing) return;

            foreach (var eventHandler in _onMessagEventHandlers)
            {
                _onMessage -= eventHandler;
            }

            _onMessagEventHandlers.Clear();
            _messagesQueue.Clear();

            _sock.Close();
            _poller.Stop();

            _sock.Dispose();
            _poller.Dispose();

            NetMQConfig.Cleanup();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
