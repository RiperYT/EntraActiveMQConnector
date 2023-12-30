using Azure.Identity;
using Microsoft.Graph;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Services
{
    public class EntraService : IEntraService
    {
        private static GraphServiceClient? _graphServiceClient;
        public EntraService() { }
        public void Connect(string tenantId, string clientId)
        {
            var scopes = new[] { "User.Read" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            //var tenantId = "common";

            // Value from app registration
            //var clientId = "YOUR_CLIENT_ID"; 

            var options = new DeviceCodeCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                ClientId = clientId,
                TenantId = tenantId,
                // Callback function that receives the user prompt
                // Prompt contains the generated device code that user must
                // enter during the auth process in the browser
                DeviceCodeCallback = (code, cancellation) =>
                {
                    Console.WriteLine(code.Message);
                    return Task.FromResult(0);
                },
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.devicecodecredential
            var deviceCodeCredential = new DeviceCodeCredential(options);

            _graphServiceClient = new GraphServiceClient(deviceCodeCredential, scopes);

            var result = _graphServiceClient.External.Connections["{externalConnection-id}"];
        }

        public bool CheckConnection()
        {
            return _graphServiceClient != null;
        }

        public async Task<List<UserEntity>> GetAllUsers()
        {
            if (_graphServiceClient == null)
                return new List<UserEntity>();

            var users = await _graphServiceClient.Users.GetAsync();

            if (users == null)
                return new List<UserEntity>();

            if (users.Value == null)
                return new List<UserEntity>();

            var answer = new List<UserEntity>();
            foreach (var user in users.Value)
            {
                if (user.Id != null)
                    answer.Add(new UserEntity(user.Id,
                                              user.Surname == null ? "" : user.Surname,
                                              user.GivenName == null ? "" : user.GivenName,
                                              user.JobTitle == null ? "" : user.JobTitle,
                                              user.Mail == null ? "" : user.Mail,
                                              user.MobilePhone == null ? "" : user.MobilePhone,
                                              DateTime.Now));
            }

            return answer;
        }

        //ToDO
        public async Task<List<UserEntity>> GetDeltaUsers()
        {
            if (_graphServiceClient == null)
                return new List<UserEntity>();

            var users = await _graphServiceClient.Users.GetAsync();

            if (users == null)
                return new List<UserEntity>();

            if (users.Value == null)
                return new List<UserEntity>();

            var answer = new List<UserEntity>();
            foreach (var user in users.Value)
            {               
                if (user.Id != null)
                    answer.Add(new UserEntity(user.Id,
                                              user.Surname == null ? "" : user.Surname,
                                              user.GivenName == null ? "" : user.GivenName,
                                              user.JobTitle == null ? "" : user.JobTitle,
                                              user.Mail == null ? "" : user.Mail,
                                              user.MobilePhone == null ? "" : user.MobilePhone,
                                              DateTime.Now));
            }

            return new List<UserEntity>();
        }
    }
}
