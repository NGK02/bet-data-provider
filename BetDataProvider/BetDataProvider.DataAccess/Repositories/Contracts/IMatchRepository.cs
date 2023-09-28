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

        List<Match> GetMatches(int? hoursAhead, bool? isActiveData);
    }
}
