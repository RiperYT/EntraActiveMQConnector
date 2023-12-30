using ActiveMQ.Artemis.Client;

namespace U_ProxMicrosoftEntraIDConnector.Services.Abstractions
{
    public interface IBrockerService
    {
        public void Connect(string domen, string port, string login, string password);
        public void Send(string message, string queue);
        public bool CheckConnection();
    }
}
