using ActiveMQ.Artemis.Client;
using U_ProxMicrosoftEntraIDConnector.Common;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Services
{
    public class ArtemisService : IBrockerService
    {
        public ArtemisService()
        {
        }

        public async Task<bool> Connect(string domen, string port, string login, string password)
        {
            try
            {
                var connectionFactory = new ConnectionFactory();
                var endpoint = ActiveMQ.Artemis.Client.Endpoint.Create(domen, int.Parse(port), login, password);
                StaticConnections.Connection = await connectionFactory.CreateAsync(endpoint);
                return StaticConnections.Connection.IsOpened;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                StaticConnections.Logger.Error(ex);
                return false;
            }
        }
        public async Task<bool> Send(string message, string queue)
        {
            try
            {
                var producer = await StaticConnections.Connection.CreateProducerAsync(queue, RoutingType.Anycast);
                await producer.SendAsync(new Message(message));
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                StaticConnections.Logger.Error(ex);
                return false;
            }
        }
        public async Task<bool> CheckConnection()
        {
            return StaticConnections.Connection == null ? false : StaticConnections.Connection.IsOpened;
        }
    }
}
