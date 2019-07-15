using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PrivateSausage.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class BotController : ControllerBase
    {
        [HttpGet("status")]
        public async Task<ActionResult> Get()
        {
            return Ok();
        }
    }
}
