using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly string[] previewBetNames = { "Match Winner", "Map Advantage", "Total Maps Played" };

        private readonly BetDataDbContext _dbContext;

        public MatchRepository(BetDataDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IQueryable<Match> GetAllMatchData()
        {
            return _dbContext.Matches.Include(m => m.Bets).ThenInclude(m => m.Odds);
        }

        public async Task<Match> GetMatchByXmlIdAsync(int xmlId)
        {
            var matchingMatch = await GetAllMatchData()
                .AsNoTracking()
                .Include(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .FirstOrDefaultAsync(m => m.XmlId == xmlId);

            return matchingMatch;
        }

        public async Task<Bet> GetBetByXmlIdAsync(int xmlId)
        {
            var matchingBet = await _dbContext.Bets
                .AsNoTracking()
                .Include(b => b.Odds)
                .FirstOrDefaultAsync(b => b.XmlId == xmlId);

            return matchingBet;
        }

        public async Task<Odd> GetOddByXmlIdAsync(int xmlId)
        {
            var matchingOdd = await _dbContext.Odds.AsNoTracking().FirstOrDefaultAsync(o => o.XmlId == xmlId);

            return matchingOdd;
        }

        public async Task<List<Match>> GetUpcomingMatchesWithPreviewBetsAsync()
        {
            DateTime futureDate = DateTime.Now.AddHours(24);
            var matches = await GetAllMatchData().Where(m => m.IsActive && m.StartDate.Date >= DateTime.Now && m.StartDate.Date <= futureDate).ToListAsync();

            foreach (var match in matches)
            {
                match.Bets = match.Bets.Where(b => previewBetNames.Contains(b.Name) && b.IsActive).ToHashSet();

                foreach (var bet in match.Bets)
                {
                    if (bet.Odds.All(o => o.SpecialBetValue != null))
                    {
                        bet.Odds = bet.Odds.GroupBy(o => o.SpecialBetValue).FirstOrDefault().ToHashSet();
                    }
                }
            }

            return matches;
        }
    }
}
