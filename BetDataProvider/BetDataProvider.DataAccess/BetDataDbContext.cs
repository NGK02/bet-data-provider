using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

        public virtual DbSet<MatchChangeMessage> MatchChangeMessages { get; set; }

        public virtual DbSet<BetChangeMessage> BetChangeMessages { get; set; }

        public virtual DbSet<OddChangeMessage> OddChangeMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Match>()
                .Property(m => m.MatchType)
                .HasConversion<string>();

            builder
                .Entity<MatchChangeMessage>()
                .Property(m => m.MessageType)
                .HasConversion<string>();

            builder
                .Entity<BetChangeMessage>()
                .Property(m => m.MessageType)
                .HasConversion<string>();

            builder
                .Entity<OddChangeMessage>()
                .Property(m => m.MessageType)
                .HasConversion<string>();

            base.OnModelCreating(builder);
        }
    }
}
