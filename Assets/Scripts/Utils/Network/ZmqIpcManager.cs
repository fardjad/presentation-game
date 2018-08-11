using System.Collections;
using System.Threading;
using AsyncIO;
using JetBrains.Annotations;
using NetMQ;
using UniRx;
using Zenject;

namespace Utils.Network
{
    [UsedImplicitly]
    public class ZmqIpcManager : IInitializable, ITickable, ILateDisposable
    {
        private readonly Thread _clientThread;
        private readonly Queue _messages;
        private readonly Subject<NetMQMessage> _messageSubject;
        private readonly object _mutex;
        private readonly ZmqIpcHelper.Factory _zmqIpcHelperFactory;
        private bool _shouldTerminate;

        private ZmqIpcHelper _zmqIpcHelper;

        public ZmqIpcManager(ZmqIpcHelper.Factory zmqIpcHelperFactory)
        {
            _mutex = new object();
            _messageSubject = new Subject<NetMQMessage>();
            _messages = Queue.Synchronized(new Queue());
            _zmqIpcHelperFactory = zmqIpcHelperFactory;
            _clientThread = new Thread(StartNetMq);
        }

        public IObservable<NetMQMessage> MessageObservable
        {
            get { return _messageSubject; }
        }

        public void Initialize()
        {
            ForceDotNet.Force();
            _zmqIpcHelper = _zmqIpcHelperFactory.Create();
            _clientThread.Start();
            _zmqIpcHelper.OnMessage += (s, a) =>
            {
                var message = a.Message;
                _messages.Enqueue(message);
            };
        }

        public void LateDispose()
        {
            lock (_mutex)
            {
                _shouldTerminate = true;
                _messageSubject.OnCompleted();
            }

            if (_clientThread.IsAlive) _clientThread.Join();
        }

        public void Tick()
        {
            while (_messages.Count > 0)
            {
                var message = (NetMQMessage) _messages.Dequeue();
                lock (_mutex)
                {
                    if (_shouldTerminate) return;
                    _messageSubject.OnNext(message);
                }
            }
        }

        public void Send(NetMQMessage message)
        {
            _zmqIpcHelper.Send(message);
        }

        private void StartNetMq()
        {
            _zmqIpcHelper.Start();
            while (true)
            {
                lock (_mutex)
                {
                    if (_shouldTerminate)
                    {
                        _zmqIpcHelper.Close();
                        break;
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}