using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Models.Enums;
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

        public Sport GetActiveSportData()
        {
            var sportData = _dbContext.Sports
                .AsNoTracking()
                .Include(s => s.Events.Where(e => e.IsActive))
                .ThenInclude(e => e.Matches.Where(m => m.IsActive))
                .ThenInclude(m => m.Bets.Where(b => b.IsActive))
                .ThenInclude(b => b.Odds.Where(o => o.IsActive))
                .SingleOrDefault();

            return sportData;
        }

        public Sport GetAllSportData()
        {
            var sportData = _dbContext.Sports
                .AsNoTracking()
                .Include(s => s.Events)
                .ThenInclude(e => e.Matches)
                .ThenInclude(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .SingleOrDefault();

            return sportData;
        }

        public Event GetEventByXmlId(int xmlId)
        {
            var @event = _dbContext.Events
                .AsNoTracking()
                .Include(e => e.Matches)
                .ThenInclude(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .FirstOrDefault(e => e.XmlId == xmlId);

            return @event;
        }

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

        public void AddMatchChangeMessage(int matchId, MessageType type)
        {
            var message = new MatchChangeMessage(matchId, type)
            {
                MatchId = matchId,
                MessageType = type
            };
            _dbContext.MatchChangeMessages.Add(message);
        }

        public void AddBetChangeMessage(int betId, MessageType type)
        {
            var message = new BetChangeMessage(betId, type)
            {
                BetId = betId,
                MessageType = type
            };
            _dbContext.BetChangeMessages.Add(message);
        }

        public void AddOddChangeMessage(int oddId, MessageType type)
        {
            var message = new OddChangeMessage(oddId, type)
            {
                OddId = oddId,
                MessageType = type
            };
            _dbContext.OddChangeMessages.Add(message);
        }

        public bool SaveChanges()
        {
            _dbContext.SaveChanges();
            return true;
        }
    }
}
