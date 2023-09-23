using BetDataProvider.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess
{
    public class BetDataDbContext : DbContext
    {
        public BetDataDbContext(DbContextOptions<BetDataDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Sport> Sports { get; set; }

        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<Match> Matches { get; set; }

        public virtual DbSet<Bet> Bets { get; set; }

        public virtual DbSet<Odd> Odds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // configure enum to be written as string into db
        }
    }
}
