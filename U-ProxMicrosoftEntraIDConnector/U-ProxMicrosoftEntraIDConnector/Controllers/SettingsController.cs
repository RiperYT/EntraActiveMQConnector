using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using U_ProxMicrosoftEntraIDConnector.Common;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IEntraService _entraService;
        private readonly IBrockerService _brockerService;
        private readonly ISettingsRepository _settingsRepository;
        public SettingsController(ISettingsRepository settingsRepository, IEntraService entraService, IBrockerService brockerService)
        {
            _entraService = entraService;
            _brockerService = brockerService;
            _settingsRepository = settingsRepository;
        }

        [HttpGet("setEntraId")]
        public async Task<string> SetEntraId(string secureToken, string tenantId, string clientId)
        {
            try
            {
                if (!secureToken.Equals(StaticConnections.SecureToken))
                    throw new Exception("Not correct secure token");
                return _entraService.Connect(tenantId, clientId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet("confirmEntraId")]
        public async Task<string> ConfirmEntraId(string secureToken)
        {
            try
            {
                if (!secureToken.Equals(StaticConnections.SecureToken))
                    throw new Exception("Not correct secure token");

                return await _entraService.ConfirmConnection() ? "Confirmed" : "Not confirmed, try new connection";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet("setActiveMQ")]
        public async Task<string> SignActiveMQ(string secureToken, string domen, string port, string username, string password, string queueName)
        {
            try
            {
                if (!secureToken.Equals(StaticConnections.SecureToken))
                    throw new Exception("Not correct secure token");
                if (!await _brockerService.Connect(domen, port, username, password))
                    throw new Exception("Cannot connect");
                //Thread.Sleep(5000);
                var settings = _settingsRepository.Get();
                if (settings == null)
                {
                    settings = new SettingEntity(domen, port, username, password, queueName, DateTime.MinValue, DateTime.MinValue);
                }
                else
                {
                    settings.DomenBrocker = domen;
                    settings.PortBroker = port;
                    settings.UsernameBroker = username;
                    settings.PasswordBroker = password;
                    settings.QueueName = queueName;
                }
                _settingsRepository.Add(settings);
                return "Success connected";
            }
            catch (Exception ex)
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                    await _brockerService.Connect(settings.DomenBrocker, settings.PortBroker, settings.UsernameBroker, settings.PasswordBroker);
                return ex.Message;
            }
        }
    }
}
