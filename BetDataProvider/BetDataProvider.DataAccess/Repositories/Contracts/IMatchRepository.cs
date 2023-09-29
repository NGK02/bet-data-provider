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
        Match GetMatchByXmlId(int xmlId);

        Bet GetBetByXmlId(int xmlId);

        Odd GetOddByXmlId(int xmlId);

        List<Match> GetUpcomingMatchesWithPreviewBets();
    }
}
