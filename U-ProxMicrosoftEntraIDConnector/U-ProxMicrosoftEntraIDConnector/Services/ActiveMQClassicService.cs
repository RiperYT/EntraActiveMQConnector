using ActiveMQ.Artemis.Client;
using Amqp.Framing;
using System.Net.Http.Headers;
using System.Text;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Services
{
    public class ActiveMQClassicService : IBrockerService
    {
        private static IConnection? _connection;
        private readonly ISettingsRepository _settingsRepository;
        public ActiveMQClassicService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<bool> Connect(string domen, string port, string login, string password)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"http://{domen}:{port}/api/message/"))
                    {
                        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{password}"));
                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                        request.Content = new StringContent("");
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                        var response = await httpClient.SendAsync(request);
                        var answer = await response.Content.ReadAsStringAsync();
                        if (!answer.Contains("HTTP ERROR 500"))
                            return false;
                        else
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> Send(string message, string queue)
        {
            try
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"http://{settings.DomenBrocker}:{settings.PortBroker}/api/message/{queue}?type=queue"))
                        {
                            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.UsernameBroker}:{settings.PasswordBroker}"));
                            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                            request.Content = new StringContent($"body={message}");
                            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                            var response = await httpClient.SendAsync(request);
                            var answer = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(answer);
                            if (!answer.Contains("Message sent"))
                                return false;
                            else
                                return true;
                        }
                    }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> CheckConnection()
        {
            try
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"http://{settings.DomenBrocker}:{settings.PortBroker}/api/message/"))
                        {
                            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.UsernameBroker}:{settings.PasswordBroker}"));
                            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                            request.Content = new StringContent("");
                            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                            var response = await httpClient.SendAsync(request);
                            var answer = await response.Content.ReadAsStringAsync();
                            if (answer.Contains("HTTP ERROR 500"))
                                return true;
                            else
                                return false;
                        }
                    }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
