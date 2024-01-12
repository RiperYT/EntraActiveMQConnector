using Azure.Identity;
using Microsoft.Graph;
using System.Threading.Tasks;
using System.Threading;
using U_ProxMicrosoftEntraIDConnector.Common;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Services
{
    public class EntraService : IEntraService
    {
        public async Task<bool> Connect(string tenantId, string clientId, string clientSecret)
        {
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var options = new DeviceCodeCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                DeviceCodeCallback = (code, cancellation) =>
                {
                    Console.WriteLine(code.Message);
                    return Task.FromResult(0);
                },
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            StaticConnections.GraphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);

            var result = StaticConnections.GraphServiceClient.External.Connections["{externalConnection-id}"];

            var success = await CheckConnection();
            if (success)
                return true;
            else
            {
                StaticConnections.GraphServiceClient = null;
                return false;
            }
        }

        public async Task<bool> CheckConnection()
        {
            if (StaticConnections.GraphServiceClient != null)
            {
                var users = await StaticConnections.GraphServiceClient.Users.GetAsync();
                if (users == null)
                    return false;
                else
                {
                    if (users.Value != null)
                        return users.Value.Count > 0;
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<List<UserEntity>> GetAllUsers()
        {

            if (StaticConnections.GraphServiceClient == null)
                return new List<UserEntity>();

            var users = await StaticConnections.GraphServiceClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "id", "surname", "givenName", "jobTitle", "userPrincipalName", "mobilePhone" };
            });

            if (users == null)
                return new List<UserEntity>();

            if (users.Value == null)
                return new List<UserEntity>();

            var answer = new List<UserEntity>();
            foreach (var user in users.Value)
            {

                if (user.Id != null)
                    answer.Add(new UserEntity(user.Id,
                                              user.Surname ?? "",
                                              user.GivenName ?? "",
                                              user.JobTitle ?? "",
                                              user.UserPrincipalName ?? "",
                                              user.MobilePhone ?? "",
                                              DateTime.UtcNow));
            }

            return answer;
        }
    }
}
