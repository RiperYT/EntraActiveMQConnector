using Azure.Identity;
using Microsoft.Graph;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Services.Abstractions
{
    public interface IEntraService
    {
        public string Connect(string tenantId, string clientId);

        public Task<bool> CheckConnection();
        public Task<bool> ConfirmConnection();

        public Task<List<UserEntity>> GetAllUsers();

        public Task<List<UserEntity>> GetDeltaUsers(DateTime filterDate);
    }
}
