using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<string> SetEntraId(string secureToken, string tenantId, string clientId, string clientSecret)
        {
            try
            {
                if (!secureToken.Equals(StaticConnections.SecureToken))
                {
                    StaticConnections.Logger.Warn("EntraId set controller, secure token is not correct");
                    throw new Exception("Not correct secure token");
                }

                StaticConnections.Logger.Info("EntraId set controller");
                var success = await _entraService.Connect(tenantId, clientId, clientSecret);
                if (success)
                {
                    var settings = _settingsRepository.Get();
                    if (settings == null)
                    {
                        settings = new SettingEntity("", "", "", "", "", DateTime.MinValue, DateTime.MinValue, "", "", "");
                    }
                    else
                    {
                        settings.TenatId = tenantId;
                        settings.ClientId = clientId;
                        settings.ClientSecret = clientSecret;
                    }
                    _settingsRepository.Add(settings);
                    return "Connected";
                }
                else
                    return "Not connected";
            }
            catch (Exception ex)
            {
                StaticConnections.Logger.Error(ex);
                var settings = _settingsRepository.Get();
                if (settings != null)
                    if (!settings.TenatId.IsNullOrEmpty())
                        await _entraService.Connect(settings.TenatId, settings.ClientId, settings.ClientSecret);
                return ex.Message;
            }
        }

        [HttpGet("setActiveMQ")]
        public async Task<string> SignActiveMQ(string secureToken, string domen, string port, string username, string password, string queueName)
        {
            try
            {
                StaticConnections.Logger.Info("ActiveMQ connnect controller");
                if (!secureToken.Equals(StaticConnections.SecureToken))
                {
                    StaticConnections.Logger.Warn("ActiveMQ controller, secure token is not correct");
                    throw new Exception("Not correct secure token");
                }

                if (!await _brockerService.Connect(domen, port, username, password))
                    throw new Exception("Cannot connect");
                //Thread.Sleep(5000);
                var settings = _settingsRepository.Get();
                if (settings == null)
                {
                    settings = new SettingEntity(domen, port, username, password, queueName, DateTime.MinValue, DateTime.MinValue, "", "", "");
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
                StaticConnections.Logger.Error(ex);
                var settings = _settingsRepository.Get();
                if (settings != null)
                    if (!settings.DomenBrocker.IsNullOrEmpty())
                        await _brockerService.Connect(settings.DomenBrocker, settings.PortBroker, settings.UsernameBroker, settings.PasswordBroker);
                return ex.Message;
            }
        }
    }
}
