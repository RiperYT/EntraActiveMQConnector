using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
        public string SetEntraId(string secureToken, string tenantId, string clientId)
        {
            try
            {
                _entraService.Connect(tenantId, clientId);
                var settings = _settingsRepository.Get();
                if (settings == null)
                {
                    settings = new SettingsEntity("", "", "", "", tenantId, clientId);
                }
                else
                {
                    settings.TenatIdEntra = tenantId;
                    settings.ClientIdEntra = clientId;
                }
                _settingsRepository.Add(settings);
                return "Success connected";
            }
            catch (Exception ex)
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                    _entraService.Connect(settings.TenatIdEntra, settings.ClientIdEntra);
                return ex.Message;
            }
        }

        [HttpGet("setActiveMQ")]
        public string SignActiveMQ(string token, string domen, string port, string username, string password)
        {
            try
            {
                _brockerService.Connect(domen, port, username, password);
                //Thread.Sleep(5000);
                var settings = _settingsRepository.Get();
                if (settings == null)
                {
                    settings = new SettingsEntity(domen, port, username, password, "", "");
                }
                else
                {
                    settings.DomenBrocker = domen;
                    settings.PortBroker = port;
                    settings.UsernameBroker = username;
                    settings.PasswordBroker = password;
                }
                _settingsRepository.Add(settings);
                //_brockerService.Send("Hello", "q");
                return "Success connected";
            }
            catch (Exception ex)
            {
                var settings = _settingsRepository.Get();
                if (settings != null)
                    _brockerService.Connect(settings.DomenBrocker, settings.PortBroker, settings.UsernameBroker, settings.PasswordBroker);
                return ex.Message;
            }
        }
    }
}
