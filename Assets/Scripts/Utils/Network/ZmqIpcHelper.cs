using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using NetMQ;
using NetMQ.Sockets;
using Zenject;

namespace Utils.Network
{
    [UsedImplicitly]
    public class ZmqIpcHelper
    {
        private readonly Queue _messages;
        private readonly List<EventHandler<NetMqMessageEventArgs>> _onMessageEventHandlers;
        private readonly NetMQPoller _poller;
        private readonly object _pollerAndMqThreadMutex;
        private readonly DealerSocket _sock;
        private bool _disposing;

        private ZmqIpcHelper(ISocketConfig config)
        {
            _pollerAndMqThreadMutex = new object();

            _onMessageEventHandlers = new List<EventHandler<NetMqMessageEventArgs>>();

            _sock = new DealerSocket(config.Address);
            _sock.Options.Linger = TimeSpan.Zero;
            _sock.Options.ReceiveHighWatermark = 0;
            _sock.Options.ReceiveLowWatermark = 1;
            _sock.Options.SendHighWatermark = 0;
            _sock.Options.SendLowWatermark = 1;

            _messages = Queue.Synchronized(new Queue());

            _sock.SendReady += (sender, args) =>
            {
                Thread.Sleep(1); // slow down the poller a bit to decrease the CPU Usage
                while (_messages.Count != 0)
                {
                    var message = (NetMQMessage) _messages.Dequeue();
                    lock (_pollerAndMqThreadMutex)
                    {
                        if (_disposing) return;
                        args.Socket.SendMultipartMessage(message);
                    }
                }
            };

            _sock.ReceiveReady += (sender, args) =>
            {
                NetMQMessage message;

                lock (_pollerAndMqThreadMutex)
                {
                    if (_disposing) return;
                    message = _sock.ReceiveMultipartMessage();
                }

                if (InternalOnMessage != null) InternalOnMessage.Invoke(this, new NetMqMessageEventArgs(message));
            };

            _poller = new NetMQPoller {_sock};
        }

        private event EventHandler<NetMqMessageEventArgs> InternalOnMessage;

        public void Start()
        {
            _poller.RunAsync();
        }

        public event EventHandler<NetMqMessageEventArgs> OnMessage
        {
            add
            {
                InternalOnMessage += value;
                _onMessageEventHandlers.Add(value);
            }
            remove
            {
                InternalOnMessage -= value;
                _onMessageEventHandlers.Remove(value);
            }
        }

        public void Send(NetMQMessage message)
        {
            _messages.Enqueue(message);
        }

        public void Close()
        {
            Dispose();
        }

        ~ZmqIpcHelper()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            lock (_pollerAndMqThreadMutex)
            {
                _disposing = disposing;
                if (!_disposing) return;
            }

            foreach (var eventHandler in _onMessageEventHandlers) InternalOnMessage -= eventHandler;

            _onMessageEventHandlers.Clear();
            _messages.Clear();

            _poller.Remove(_sock);

            lock (_pollerAndMqThreadMutex)
            {
                _sock.Close();
            }

            if (_poller.IsRunning) _poller.Stop();

            _sock.Dispose();
            _poller.Dispose();

            NetMQConfig.Cleanup();
        }

        private void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [UsedImplicitly]
        public class Factory : PlaceholderFactory<ZmqIpcHelper>
        {
        }
    }
}