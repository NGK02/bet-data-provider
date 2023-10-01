using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Models.Enums;
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
        Task AddSportDataAsync(Sport sportData);

        Task<Sport> GetActiveSportDataAsync();

        Task<Sport> GetAllSportDataAsync();

        Task<Event> GetEventByXmlIdAsync(int xmlId);

        void UpdateSportData(Sport sportData);

        Task AddMatchChangeMessageAsync(int matchId, MessageType type);

        Task AddBetChangeMessageAsync(int betId, MessageType type);

        Task AddOddChangeMessageAsync(int oddId, MessageType type);

        Task SaveChangesAsync();
    }
}
