using Microsoft.AspNetCore.Mvc;

namespace U_ProxMicrosoftEntraIDConnector.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet("getStatus")]
        public string Status(string secureToken)
        {
            return "Hello";
        }

        [HttpGet("update")]
        public string Update(string secureToken)
        {
            return "Hello";
        }
    }
}
