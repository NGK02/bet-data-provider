using BetDataProvider.Business.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BetDataProvider.Web.Controllers
{
    [ApiController]
    [Route("api/sports")]
    public class SportController : ControllerBase
    {
        private readonly IXmlHandler _xmlHandler;

        public SportController(IXmlHandler xmlHandler) 
        {
            _xmlHandler = xmlHandler;
        }

        [HttpGet("")]
        public IActionResult Get() 
        {
            return Ok();
        }
    }
}
