using BetDataProvider.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Repositories.Contracts
{
    public interface ISportRepository
    {
        Match GetMatchByXmlId(int xmlId);

        Bet GetBetByXmlId(int xmlId);

        Odd GetOddByXmlId(int xmlId);

        bool AddSportData(Sport sportData);

        Sport GetActiveSportData();

        Sport GetSportData();

        bool UpdateSportData(Sport sportData);

        bool UpdateEventData(ICollection<Event> events);

        bool SaveChanges();
    }
}
