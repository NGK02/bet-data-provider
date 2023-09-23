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
    public class SportRepository : ISportRepository
    {
        private readonly BetDataDbContext _dbContext;

        public SportRepository(BetDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool SaveSportData(Sport sportData)
        {
            if (GetSportData() is null)
            {
                _dbContext.Sports.Add(sportData);
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public Sport GetSportData()
        {
            var sportData = _dbContext.Sports.Include(s => s.Events)
                .ThenInclude(e => e.Matches)
                .ThenInclude(m => m.Bets)
                .ThenInclude(b => b.Odds)
                .SingleOrDefault();

            return sportData;
        }
    }
}
