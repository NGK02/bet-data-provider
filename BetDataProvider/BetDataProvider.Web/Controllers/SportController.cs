using BetDataProvider.Business.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BetDataProvider.Web.Controllers
{
    [ApiController]
    [Route("api/sports")]
    public class SportController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult Get() 
        {
            throw new NotImplementedException();
        }
    }
}
