using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Repositories
{
    public class SportRepository : ISportRepository
    {
        private readonly BetDataDbContext _dbContext;

        public SportRepository(BetDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Match GetMatchByXmlId(int xmlId) 
        {
            var matchingMatch = _dbContext.Matches.AsNoTracking().Include(m => m.Bets).ThenInclude(b => b.Odds).FirstOrDefault(m => m.XmlId == xmlId);
            return matchingMatch;
        }

        public Bet GetBetByXmlId(int xmlId)
        {
            var matchingBet = _dbContext.Bets.AsNoTracking().Include(b => b.Odds).FirstOrDefault(b => b.XmlId == xmlId);
            return matchingBet;
        }

        public Odd GetOddByXmlId(int xmlId)
        {
            var matchingOdd = _dbContext.Odds.AsNoTracking().FirstOrDefault(o => o.XmlId == xmlId);
            return matchingOdd;
        }

        public Sport GetActiveSportData()
        {
            var sportData = _dbContext.Sports.AsNoTracking()
                .Include(s => s.Events)
                .ThenInclude(e => e.Matches.Where(m => m.IsActive))
                .ThenInclude(m => m.Bets.Where(m => m.IsActive))
                .ThenInclude(b => b.Odds.Where(m => m.IsActive))
                .SingleOrDefault();

            return sportData;
        }

        public Sport GetSportData()
        {
            var sportData = _dbContext.Sports.AsNoTracking()
                .Include(s => s.Events)
                .ThenInclude(e => e.Matches)
                .ThenInclude(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .SingleOrDefault();

            return sportData;
        }

        // generic?
        public bool AddSportData(Sport sportData)
        {
            _dbContext.Sports.Add(sportData);
            return true;
        }

        public bool UpdateSportData(Sport sportData)
        {
            _dbContext.Update(sportData);
            return true;
        }

        public bool UpdateEventData(ICollection<Event> events)
        {
            _dbContext.Events.UpdateRange(events);
            return true;
        }

        public bool SaveChanges()
        {
            _dbContext.SaveChanges();
            return true;
        }
    }
}
