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
            var answer = await GetMail();
            if (answer.Contains('@'))
                return true;
            else
                return false;
            /*Console.WriteLine(StaticConnections.GraphServiceClient == null);
            if (StaticConnections.GraphServiceClient != null)
            {
                var answer = await StaticConnections.GraphServiceClient.Me.GetAsync();

                //var task = StaticConnections.GraphServiceClient.Me.GetAsync();

                //var completedTask = await Task.WhenAny(StaticConnections.GraphServiceClient.Me.GetAsync(), Task.Delay(TimeSpan.FromSeconds(1)));

                //if (completedTask == task)
                if (answer != null)
                {
                    //var answer = await task;
                    if (answer.Mail.Contains('@'))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }*/
        }

        public async Task<List<UserEntity>> GetAllUsers()
        {
            /*if (_graphServiceClient == null)
                return new List<UserEntity>();

            var users = await _graphServiceClient.Users.GetAsync();*/

            if (StaticConnections.GraphServiceClient == null)
                return new List<UserEntity>();

            var users = await StaticConnections.GraphServiceClient.Users.GetAsync();

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
            /*if (_graphServiceClient == null)
                return new List<UserEntity>();

            var users = await _graphServiceClient.Users.GetAsync();*/

            if (StaticConnections.GraphServiceClient == null)
                return new List<UserEntity>();

            var users = await StaticConnections.GraphServiceClient.Users.GetAsync();

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

        private string GetCode()
        {
            try
            {
                //if (_graphServiceClient != null)
                if (StaticConnections.GraphServiceClient != null)
                {
                    FileStream fileStream = new FileStream("console.txt", FileMode.Create);
                    StreamWriter streamWriter = new StreamWriter(fileStream);
                    var oldWriter = Console.Out;
                    Console.SetOut(streamWriter);

                    Console.Clear();
                    //var users = _graphServiceClient.Me.GetAsync();
                    var users = StaticConnections.GraphServiceClient.Me.GetAsync();
                    Thread.Sleep(1000);

                    streamWriter.Close();
                    fileStream.Close();
                    /*var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                    standardOutput.AutoFlush = true;
                    Console.SetOut(standardOutput);*/
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
                Console.WriteLine(ex.Message);
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

                    //var task = StaticConnections.GraphServiceClient.Me.GetAsync();

                    //var completedTask = await Task.WhenAny(StaticConnections.GraphServiceClient.Me.GetAsync(), Task.Delay(TimeSpan.FromSeconds(1)));

                    //if (completedTask == task)
                    if (answer != null)
                    {
                        //var answer = await task;
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
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
