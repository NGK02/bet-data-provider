using BetDataProvider.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Repositories.Contracts
{
    public interface ISportRepository
    {
        bool SaveSportData(Sport sportData);

        Sport GetActiveSportData();

        Sport GetSportData();

        bool UpdateSportData(Sport sportData);

        bool UpdateEventData(ICollection<Event> events);

        bool SaveChanges();
    }
}
