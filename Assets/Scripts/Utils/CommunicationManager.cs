using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using NetMQ;
using UniRx;
using UnityEngine;
using Utils.Network;
using Utils.StateMachine.Parser;
using Zenject;

namespace Utils
{
    [UsedImplicitly]
    public class CommunicationManager : ILateDisposable
    {
        private readonly ZmqIpcManager _zmqIpcManager;

        public CommunicationManager(ZmqIpcManager zmqIpcManager)
        {
            _zmqIpcManager = zmqIpcManager;
        }

        private void Send(string message)
        {
            _zmqIpcManager.Send(new NetMQMessage(new List<NetMQFrame>
            {
                new NetMQFrame(message, Encoding.UTF8)
            }));
        }

        public void SendJson(object value)
        {
            Send(Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }

        public IObservable<object> GetObservableForType(string type)
        {
            return _zmqIpcManager.MessageObservable.Select(message => message.First.ConvertToString(Encoding.UTF8))
                .Select(JsonHelper.Deserialize)
                .Select(obj => (IDictionary<string, object>) obj)
                .Where(obj => (string) obj["type"] == type)
                .Select(obj => obj["value"]);
        }

        public void LateDispose()
        {
        }
    }
}
