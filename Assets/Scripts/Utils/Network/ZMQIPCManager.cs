using System.Collections;
using System.Threading;
using AsyncIO;
using NetMQ;
using Newtonsoft.Json;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Network.Json;

namespace Utils.Network
{
    internal class SocketConfig : ISocketConfig
    {
        private const string PROTOCOL = "tcp";
        private const string HOST = "localhost";
        private const int PORT = 3000;

        public string Address
        {
            get { return string.Format(">{0}://{1}:{2}", PROTOCOL, HOST, PORT); }
        }
    }

    public class ZMQIPCManager : MonoBehaviour
    {
        private static readonly Object _mutex = new Object();

        private IZMQIPCHelper _zmqipcHelper;
        private Thread _clientThread;
        private bool _shouldTerminate;
        private volatile bool _threadStarted;

        private Queue _messages;

        public IObservable<NetMQMessage> MessageObservable
        {
            get { return _subject; }
        }

        private readonly Subject<NetMQMessage> _subject;

        public ZMQIPCManager()
        {
            // I had to create a Subject in constructor because other scripts might attempt to subscribe to
            // MessageObservable before Start() gets called
            _subject = new Subject<NetMQMessage>();
        }

        private void Start()
        {
            ForceDotNet.Force();

            _zmqipcHelper = new ZMQIPCHelper(new SocketConfig());
            _clientThread = new Thread(startNetMQ);

            _clientThread.Start();

            _messages = Queue.Synchronized(new Queue());

            _zmqipcHelper.OnMessage += (s, a) =>
            {
                var message = a.Message;
                var str = message.First.ConvertToString();
                if (!JsonValidate.Validate(str)) return;
                _messages.Enqueue(message);
            };

            // For some reason, subscribing to an observable created from an EventHandler with ObserveOnMainThread
            // causes UniRX to raise exceptions randomly. The error message says it cannot find any
            // MainThreadDispatcher objects in the scene. I tried for hours but couldn't make it work
            var messagesObservableDisposable = this.UpdateAsObservable()
                .Select(_ => _messages)
                .Subscribe(q =>
                {
                    while (q.Count > 0)
                    {
                        _subject.OnNext((NetMQMessage) q.Dequeue());
                    }
                });

            Observable.OnceApplicationQuit().Subscribe(_ => messagesObservableDisposable.Dispose());
        }


        public void Send<T>(T action) where T : NetAction
        {
            _zmqipcHelper.Send(JsonConvert.SerializeObject(action));
        }

        private void startNetMQ()
        {
            _threadStarted = true;

            _zmqipcHelper.Start();

            while (true)
            {
                lock (_mutex)
                {
                    if (_shouldTerminate)
                    {
                        _zmqipcHelper.Close();
                        NetMQConfig.Cleanup(false);
                        break;
                    }
                }

                Thread.Sleep(1000);
            }

            _threadStarted = false;
        }

        private void OnDisable()
        {
            _subject.OnCompleted();

            if (!_threadStarted)
            {
                return;
            }

            lock (_mutex)
            {
                _shouldTerminate = true;
            }

            _clientThread.Join();
        }
    }
}
