using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using U_ProxMicrosoftEntraIDConnector.Common;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

namespace U_ProxMicrosoftEntraIDConnector.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IEntraService _entraService;
        private readonly IBrockerService _brockerService;
        public StatusController(IEntraService entraService, IBrockerService brockerService)
        {
            _entraService = entraService;
            _brockerService = brockerService;
        }

        [HttpGet("getStatus")]
        public async Task<string> Status(string secureToken)
        {
            try
            {
                if (!secureToken.Equals(StaticConnections.SecureToken))
                {
                    StaticConnections.Logger.Warn("Status controller, secure token is not correct");
                    throw new Exception("Not correct secure token");
                }
                var entra = await _entraService.CheckConnection() == true ? "connected" : "not connected";
                var brocker = await _brockerService.CheckConnection() == true ? "connected" : "not connected";
                return $"Entra id: {entra}\nBrocker : {brocker}";
            }
            catch (Exception ex)
            {
                    StaticConnections.Logger.Error(ex);
                    return ex.Message;
            }
        }

        /*[HttpGet("update")]
        public string Update(string secureToken)
        {
            return "Hello";
        }*/
    }
}
