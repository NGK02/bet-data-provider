using AutoMapper;
using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
            var match = _matchService.GetMatchByXmlId(xmlId);

            if (match is null)
            {
                return NotFound(match);
            }

            var matchDto = _mapper.Map<GetMatchDto>(match);

            return Ok(matchDto);
        }

        [HttpGet("")]
        public IActionResult GetUpcomingMatchesWithPreviewBets([FromQuery] double? hoursAhead)
        {
            var matches = _matchService.GetUpcomingMatchesWithPreviewBets(hoursAhead);

            if (matches.IsNullOrEmpty())
            {
                return NotFound(matches);
            }

            var matchDto = matches.Select(m => _mapper.Map<GetMatchDto>(m));

            return Ok(matchDto);
        }
    }
}
