using Azure.Identity;
using Microsoft.Graph;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Services.Abstractions
{
    public interface IEntraService
    {
        public Task<bool> Connect(string tenantId, string clientId, string clientSecret);

        public Task<bool> CheckConnection();

        public Task<List<UserEntity>> GetAllUsers();
    }
}
