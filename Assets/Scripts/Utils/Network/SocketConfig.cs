using JetBrains.Annotations;

namespace Utils.Network
{
    [UsedImplicitly]
    public class SocketConfig : ISocketConfig
    {
        private const string Protocol = "tcp";
        private const string Host = "localhost";
        private const int Port = 3000;

        public string Address
        {
            get { return string.Format(">{0}://{1}:{2}", Protocol, Host, Port); }
        }
    }
}
