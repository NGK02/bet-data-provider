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

        public async Task SaveSportDataAsync(Sport? newSportData)
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
                    await UpdateSportDataAsync(oldSportData, newSportData);
                }
                await _sportRepository.SaveChangesAsync();
            }
        }

        private async Task UpdateSportDataAsync(Sport oldActiveSportData, Sport newSportData)
        {
            var oldActiveEvents = oldActiveSportData.Events;
            var newEvents = newSportData.Events;

            var updatedEvents = await MergeAndUpdateEventsAsync(oldActiveEvents, newEvents);

            oldActiveSportData.Events = updatedEvents;

            _sportRepository.UpdateSportData(oldActiveSportData);
        }

        private async Task<HashSet<Event>> MergeAndUpdateEventsAsync(HashSet<Event> oldActiveEvents, HashSet<Event> newEvents)
        {
            HashSet<Event> updatedEvents = new HashSet<Event>();

            foreach (var newEvent in newEvents)
            {
                //var updatedEvent = oldActiveEvents.FirstOrDefault(e => e.XmlId == newEvent.XmlId) ?? await _sportRepository.GetEventByXmlIdAsync(newEvent.XmlId);

                oldActiveEvents.TryGetValue(newEvent, out var updatedEvent);
                updatedEvent ??= await _sportRepository.GetEventByXmlIdAsync(newEvent.XmlId);

                if (updatedEvent is not null)
                {
                    updatedEvent.IsActive = newEvent.IsActive;
                    updatedEvent.Matches = await MergeAndUpdateMatchesAsync(updatedEvent.Matches, newEvent.Matches);

                    oldActiveEvents.Remove(updatedEvent);
                }
                else
                {
                    updatedEvent = newEvent;
                }

                updatedEvents.Add(updatedEvent);
            }

            updatedEvents.UnionWith(await SetInactiveEvents(oldActiveEvents));

            return updatedEvents;
        }

        private async Task<HashSet<Match>> MergeAndUpdateMatchesAsync(HashSet<Match> oldActiveMatches, HashSet<Match> newMatches)
        {
            HashSet<Match> updatedMatches = new HashSet<Match>();

            foreach (var newMatch in newMatches)
            {
                //var updatedMatch = oldActiveMatches.FirstOrDefault(e => e.XmlId == newMatch.XmlId) ?? await _matchRepository.GetMatchByXmlIdAsync(newMatch.XmlId);

                oldActiveMatches.TryGetValue(newMatch, out var updatedMatch);
                updatedMatch ??= await _matchRepository.GetMatchByXmlIdAsync(newMatch.XmlId);

                if (updatedMatch is not null)
                {
                    await UpdateMatchPropertiesAsync(updatedMatch, newMatch);

                    oldActiveMatches.Remove(updatedMatch);
                }
                else
                {
                    updatedMatch = newMatch;
                }

                updatedMatches.Add(updatedMatch);
            }

            updatedMatches.UnionWith(await SetInactiveMatchesAsync(oldActiveMatches));

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

        private async Task<HashSet<Bet>> MergeAndUpdateBetsAsync(HashSet<Bet> oldActiveBets, HashSet<Bet> newBets)
        {
            HashSet<Bet> updatedBets = new HashSet<Bet>();

            foreach (var newBet in newBets)
            {
                //var updatedBet = oldActiveBets.FirstOrDefault(e => e.XmlId == newBet.XmlId) ?? await _matchRepository.GetBetByXmlIdAsync(newBet.XmlId);

                oldActiveBets.TryGetValue(newBet, out var updatedBet);
                updatedBet ??= await _matchRepository.GetBetByXmlIdAsync(newBet.XmlId);

                if (updatedBet is not null)
                {
                    await UpdateBetPropertiesAsync(updatedBet, newBet);

                    oldActiveBets.Remove(updatedBet);
                }
                else
                {
                    updatedBet = newBet;
                }

                updatedBets.Add(updatedBet);
            }

            updatedBets.UnionWith(await SetInactiveBetsAsync(oldActiveBets));

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

        private async Task<HashSet<Odd>> MergeAndUpdateOddsAsync(HashSet<Odd> oldActiveOdds, HashSet<Odd> newOdds)
        {
            HashSet<Odd> updatedOdds = new HashSet<Odd>();

            foreach (var newOdd in newOdds)
            {
                //var updatedOdd = oldActiveOdds.FirstOrDefault(e => e.XmlId == newOdd.XmlId) ?? await _matchRepository.GetOddByXmlIdAsync(newOdd.XmlId);

                oldActiveOdds.TryGetValue(newOdd, out var updatedOdd);
                updatedOdd ??= await _matchRepository.GetOddByXmlIdAsync(newOdd.XmlId);

                if (updatedOdd is not null)
                {
                    await UpdateOddPropertiesAsync(updatedOdd, newOdd);

                    oldActiveOdds.Remove(updatedOdd);
                }
                else
                {
                    updatedOdd = newOdd;
                }

                updatedOdds.Add(updatedOdd);
            }

            updatedOdds.UnionWith(await SetInactiveOddsAsync(oldActiveOdds));

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
