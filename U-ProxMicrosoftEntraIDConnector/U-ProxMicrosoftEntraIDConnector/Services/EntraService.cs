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
        private GraphServiceClient? _graphServiceClient;
        //public static EntraService() { }
        public string Connect(string tenantId, string clientId)
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

            //_graphServiceClient = new GraphServiceClient(deviceCodeCredential, scopes);
            StaticConnections.GraphServiceClient = new GraphServiceClient(deviceCodeCredential, scopes);

            //var result = _graphServiceClient.External.Connections["{externalConnection-id}"];
            var result = StaticConnections.GraphServiceClient.External.Connections["{externalConnection-id}"];


            return GetCode();
        }

        public async Task<bool> CheckConnection()
        {
            /*var answer = await GetMail();
            if (answer.Contains('@'))
                return true;
            else
                return false;*/
            return StaticConnections.IsConnected;
        }

        public async Task<bool> ConfirmConnection()
        {
            try
            {
                if (StaticConnections.GraphServiceClient != null)
                {
                    var answer = await GetAllUsers();
                    if (answer.Count > 0)
                    {
                        StaticConnections.IsConnected = true;
                        return true;
                    }
                    else
                    {
                        StaticConnections.IsConnected = false;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"{ex.Message}");
                StaticConnections.Logger.Error(ex);
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

            /*var users2 = await StaticConnections.GraphServiceClient.Users.GetAsync();
            foreach (var user in users2.Value)
            {
                if (!user.Id.Equals("0f729d98-fc17-4cca-982a-1580829b2006"))
                {
                    //Console.WriteLine(user.acc);
                    user.AccountEnabled = false;
                    await StaticConnections.GraphServiceClient.Users[user.Id].PatchAsync(user);
                }
            }*/
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

        [Obsolete]
        public async Task<List<UserEntity>> GetDeltaUsers(DateTime filterDate)
        {
            if (StaticConnections.GraphServiceClient == null)
                return new List<UserEntity>();

            var users = await StaticConnections.GraphServiceClient.Users.Delta.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "id", "surname", "givenName", "jobTitle", "userPrincipalName", "mobilePhone"};
                //requestConfiguration.QueryParameters.Filter = $"accountEnabled eq true and createdDateTime gt {filterDate.ToString("s") + "Z"}";
                //requestConfiguration.QueryParameters.Filter = $"createdDateTime gt {filterDate.ToString("s") + "Z"}";
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

            return new List<UserEntity>();
        }

        private string GetCode()
        {
            try
            {
                if (StaticConnections.GraphServiceClient != null)
                {
                    FileStream fileStream = new FileStream("console.txt", FileMode.Create);
                    StreamWriter streamWriter = new StreamWriter(fileStream);
                    var oldWriter = Console.Out;
                    Console.SetOut(streamWriter);

                    Console.Clear();
                    var users = StaticConnections.GraphServiceClient.Me.GetAsync();
                    Thread.Sleep(1000);

                    streamWriter.Close();
                    fileStream.Close();
                    Console.SetOut(oldWriter);

                    Console.WriteLine("Code end.");

                    return File.ReadAllText("console.txt");
                }
                else
                {
                    return "Not connected";
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                StaticConnections.Logger.Error(ex);
                return "Not connected ERROR";
            }
        }

        private async Task<string> GetMail()
        {
            try
            {
                if (StaticConnections.GraphServiceClient != null)
                {
                    var answer = await StaticConnections.GraphServiceClient.Me.GetAsync();
                    if (answer != null)
                    {
                        if (answer.Mail != null)
                            return answer.Mail;
                        else
                            return "";
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                StaticConnections.Logger.Error(ex);
                return "";
            }
        }
    }
}
