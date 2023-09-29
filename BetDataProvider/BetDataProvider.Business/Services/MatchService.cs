using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.Business.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchService(IMatchRepository matchRepository) 
        {
            this._matchRepository = matchRepository;
        }

        public Match GetMatchByXmlId(int xmlId) 
        {
            return _matchRepository.GetMatchByXmlId(xmlId);
        }

        public List<Match> GetUpcomingMatchesWithPreviewBets()
        {
           return _matchRepository.GetUpcomingMatchesWithPreviewBets();
        }
    }
}
