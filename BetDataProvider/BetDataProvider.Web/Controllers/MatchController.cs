using AutoMapper;
using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.Web.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BetDataProvider.Web.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly IMapper _mapper;

        public MatchController(IMatchService matchService, IMapper mapper) 
        {
            this._matchService = matchService;
            this._mapper = mapper;
        }

        [HttpGet("{xmlId}")]
        public IActionResult GetMatchByXmlId(int xmlId) 
        {
            var matchDto = _mapper.Map<GetMatchDto>(_matchService.GetMatchByXmlId(xmlId));
            return Ok(matchDto);
        }

        [HttpGet("")]
        public IActionResult GetUpcomingMatches([FromQuery] string hoursAhead)
        {
            //var matchDto = _mapper.Map<GetMatchDto>(_matchService.GetUpcomingMatches(hoursAhead));
            //return Ok(matchDto);
            throw new NotImplementedException();
        }
    }
}
