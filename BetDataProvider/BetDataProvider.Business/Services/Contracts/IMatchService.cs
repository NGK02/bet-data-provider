using BetDataProvider.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.Business.Services.Contracts
{
    public interface IMatchService
    {
        Match GetMatchByXmlId(int xmlId);

        List<Match> GetUpcomingMatchesWithPreviewBets(double? hoursAhead/*, bool? isActiveData*/);
    }
}
