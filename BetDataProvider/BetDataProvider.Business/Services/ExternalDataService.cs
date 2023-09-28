using Azure;
using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Models.Contracts;
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
        private const string _xmlFeedUrl = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";

        private readonly ISportRepository _sportRepository;
        private readonly HttpClient _httpClient;

        public ExternalDataService(ISportRepository sportRepository, HttpClient httpClient)
        {
            this._sportRepository = sportRepository;
            this._httpClient = httpClient;
        }

        // exceptions?
        public async Task<Sport> GetAndParseXmlDataAsync()
        {
            var uri = new Uri(_xmlFeedUrl);

            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var resultStream = await response.Content.ReadAsByteArrayAsync();

                var serializer = new XmlSerializer(typeof(XmlSports));
                using (TextReader reader = new StreamReader(new MemoryStream(resultStream), Encoding.UTF8))
                {
                    var parsedXml = (XmlSports)serializer.Deserialize(reader);
                    return parsedXml.Sport;
                }
            }

            return null;
        }

        public bool SaveSportData(Sport newSportData)
        {
            // maybe first check if ANY sport data instead of active, not sure if needed
            var oldSportData = _sportRepository.GetActiveSportData();

            if (newSportData is not null)
            {
                // not sure what to do if Sport exists but no Events
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

        public bool UpdateSportData(Sport oldSportData, Sport newSportData)
        {
            var oldEvents = oldSportData.Events;
            var newEvents = newSportData.Events;

            var updatedEvents = MergeAndUpdateEvents(oldEvents, newEvents);
            //_sportRepository.GetMatchingEntities(_matches, _bets, _odds);

            oldSportData.Events = updatedEvents;

            _sportRepository.UpdateSportData(oldSportData);

            return true;
        }

        private HashSet<Event> MergeAndUpdateEvents(HashSet<Event> oldEvents, HashSet<Event> newEvents)
        {
            HashSet<Event> updatedEvents = new HashSet<Event>();

            foreach (Event newEvent in newEvents)
            {
                var updatedEvent = oldEvents.FirstOrDefault(e => e.XmlId == newEvent.XmlId);

                // add new event
                if (updatedEvent is null)
                {
                    updatedEvents.Add(newEvent);
                }
                else
                {
                    // update old active event
                    updatedEvent.Matches = MergeAndUpdateMatches(updatedEvent.Matches, newEvent.Matches);

                    oldEvents.Remove(updatedEvent);

                    updatedEvents.Add(updatedEvent);
                }
            }

            // set old active event to inactive
            //updatedEvents.UnionWith(UpdateInactiveEvents(oldEvents));

            return updatedEvents;
        }

        private HashSet<Match> MergeAndUpdateMatches(HashSet<Match> oldMatches, HashSet<Match> newMatches)
        {
            HashSet<Match> updatedMatches = new HashSet<Match>();

            foreach (var newMatch in newMatches)
            {
                var updatedMatch = oldMatches.FirstOrDefault(e => e.XmlId == newMatch.XmlId) ?? _sportRepository.GetMatchByXmlId(newMatch.XmlId);

                if (updatedMatch is not null)
                {
                    UpdateMatchProperties(updatedMatch, newMatch);
                    // maybe a check first to see if updated match was assigned from oldMatches for safety?
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
            updatedMatch.MatchType = newMatch.MatchType;
            updatedMatch.StartDate = newMatch.StartDate;
            updatedMatch.IsActive = newMatch.IsActive;
            updatedMatch.Bets = MergeAndUpdateBets(updatedMatch.Bets, newMatch.Bets);
        }

        //private HashSet<Match> MergeAndUpdateMatches(HashSet<Match> oldMatches, HashSet<Match> newMatches)
        //{
        //    HashSet<Match> updatedMatches = new HashSet<Match>();

        //    foreach (var newMatch in newMatches)
        //    {
        //        var updatedMatch = oldMatches.FirstOrDefault(e => e.XmlId == newMatch.XmlId);

        //        if (updatedMatch is null)
        //        {
        //            var reactivatedMatch = _sportRepository.GetMatchByXmlId(newMatch.XmlId);

        //            if (reactivatedMatch is not null)
        //            {
        //                reactivatedMatch.MatchType = newMatch.MatchType;
        //                reactivatedMatch.StartDate = newMatch.StartDate;
        //                reactivatedMatch.IsActive = newMatch.IsActive;
        //                reactivatedMatch.Bets = MergeAndUpdateBets(reactivatedMatch.Bets, newMatch.Bets);

        //                updatedMatches.Add(reactivatedMatch);

        //                continue;
        //            }
        //            else
        //            {
        //                updatedMatches.Add(newMatch);
        //            }
        //        }
        //        else
        //        {
        //            updatedMatch.IsActive = newMatch.IsActive;
        //            updatedMatch.MatchType = newMatch.MatchType;
        //            updatedMatch.StartDate = newMatch.StartDate;
        //            updatedMatch.Bets = MergeAndUpdateBets(updatedMatch.Bets, newMatch.Bets);

        //            oldMatches.Remove(updatedMatch);

        //            updatedMatches.Add(updatedMatch);
        //        }
        //    }

        //    updatedMatches.UnionWith(UpdateInactiveMatches(oldMatches));

        //    return updatedMatches;
        //}

        //private HashSet<Bet> MergeAndUpdateBets(HashSet<Bet> oldBets, HashSet<Bet> newBets)
        //{
        //    HashSet<Bet> updatedBets = new HashSet<Bet>();

        //    foreach (var newBet in newBets)
        //    {
        //        var updatedBet = oldBets.FirstOrDefault(e => e.XmlId == newBet.XmlId);

        //        if (updatedBet is null)
        //        {
        //            var reactivatedBet = _sportRepository.GetBetByXmlId(newBet.XmlId);

        //            if (reactivatedBet is not null)
        //            {
        //                reactivatedBet.IsActive = newBet.IsActive;
        //                reactivatedBet.Odds = MergeAndUpdateOdds(reactivatedBet.Odds, newBet.Odds);

        //                updatedBets.Add(reactivatedBet);
        //            }
        //            else
        //            {
        //                updatedBets.Add(newBet);
        //            }
        //        }
        //        else
        //        {
        //            updatedBet.IsActive = newBet.IsActive;
        //            updatedBet.Odds = MergeAndUpdateOdds(updatedBet.Odds, newBet.Odds);

        //            oldBets.Remove(updatedBet);

        //            updatedBets.Add(updatedBet);
        //        }
        //    }

        //    updatedBets.UnionWith(UpdateInactiveBets(oldBets));

        //    return updatedBets;
        //}

        private HashSet<Bet> MergeAndUpdateBets(HashSet<Bet> oldBets, HashSet<Bet> newBets)
        {
            HashSet<Bet> updatedBets = new HashSet<Bet>();

            foreach (var newBet in newBets)
            {
                var updatedBet = oldBets.FirstOrDefault(e => e.XmlId == newBet.XmlId) ?? _sportRepository.GetBetByXmlId(newBet.XmlId);

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
            updatedBet.IsActive = newBet.IsActive;
            updatedBet.Odds = MergeAndUpdateOdds(updatedBet.Odds, newBet.Odds);
        }

        //private HashSet<Odd> MergeAndUpdateOdds(HashSet<Odd> oldOdds, HashSet<Odd> newOdds)
        //{
        //    HashSet<Odd> updatedOdds = new HashSet<Odd>();

        //    foreach (var newOdd in newOdds)
        //    {
        //        var updatedOdd = oldOdds.FirstOrDefault(e => e.XmlId == newOdd.XmlId);

        //        if (updatedOdd is null)
        //        {
        //            var reactivatedOdd = _sportRepository.GetOddByXmlId(newOdd.XmlId);

        //            if (reactivatedOdd is not null)
        //            {
        //                reactivatedOdd.Value = newOdd.Value;
        //                reactivatedOdd.SpecialBetValue = newOdd.SpecialBetValue;
        //                reactivatedOdd.IsActive = newOdd.IsActive;

        //                updatedOdds.Add(reactivatedOdd);
        //            }
        //            else
        //            {
        //                updatedOdds.Add(newOdd);
        //            }
        //        }
        //        else
        //        {
        //            updatedOdd.IsActive = newOdd.IsActive;
        //            updatedOdd.Value = newOdd.Value;
        //            updatedOdd.SpecialBetValue = newOdd.SpecialBetValue;

        //            oldOdds.Remove(updatedOdd);

        //            updatedOdds.Add(updatedOdd);
        //        }
        //    }

        //    updatedOdds.UnionWith(SetInactiveOdds(oldOdds));

        //    return updatedOdds;
        //}

        private HashSet<Odd> MergeAndUpdateOdds(HashSet<Odd> oldOdds, HashSet<Odd> newOdds)
        {
            HashSet<Odd> updatedOdds = new HashSet<Odd>();

            foreach (var newOdd in newOdds)
            {
                var updatedOdd = oldOdds.FirstOrDefault(e => e.XmlId == newOdd.XmlId) ?? _sportRepository.GetOddByXmlId(newOdd.XmlId);

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
            updatedOdd.IsActive = newOdd.IsActive;
            updatedOdd.Value = newOdd.Value;
            updatedOdd.SpecialBetValue = newOdd.SpecialBetValue;
        }

        private HashSet<Match> SetInactiveMatches(HashSet<Match> inactiveMatches)
        {
            foreach (var inactiveMatch in inactiveMatches)
            {
                inactiveMatch.IsActive = false;
                inactiveMatch.Bets = SetInactiveBets(inactiveMatch.Bets);
            }
            return inactiveMatches;
        }

        private HashSet<Bet> SetInactiveBets(HashSet<Bet> inactiveBets)
        {
            foreach (var inactiveBet in inactiveBets)
            {
                inactiveBet.IsActive = false;
                inactiveBet.Odds = SetInactiveOdds(inactiveBet.Odds);
            }
            return inactiveBets;
        }

        private HashSet<Odd> SetInactiveOdds(HashSet<Odd> inactiveOdds)
        {
            foreach (var inactiveOdd in inactiveOdds)
            {
                inactiveOdd.IsActive = false;
            }
            return inactiveOdds;
        }

        // test method with seeded data for testing edge cases

        //public Sport GetAndParseXmlDataAsync()
        //{
        //    string[] strings = new string[4] { "<Sport Name=\"eSports\" ID=\"2357\"><Event Name=\"NBA2K, NBA Battle\" ID=\"83063\" IsLive=\"false\" CategoryID=\"9357\"><Match Name=\"L.A. Lakers (dema21) - Boston Celtics (yaro)\" ID=\"3118821\" StartDate=\"2023-09-25T07:20:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850075\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210324\" Value=\"2.29\"/><Odd Name=\"2\" ID=\"336210323\" Value=\"1.57\"/></Bet><Bet Name=\"Spread\" ID=\"48850074\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210864\" Value=\"1.83\" SpecialBetValue=\"2.5\"/><Odd Name=\"2\" ID=\"336210863\" Value=\"1.90\" SpecialBetValue=\"2.5\"/><Odd Name=\"1\" ID=\"336210606\" Value=\"2.02\" SpecialBetValue=\"-1.5\"/><Odd Name=\"2\" ID=\"336210605\" Value=\"1.73\" SpecialBetValue=\"-1.5\"/></Bet><Bet Name=\"Total\" ID=\"48850076\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210792\" Value=\"2.02\" SpecialBetValue=\"128.5\"/><Odd Name=\"Under\" ID=\"336210793\" Value=\"1.73\" SpecialBetValue=\"128.5\"/><Odd Name=\"Over\" ID=\"336210810\" Value=\"1.93\" SpecialBetValue=\"129.5\"/><Odd Name=\"Under\" ID=\"336210811\" Value=\"1.79\" SpecialBetValue=\"129.5\"/><Odd Name=\"Over\" ID=\"336210816\" Value=\"1.83\" SpecialBetValue=\"127.5\"/><Odd Name=\"Under\" ID=\"336210817\" Value=\"1.90\" SpecialBetValue=\"127.5\"/><Odd Name=\"Over\" ID=\"336210818\" Value=\"2.02\" SpecialBetValue=\"130.5\"/><Odd Name=\"Under\" ID=\"336210819\" Value=\"1.73\" SpecialBetValue=\"130.5\"/></Bet></Match><Match Name=\"Dallas Mavericks (Lalkoff) - Minnesota Timberwolves (resarke)\" ID=\"3118819\" StartDate=\"2023-09-25T07:23:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850094\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210425\" Value=\"1.90\"/><Odd Name=\"2\" ID=\"336210426\" Value=\"1.83\"/></Bet><Bet Name=\"Spread\" ID=\"48850095\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210853\" Value=\"1.86\" SpecialBetValue=\"2.5\"/><Odd Name=\"2\" ID=\"336210856\" Value=\"1.86\" SpecialBetValue=\"2.5\"/><Odd Name=\"1\" ID=\"336210795\" Value=\"1.79\" SpecialBetValue=\"1.5\"/><Odd Name=\"2\" ID=\"336210794\" Value=\"1.93\" SpecialBetValue=\"1.5\"/><Odd Name=\"1\" ID=\"336210423\" Value=\"2.06\" SpecialBetValue=\"-1.5\"/><Odd Name=\"2\" ID=\"336210424\" Value=\"1.70\" SpecialBetValue=\"-1.5\"/></Bet><Bet Name=\"Total\" ID=\"48850093\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210563\" Value=\"1.90\" SpecialBetValue=\"133.5\"/><Odd Name=\"Under\" ID=\"336210564\" Value=\"1.83\" SpecialBetValue=\"133.5\"/><Odd Name=\"Over\" ID=\"336210815\" Value=\"1.93\" SpecialBetValue=\"137.5\"/><Odd Name=\"Under\" ID=\"336210813\" Value=\"1.79\" SpecialBetValue=\"137.5\"/><Odd Name=\"Over\" ID=\"336210783\" Value=\"2.02\" SpecialBetValue=\"135.5\"/><Odd Name=\"Under\" ID=\"336210782\" Value=\"1.73\" SpecialBetValue=\"135.5\"/><Odd Name=\"Over\" ID=\"336210855\" Value=\"1.86\" SpecialBetValue=\"139.5\"/><Odd Name=\"Under\" ID=\"336210854\" Value=\"1.86\" SpecialBetValue=\"139.5\"/><Odd Name=\"Over\" ID=\"336210857\" Value=\"1.86\" SpecialBetValue=\"136.5\"/><Odd Name=\"Under\" ID=\"336210858\" Value=\"1.86\" SpecialBetValue=\"136.5\"/></Bet></Match></Event></Sport>",
        //        "<Sport Name=\"eSports\" ID=\"2357\"><Event Name=\"NBA2K, NBA Battle\" ID=\"83063\" IsLive=\"false\" CategoryID=\"9357\"><Match Name=\"L.A. Lakers (dema21) - Boston Celtics (yaro)\" ID=\"3118821\" StartDate=\"2023-09-25T07:20:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850075\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210324\" Value=\"1.52\"/><Odd Name=\"2\" ID=\"336210323\" Value=\"2.40\"/></Bet><Bet Name=\"Spread\" ID=\"48850074\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336211027\" Value=\"2.02\" SpecialBetValue=\"-5.5\"/><Odd Name=\"2\" ID=\"336211026\" Value=\"1.73\" SpecialBetValue=\"-5.5\"/><Odd Name=\"1\" ID=\"336210431\" Value=\"1.97\" SpecialBetValue=\"-2.5\"/><Odd Name=\"2\" ID=\"336210432\" Value=\"1.76\" SpecialBetValue=\"-2.5\"/><Odd Name=\"1\" ID=\"336210574\" Value=\"1.79\" SpecialBetValue=\"-3.5\"/><Odd Name=\"2\" ID=\"336210573\" Value=\"1.93\" SpecialBetValue=\"-3.5\"/></Bet></Match></Event></Sport>",
        //        "<Sport Name=\"eSports\" ID=\"2357\"><Event Name=\"NBA2K, NBA Battle\" ID=\"83063\" IsLive=\"false\" CategoryID=\"9357\"><Match Name=\"L.A. Lakers (dema21) - Boston Celtics (yaro)\" ID=\"3118821\" StartDate=\"2023-09-25T07:20:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850075\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210324\" Value=\"1.52\"/><Odd Name=\"2\" ID=\"336210323\" Value=\"2.40\"/></Bet><Bet Name=\"Spread\" ID=\"48850074\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336211027\" Value=\"2.02\" SpecialBetValue=\"-5.5\"/><Odd Name=\"2\" ID=\"336211026\" Value=\"1.73\" SpecialBetValue=\"-5.5\"/><Odd Name=\"1\" ID=\"336210431\" Value=\"1.97\" SpecialBetValue=\"-2.5\"/><Odd Name=\"2\" ID=\"336210432\" Value=\"1.76\" SpecialBetValue=\"-2.5\"/><Odd Name=\"1\" ID=\"336210574\" Value=\"1.79\" SpecialBetValue=\"-3.5\"/><Odd Name=\"2\" ID=\"336210573\" Value=\"1.93\" SpecialBetValue=\"-3.5\"/></Bet><Bet Name=\"Total\" ID=\"48850076\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210792\" Value=\"2.02\" SpecialBetValue=\"128.5\"/><Odd Name=\"Under\" ID=\"336210793\" Value=\"1.73\" SpecialBetValue=\"128.5\"/><Odd Name=\"Over\" ID=\"336210810\" Value=\"1.93\" SpecialBetValue=\"129.5\"/><Odd Name=\"Under\" ID=\"336210811\" Value=\"1.79\" SpecialBetValue=\"129.5\"/><Odd Name=\"Over\" ID=\"336210816\" Value=\"1.83\" SpecialBetValue=\"127.5\"/><Odd Name=\"Under\" ID=\"336210817\" Value=\"1.90\" SpecialBetValue=\"127.5\"/><Odd Name=\"Over\" ID=\"336210818\" Value=\"2.02\" SpecialBetValue=\"130.5\"/><Odd Name=\"Under\" ID=\"336210819\" Value=\"1.73\" SpecialBetValue=\"130.5\"/></Bet></Match></Event></Sport>",
        //        "<Sport Name=\"eSports\" ID=\"2357\"><Event Name=\"NBA2K, NBA Battle\" ID=\"83063\" IsLive=\"false\" CategoryID=\"9357\"><Match Name=\"L.A. Lakers (dema21) - Boston Celtics (yaro)\" ID=\"3118821\" StartDate=\"2023-09-25T07:20:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850075\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210324\" Value=\"1.52\"/><Odd Name=\"2\" ID=\"336210323\" Value=\"2.40\"/></Bet><Bet Name=\"Spread\" ID=\"48850074\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336211027\" Value=\"2.02\" SpecialBetValue=\"-5.5\"/><Odd Name=\"2\" ID=\"336211026\" Value=\"1.73\" SpecialBetValue=\"-5.5\"/><Odd Name=\"1\" ID=\"336210431\" Value=\"1.97\" SpecialBetValue=\"-2.5\"/><Odd Name=\"2\" ID=\"336210432\" Value=\"1.76\" SpecialBetValue=\"-2.5\"/><Odd Name=\"1\" ID=\"336210574\" Value=\"1.79\" SpecialBetValue=\"-3.5\"/><Odd Name=\"2\" ID=\"336210573\" Value=\"1.93\" SpecialBetValue=\"-3.5\"/></Bet><Bet Name=\"Total\" ID=\"48850076\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210792\" Value=\"2.02\" SpecialBetValue=\"128.5\"/><Odd Name=\"Under\" ID=\"336210793\" Value=\"1.73\" SpecialBetValue=\"128.5\"/><Odd Name=\"Over\" ID=\"336210810\" Value=\"1.93\" SpecialBetValue=\"129.5\"/><Odd Name=\"Under\" ID=\"336210811\" Value=\"1.79\" SpecialBetValue=\"129.5\"/><Odd Name=\"Over\" ID=\"336210816\" Value=\"1.83\" SpecialBetValue=\"127.5\"/><Odd Name=\"Under\" ID=\"336210817\" Value=\"1.90\" SpecialBetValue=\"127.5\"/><Odd Name=\"Over\" ID=\"336210818\" Value=\"2.02\" SpecialBetValue=\"130.5\"/><Odd Name=\"Under\" ID=\"336210819\" Value=\"1.73\" SpecialBetValue=\"130.5\"/></Bet></Match><Match Name=\"Dallas Mavericks (Lalkoff) - Minnesota Timberwolves (resarke)\" ID=\"3118819\" StartDate=\"2023-09-25T07:23:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850094\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210425\" Value=\"1.90\"/><Odd Name=\"2\" ID=\"336210426\" Value=\"1.83\"/></Bet><Bet Name=\"Spread\" ID=\"48850095\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210853\" Value=\"1.86\" SpecialBetValue=\"2.5\"/><Odd Name=\"2\" ID=\"336210856\" Value=\"1.86\" SpecialBetValue=\"2.5\"/><Odd Name=\"1\" ID=\"336210795\" Value=\"1.79\" SpecialBetValue=\"1.5\"/><Odd Name=\"2\" ID=\"336210794\" Value=\"1.93\" SpecialBetValue=\"1.5\"/><Odd Name=\"1\" ID=\"336210423\" Value=\"2.06\" SpecialBetValue=\"-1.5\"/><Odd Name=\"2\" ID=\"336210424\" Value=\"1.70\" SpecialBetValue=\"-1.5\"/></Bet><Bet Name=\"Total\" ID=\"48850093\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210563\" Value=\"1.90\" SpecialBetValue=\"133.5\"/><Odd Name=\"Under\" ID=\"336210564\" Value=\"1.83\" SpecialBetValue=\"133.5\"/><Odd Name=\"Over\" ID=\"336210815\" Value=\"1.93\" SpecialBetValue=\"137.5\"/><Odd Name=\"Under\" ID=\"336210813\" Value=\"1.79\" SpecialBetValue=\"137.5\"/><Odd Name=\"Over\" ID=\"336210783\" Value=\"2.02\" SpecialBetValue=\"135.5\"/><Odd Name=\"Under\" ID=\"336210782\" Value=\"1.73\" SpecialBetValue=\"135.5\"/><Odd Name=\"Over\" ID=\"336210855\" Value=\"1.86\" SpecialBetValue=\"139.5\"/><Odd Name=\"Under\" ID=\"336210854\" Value=\"1.86\" SpecialBetValue=\"139.5\"/><Odd Name=\"Over\" ID=\"336210857\" Value=\"1.86\" SpecialBetValue=\"136.5\"/><Odd Name=\"Under\" ID=\"336210858\" Value=\"1.86\" SpecialBetValue=\"136.5\"/><Odd Name=\"Over\" ID=\"336210868\" Value=\"10\" SpecialBetValue=\"500\"/><Odd Name=\"Under\" ID=\"336210869\" Value=\"10\" SpecialBetValue=\"500\"/></Bet></Match></Event></Sport>"};

        //    var resultStream = Encoding.UTF8.GetBytes(strings[3]); ;

        //    var serializer = new XmlSerializer(typeof(Sport));
        //    using (TextReader reader = new StreamReader(new MemoryStream(resultStream), Encoding.UTF8))
        //    {
        //        var parsedXml = (Sport)serializer.Deserialize(reader);
        //        return parsedXml;
        //    }

        //    return null;
        //}

        //private HashSet<Event> UpdateInactiveEvents(HashSet<Event> inactiveEvents)
        //{
        //    foreach (var inactiveEvent in inactiveEvents)
        //    {
        //        inactiveEvent.IsActive = false;
        //        inactiveEvent.Matches = UpdateInactiveMatches(inactiveEvent.Matches);
        //    }

        //    return inactiveEvents;
        //}
    }
}
