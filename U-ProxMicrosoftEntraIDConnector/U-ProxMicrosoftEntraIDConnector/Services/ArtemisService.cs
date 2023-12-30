using ActiveMQ.Artemis.Client;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Services
{
    public class ArtemisService : IBrockerService
    {
        private static IConnection? _connection;
        public async void Connect(string domen, string port, string login, string password)
        {
            var connectionFactory = new ConnectionFactory();
            var endpoint = ActiveMQ.Artemis.Client.Endpoint.Create(domen, int.Parse(port), login, password);
            _connection = await connectionFactory.CreateAsync(endpoint);
        }
        public async void Send(string message, string queue)
        {
            var producer = await _connection.CreateProducerAsync("a1", RoutingType.Anycast);
            await producer.SendAsync(new Message(message));
        }
        public bool CheckConnection()
        {
            return _connection != null;
        }
    }
}
