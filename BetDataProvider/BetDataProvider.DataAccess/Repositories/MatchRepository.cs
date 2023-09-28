﻿using BetDataProvider.DataAccess.Models;
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
            var matchingMatch = _dbContext.Matches.AsNoTracking()
                .Include(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .FirstOrDefault(m => m.XmlId == xmlId);

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

        public List<Match> GetUpcomingMatchesWithPreviewBets(double? hoursAhead/*, bool? isActiveData*/)
        {
            var matches = GetAllMatchData().Where(m => m.IsActive);

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

            matches = FilterBy(hoursAhead/*, isActiveData*/, matches);

            return matches.ToList();
        }

        private IQueryable<Match> FilterBy(double? hoursAhead, /*bool? isActiveData,*/ IQueryable<Match> matches)
        {
            if (hoursAhead is not null)
            {
                DateTime futureDate = DateTime.Now.AddDays(hoursAhead.Value);
                matches = matches.Where(m => m.StartDate.Date >= DateTime.Now && m.StartDate.Date <= futureDate);
            }
            //if (isActiveData ?? true)
            //{
            //    matches = matches.Where(m => m.IsActive && m.Bets
            //                       .All(b => b.IsActive && b.Odds
            //                       .All(o => o.IsActive)));
            //}

            return matches;
        }
    }
}
