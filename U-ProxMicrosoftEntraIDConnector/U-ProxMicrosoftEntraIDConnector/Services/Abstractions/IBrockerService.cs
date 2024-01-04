using ActiveMQ.Artemis.Client;

namespace U_ProxMicrosoftEntraIDConnector.Services.Abstractions
{
    public interface IBrockerService
    {
        public Task<bool> Connect(string domen, string port, string login, string password);
        public Task<bool> Send(string message, string queue);
        public Task<bool> CheckConnection();
    }
}
