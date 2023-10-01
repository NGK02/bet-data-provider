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
        public async Task<IActionResult> GetMatchByXmlIdAsync(int xmlId) 
        {
            var match = await _matchService.GetMatchByXmlIdAsync(xmlId);

            if (match is null)
            {
                return NotFound(match);
            }

            var matchDto = _mapper.Map<GetMatchDto>(match);

            return Ok(matchDto);
        }

        [HttpGet("in24Hours")]
        public async Task<IActionResult> GetUpcomingMatchesWithPreviewBets()
        {
            var matches = await _matchService.GetUpcomingMatchesWithPreviewBetsAsync();

            if (matches.IsNullOrEmpty())
            {
                return NotFound(matches);
            }

            var matchDto = matches.Select(m => _mapper.Map<GetMatchDto>(m));

            return Ok(matchDto);
        }
    }
}
