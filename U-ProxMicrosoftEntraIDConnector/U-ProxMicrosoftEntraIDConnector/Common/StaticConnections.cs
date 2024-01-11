using ActiveMQ.Artemis.Client;
using Microsoft.Graph;

namespace U_ProxMicrosoftEntraIDConnector.Common
{
    public static class StaticConnections
    {
        public static readonly string SecureToken = "123";
        public static GraphServiceClient? GraphServiceClient;
        public static IConnection? Connection;
        public static bool IsConnected = false;
        public static NLog.Logger Logger;
    }
}
