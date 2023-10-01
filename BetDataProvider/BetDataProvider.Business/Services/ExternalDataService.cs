using Azure;
using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Models.Contracts;
using BetDataProvider.DataAccess.Models.Enums;
using BetDataProvider.DataAccess.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BetDataProvider.Business.Services
{
    public class ExternalDataService : IExternalDataService
    {
        private readonly ISportRepository _sportRepository;
        private readonly IMatchRepository _matchRepository;

        public ExternalDataService(ISportRepository sportRepository, IMatchRepository matchRepository)
        {
            this._sportRepository = sportRepository;
            this._matchRepository = matchRepository;
        }

        public async Task SaveSportDataAsync(Sport newSportData)
        {
            var oldSportData = await _sportRepository.GetActiveSportDataAsync();

            if (newSportData is not null)
            {
                if (oldSportData is null)
                {
                    await _sportRepository.AddSportDataAsync(newSportData);
                }
                else
                {
                    await UpdateSportData(oldSportData, newSportData);
                }
                await _sportRepository.SaveChangesAsync();
            }
        }

        private async Task UpdateSportData(Sport oldSportData, Sport newSportData)
        {
            var oldEvents = oldSportData.Events;
            var newEvents = newSportData.Events;

            var updatedEvents = await MergeAndUpdateEvents(oldEvents, newEvents);

            oldSportData.Events = updatedEvents;

            _sportRepository.UpdateSportData(oldSportData);
        }

        private async Task<HashSet<Event>> MergeAndUpdateEvents(HashSet<Event> oldEvents, HashSet<Event> newEvents)
        {
            HashSet<Event> updatedEvents = new HashSet<Event>();

            foreach (var newEvent in newEvents)
            {
                var updatedEvent = oldEvents.FirstOrDefault(e => e.XmlId == newEvent.XmlId) ?? await _sportRepository.GetEventByXmlIdAsync(newEvent.XmlId);

                if (updatedEvent is not null)
                {
                    updatedEvent.IsActive = newEvent.IsActive;
                    updatedEvent.Matches = await MergeAndUpdateMatchesAsync(updatedEvent.Matches, newEvent.Matches);

                    oldEvents.Remove(updatedEvent);
                }
                else
                {
                    updatedEvent = newEvent;
                }

                updatedEvents.Add(updatedEvent);
            }

            updatedEvents.UnionWith(await SetInactiveEvents(oldEvents));

            return updatedEvents;
        }

        private async Task<HashSet<Match>> MergeAndUpdateMatchesAsync(HashSet<Match> oldMatches, HashSet<Match> newMatches)
        {
            HashSet<Match> updatedMatches = new HashSet<Match>();

            foreach (var newMatch in newMatches)
            {
                var updatedMatch = oldMatches.FirstOrDefault(e => e.XmlId == newMatch.XmlId) ?? await _matchRepository.GetMatchByXmlIdAsync(newMatch.XmlId);

                if (updatedMatch is not null)
                {
                    await UpdateMatchPropertiesAsync(updatedMatch, newMatch);

                    oldMatches.Remove(updatedMatch);
                }
                else
                {
                    updatedMatch = newMatch;
                }

                updatedMatches.Add(updatedMatch);
            }

            updatedMatches.UnionWith(await SetInactiveMatchesAsync(oldMatches));

            return updatedMatches;
        }

        private async Task UpdateMatchPropertiesAsync(Match updatedMatch, Match newMatch)
        {
            bool hasChanged = false;

            if (updatedMatch.MatchType != newMatch.MatchType)
            {
                hasChanged = true;
                updatedMatch.MatchType = newMatch.MatchType;
            }
            if (updatedMatch.StartDate != newMatch.StartDate)
            {
                hasChanged = true;
                updatedMatch.StartDate = newMatch.StartDate;
            }
            if (updatedMatch.IsActive != newMatch.IsActive)
            {
                hasChanged = true;
                updatedMatch.IsActive = newMatch.IsActive;
            }
            updatedMatch.Bets = await MergeAndUpdateBetsAsync(updatedMatch.Bets, newMatch.Bets);

            if (hasChanged)
            {
                await _sportRepository.AddMatchChangeMessageAsync(updatedMatch.Id, MessageType.Update);
            }
        }

        private async Task<HashSet<Bet>> MergeAndUpdateBetsAsync(HashSet<Bet> oldBets, HashSet<Bet> newBets)
        {
            HashSet<Bet> updatedBets = new HashSet<Bet>();

            foreach (var newBet in newBets)
            {
                var updatedBet = oldBets.FirstOrDefault(e => e.XmlId == newBet.XmlId) ?? await _matchRepository.GetBetByXmlIdAsync(newBet.XmlId);

                if (updatedBet is not null)
                {
                    await UpdateBetPropertiesAsync(updatedBet, newBet);

                    oldBets.Remove(updatedBet);
                }
                else
                {
                    updatedBet = newBet;
                }

                updatedBets.Add(updatedBet);
            }

            updatedBets.UnionWith(await SetInactiveBetsAsync(oldBets));

            return updatedBets;
        }

        private async Task UpdateBetPropertiesAsync(Bet updatedBet, Bet newBet)
        {
            bool hasChanged = false;

            if (updatedBet.IsActive != newBet.IsActive)
            {
                hasChanged = true;
                updatedBet.IsActive = newBet.IsActive;
            }
            updatedBet.Odds = await MergeAndUpdateOddsAsync(updatedBet.Odds, newBet.Odds);

            if (hasChanged)
            {
                await _sportRepository.AddBetChangeMessageAsync(updatedBet.Id, MessageType.Update);
            }
        }

        private async Task<HashSet<Odd>> MergeAndUpdateOddsAsync(HashSet<Odd> oldOdds, HashSet<Odd> newOdds)
        {
            HashSet<Odd> updatedOdds = new HashSet<Odd>();

            foreach (var newOdd in newOdds)
            {
                var updatedOdd = oldOdds.FirstOrDefault(e => e.XmlId == newOdd.XmlId) ?? await _matchRepository.GetOddByXmlIdAsync(newOdd.XmlId);

                if (updatedOdd is not null)
                {
                    await UpdateOddPropertiesAsync(updatedOdd, newOdd);

                    oldOdds.Remove(updatedOdd);
                }
                else
                {
                    updatedOdd = newOdd;
                }

                updatedOdds.Add(updatedOdd);
            }

            updatedOdds.UnionWith(await SetInactiveOddsAsync(oldOdds));

            return updatedOdds;
        }

        private async Task UpdateOddPropertiesAsync(Odd updatedOdd, Odd newOdd)
        {
            bool hasChanged = false;

            if (updatedOdd.IsActive != newOdd.IsActive)
            {
                hasChanged = true;
                updatedOdd.IsActive = newOdd.IsActive;
            }
            if (updatedOdd.Value != newOdd.Value)
            {
                hasChanged = true;
                updatedOdd.Value = newOdd.Value;
            }
            if (updatedOdd.SpecialBetValue != newOdd.SpecialBetValue) 
            {
                hasChanged = true;
                updatedOdd.SpecialBetValue = newOdd.SpecialBetValue;
            }

            if (hasChanged)
            {
                await _sportRepository.AddOddChangeMessageAsync(updatedOdd.Id, MessageType.Update);
            }
        }

        private async Task<HashSet<Event>> SetInactiveEvents(HashSet<Event> inactiveEvents)
        {
            foreach (var inactiveEvent in inactiveEvents)
            {
                inactiveEvent.IsActive = false;
                inactiveEvent.Matches = await SetInactiveMatchesAsync(inactiveEvent.Matches);
            }
            return inactiveEvents;
        }

        private async Task<HashSet<Match>> SetInactiveMatchesAsync(HashSet<Match> inactiveMatches)
        {
            foreach (var inactiveMatch in inactiveMatches)
            {
                inactiveMatch.IsActive = false;
                inactiveMatch.Bets = await SetInactiveBetsAsync(inactiveMatch.Bets);

                await _sportRepository.AddMatchChangeMessageAsync(inactiveMatch.Id, MessageType.Delete);
            }
            return inactiveMatches;
        }

        private async Task<HashSet<Bet>> SetInactiveBetsAsync(HashSet<Bet> inactiveBets)
        {
            foreach (var inactiveBet in inactiveBets)
            {
                inactiveBet.IsActive = false;
                inactiveBet.Odds = await SetInactiveOddsAsync(inactiveBet.Odds);

                await _sportRepository.AddBetChangeMessageAsync(inactiveBet.Id, MessageType.Delete);
            }
            return inactiveBets;
        }

        private async Task<HashSet<Odd>> SetInactiveOddsAsync(HashSet<Odd> inactiveOdds)
        {
            foreach (var inactiveOdd in inactiveOdds)
            {
                inactiveOdd.IsActive = false;

                await _sportRepository.AddOddChangeMessageAsync(inactiveOdd.Id, MessageType.Delete);
            }
            return inactiveOdds;
        }
    }
}
