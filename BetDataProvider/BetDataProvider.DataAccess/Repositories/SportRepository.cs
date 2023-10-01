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

        public async Task<Sport> GetActiveSportDataAsync()
        {
            var sportData = await _dbContext.Sports
                .AsNoTracking()
                .Include(s => s.Events.Where(e => e.IsActive))
                .ThenInclude(e => e.Matches.Where(m => m.IsActive))
                .ThenInclude(m => m.Bets.Where(b => b.IsActive))
                .ThenInclude(b => b.Odds.Where(o => o.IsActive))
                .SingleOrDefaultAsync();

            return sportData;
        }

        public async Task<Sport> GetAllSportDataAsync()
        {
            var sportData = await _dbContext.Sports
                .AsNoTracking()
                .Include(s => s.Events)
                .ThenInclude(e => e.Matches)
                .ThenInclude(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .SingleOrDefaultAsync();

            return sportData;
        }

        public async Task<Event> GetEventByXmlIdAsync(int xmlId)
        {
            var @event = await _dbContext.Events
                .AsNoTracking()
                .Include(e => e.Matches)
                .ThenInclude(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .FirstOrDefaultAsync(e => e.XmlId == xmlId);

            return @event;
        }

        public async Task AddSportDataAsync(Sport sportData)
        {
            await _dbContext.Sports.AddAsync(sportData);
        }

        public void UpdateSportData(Sport sportData)
        {
            _dbContext.Update(sportData);
        }

        public async Task AddMatchChangeMessageAsync(int matchId, MessageType type)
        {
            var message = new MatchChangeMessage(matchId, type)
            {
                MatchId = matchId,
                MessageType = type
            };
            await _dbContext.MatchChangeMessages.AddAsync(message);
        }

        public async Task AddBetChangeMessageAsync(int betId, MessageType type)
        {
            var message = new BetChangeMessage(betId, type)
            {
                BetId = betId,
                MessageType = type
            };
            await _dbContext.BetChangeMessages.AddAsync(message);
        }

        public async Task AddOddChangeMessageAsync(int oddId, MessageType type)
        {
            var message = new OddChangeMessage(oddId, type)
            {
                OddId = oddId,
                MessageType = type
            };
            await _dbContext.OddChangeMessages.AddAsync(message);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
