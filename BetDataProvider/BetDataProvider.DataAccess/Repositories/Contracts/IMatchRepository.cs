using BetDataProvider.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Repositories.Contracts
{
    public interface IMatchRepository
    {
        Task<Match> GetMatchByXmlIdAsync(int xmlId);

        Task<Bet> GetBetByXmlIdAsync(int xmlId);

        Task<Odd> GetOddByXmlIdAsync(int xmlId);

        Task<List<Match>> GetUpcomingMatchesWithPreviewBetsAsync();
    }
}
