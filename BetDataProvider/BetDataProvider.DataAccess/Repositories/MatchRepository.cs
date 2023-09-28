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
        private readonly BetDataDbContext _dbContext;

        public MatchRepository(BetDataDbContext dbContext) 
        {
                this._dbContext = dbContext;
        }

        public IQueryable<Match> GetAllMatches() 
        {
            return _dbContext.Matches.Include(m => m.Bets).ThenInclude(m => m.Odds);
        }

        public Match GetMatchByXmlId(int xmlId)
        {
            // throw ex when null?
            return GetAllMatches().FirstOrDefault(m => m.XmlId == xmlId);
        }

        public List<Match> GetMatches(int? hoursAhead, bool? isActiveData)
        {
            //var allMatches = GetAllMatches();
            //allMatches = FilterBy();
            throw new NotImplementedException();
        }

        private IQueryable<Match> FilterBy(int? hoursAhead, bool? isActiveData, IQueryable<Match> matches)
        {
            //if (hoursAhead is not null)
            //{
            //    matches = matches.Where();
            //}

            //return walletTransactions;
            throw new NotImplementedException();
        }
    }
}
