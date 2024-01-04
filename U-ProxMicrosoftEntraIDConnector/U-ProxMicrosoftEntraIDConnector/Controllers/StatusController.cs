using Microsoft.AspNetCore.Mvc;
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
            var entra = await _entraService.CheckConnection() == true ? "connected" : "not connected";
            return $"Entra id: {entra}";
        }

        /*[HttpGet("update")]
        public string Update(string secureToken)
        {
            return "Hello";
        }*/
    }
}
