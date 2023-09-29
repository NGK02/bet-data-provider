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
        bool AddSportData(Sport sportData);

        Sport GetActiveSportData();

        Sport GetAllSportData();

        Event GetEventByXmlId(int xmlId);

        bool UpdateSportData(Sport sportData);

        void AddMatchChangeMessage(int matchId, MessageType type);

        void AddBetChangeMessage(int betId, MessageType type);

        void AddOddChangeMessage(int oddId, MessageType type);

        bool SaveChanges();
    }
}
