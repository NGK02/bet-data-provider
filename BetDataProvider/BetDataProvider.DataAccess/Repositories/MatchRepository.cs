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

        public Match GetMatchByXmlId(int xmlId)
        {
            var matchingMatch = GetAllMatchData()
                .AsNoTracking()
                .Include(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .FirstOrDefault(m => m.XmlId == xmlId);

            return matchingMatch;
        }

        public Bet GetBetByXmlId(int xmlId)
        {
            var matchingBet = _dbContext.Bets
                .AsNoTracking()
                .Include(b => b.Odds)
                .FirstOrDefault(b => b.XmlId == xmlId);

            return matchingBet;
        }

        public Odd GetOddByXmlId(int xmlId)
        {
            var matchingOdd = _dbContext.Odds.AsNoTracking().FirstOrDefault(o => o.XmlId == xmlId);

            return matchingOdd;
        }

        public List<Match> GetUpcomingMatchesWithPreviewBets()
        {
            DateTime futureDate = DateTime.Now.AddHours(24);
            var matches = GetAllMatchData().Where(m => m.IsActive && m.StartDate.Date >= DateTime.Now && m.StartDate.Date <= futureDate);

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

            return matches.ToList();
        }
    }
}
