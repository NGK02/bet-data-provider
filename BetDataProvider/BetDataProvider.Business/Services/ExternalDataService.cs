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

        public bool SaveSportData(Sport newSportData)
        {
            var oldSportData = _sportRepository.GetActiveSportData();

            if (newSportData is not null)
            {
                if (oldSportData is null)
                {
                    _sportRepository.AddSportData(newSportData);
                }
                else
                {
                    UpdateSportData(oldSportData, newSportData);
                }
                _sportRepository.SaveChanges();
            }

            return true;
        }

        private bool UpdateSportData(Sport oldSportData, Sport newSportData)
        {
            var oldEvents = oldSportData.Events;
            var newEvents = newSportData.Events;

            var updatedEvents = MergeAndUpdateEvents(oldEvents, newEvents);

            oldSportData.Events = updatedEvents;

            _sportRepository.UpdateSportData(oldSportData);

            return true;
        }

        private HashSet<Event> MergeAndUpdateEvents(HashSet<Event> oldEvents, HashSet<Event> newEvents)
        {
            HashSet<Event> updatedEvents = new HashSet<Event>();

            foreach (var newEvent in newEvents)
            {
                var updatedEvent = oldEvents.FirstOrDefault(e => e.XmlId == newEvent.XmlId) ?? _sportRepository.GetEventByXmlId(newEvent.XmlId);

                if (updatedEvent is not null)
                {
                    updatedEvent.IsActive = newEvent.IsActive;
                    updatedEvent.Matches = MergeAndUpdateMatches(updatedEvent.Matches, newEvent.Matches);

                    oldEvents.Remove(updatedEvent);
                }
                else
                {
                    updatedEvent = newEvent;
                }

                updatedEvents.Add(updatedEvent);
            }

            updatedEvents.UnionWith(SetInactiveEvents(oldEvents));

            return updatedEvents;
        }

        private HashSet<Match> MergeAndUpdateMatches(HashSet<Match> oldMatches, HashSet<Match> newMatches)
        {
            HashSet<Match> updatedMatches = new HashSet<Match>();

            foreach (var newMatch in newMatches)
            {
                var updatedMatch = oldMatches.FirstOrDefault(e => e.XmlId == newMatch.XmlId) ?? _matchRepository.GetMatchByXmlId(newMatch.XmlId);

                if (updatedMatch is not null)
                {
                    UpdateMatchProperties(updatedMatch, newMatch);

                    oldMatches.Remove(updatedMatch);
                }
                else
                {
                    updatedMatch = newMatch;
                }

                updatedMatches.Add(updatedMatch);
            }

            updatedMatches.UnionWith(SetInactiveMatches(oldMatches));

            return updatedMatches;
        }

        private void UpdateMatchProperties(Match updatedMatch, Match newMatch)
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
            updatedMatch.Bets = MergeAndUpdateBets(updatedMatch.Bets, newMatch.Bets);

            if (hasChanged)
            {
                _sportRepository.AddMatchChangeMessage(updatedMatch.Id, MessageType.Update);
            }
        }

        private HashSet<Bet> MergeAndUpdateBets(HashSet<Bet> oldBets, HashSet<Bet> newBets)
        {
            HashSet<Bet> updatedBets = new HashSet<Bet>();

            foreach (var newBet in newBets)
            {
                var updatedBet = oldBets.FirstOrDefault(e => e.XmlId == newBet.XmlId) ?? _matchRepository.GetBetByXmlId(newBet.XmlId);

                if (updatedBet is not null)
                {
                    UpdateBetProperties(updatedBet, newBet);

                    oldBets.Remove(updatedBet);
                }
                else
                {
                    updatedBet = newBet;
                }

                updatedBets.Add(updatedBet);
            }

            updatedBets.UnionWith(SetInactiveBets(oldBets));

            return updatedBets;
        }

        private void UpdateBetProperties(Bet updatedBet, Bet newBet)
        {
            bool hasChanged = false;

            if (updatedBet.IsActive != newBet.IsActive)
            {
                hasChanged = true;
                updatedBet.IsActive = newBet.IsActive;
            }
            updatedBet.Odds = MergeAndUpdateOdds(updatedBet.Odds, newBet.Odds);

            if (hasChanged)
            {
                _sportRepository.AddBetChangeMessage(updatedBet.Id, MessageType.Update);
            }
        }

        private HashSet<Odd> MergeAndUpdateOdds(HashSet<Odd> oldOdds, HashSet<Odd> newOdds)
        {
            HashSet<Odd> updatedOdds = new HashSet<Odd>();

            foreach (var newOdd in newOdds)
            {
                var updatedOdd = oldOdds.FirstOrDefault(e => e.XmlId == newOdd.XmlId) ?? _matchRepository.GetOddByXmlId(newOdd.XmlId);

                if (updatedOdd is not null)
                {
                    UpdateOddProperties(updatedOdd, newOdd);

                    oldOdds.Remove(updatedOdd);
                }
                else
                {
                    updatedOdd = newOdd;
                }

                updatedOdds.Add(updatedOdd);
            }

            updatedOdds.UnionWith(SetInactiveOdds(oldOdds));

            return updatedOdds;
        }

        private void UpdateOddProperties(Odd updatedOdd, Odd newOdd)
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
                _sportRepository.AddOddChangeMessage(updatedOdd.Id, MessageType.Update);
            }
        }

        private HashSet<Event> SetInactiveEvents(HashSet<Event> inactiveEvents)
        {
            foreach (var inactiveEvent in inactiveEvents)
            {
                inactiveEvent.IsActive = false;
                inactiveEvent.Matches = SetInactiveMatches(inactiveEvent.Matches);
            }
            return inactiveEvents;
        }

        private HashSet<Match> SetInactiveMatches(HashSet<Match> inactiveMatches)
        {
            foreach (var inactiveMatch in inactiveMatches)
            {
                inactiveMatch.IsActive = false;
                inactiveMatch.Bets = SetInactiveBets(inactiveMatch.Bets);

                _sportRepository.AddMatchChangeMessage(inactiveMatch.Id, MessageType.Delete);
            }
            return inactiveMatches;
        }

        private HashSet<Bet> SetInactiveBets(HashSet<Bet> inactiveBets)
        {
            foreach (var inactiveBet in inactiveBets)
            {
                inactiveBet.IsActive = false;
                inactiveBet.Odds = SetInactiveOdds(inactiveBet.Odds);

                _sportRepository.AddBetChangeMessage(inactiveBet.Id, MessageType.Delete);
            }
            return inactiveBets;
        }

        private HashSet<Odd> SetInactiveOdds(HashSet<Odd> inactiveOdds)
        {
            foreach (var inactiveOdd in inactiveOdds)
            {
                inactiveOdd.IsActive = false;

                _sportRepository.AddOddChangeMessage(inactiveOdd.Id, MessageType.Delete);
            }
            return inactiveOdds;
        }
    }
}
