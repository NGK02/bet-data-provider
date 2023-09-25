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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BetDataProvider.Business.Services
{
    public class XmlHandler : IXmlHandler
    {
        private const string _xmlFeedUrl = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";

        private readonly ISportRepository _sportRepository;

        public XmlHandler(ISportRepository sportRepository)
        {
            _sportRepository = sportRepository;
        }

        public async Task<Sport> GetAndParseXmlDataAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(_xmlFeedUrl);

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
            }

            return null;
        }

        //public Sport GetAndParseXmlDataAsync()
        //{
        //    string[] strings = new string[2] { "<Sport Name=\"eSports\" ID=\"2357\"><Event Name=\"NBA2K, NBA Battle\" ID=\"83063\" IsLive=\"false\" CategoryID=\"9357\"><Match Name=\"L.A. Lakers (dema21) - Boston Celtics (yaro)\" ID=\"3118821\" StartDate=\"2023-09-25T07:20:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850075\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210324\" Value=\"2.29\"/><Odd Name=\"2\" ID=\"336210323\" Value=\"1.57\"/></Bet><Bet Name=\"Spread\" ID=\"48850074\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210864\" Value=\"1.83\" SpecialBetValue=\"2.5\"/><Odd Name=\"2\" ID=\"336210863\" Value=\"1.90\" SpecialBetValue=\"2.5\"/><Odd Name=\"1\" ID=\"336210606\" Value=\"2.02\" SpecialBetValue=\"-1.5\"/><Odd Name=\"2\" ID=\"336210605\" Value=\"1.73\" SpecialBetValue=\"-1.5\"/></Bet><Bet Name=\"Total\" ID=\"48850076\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210792\" Value=\"2.02\" SpecialBetValue=\"128.5\"/><Odd Name=\"Under\" ID=\"336210793\" Value=\"1.73\" SpecialBetValue=\"128.5\"/><Odd Name=\"Over\" ID=\"336210810\" Value=\"1.93\" SpecialBetValue=\"129.5\"/><Odd Name=\"Under\" ID=\"336210811\" Value=\"1.79\" SpecialBetValue=\"129.5\"/><Odd Name=\"Over\" ID=\"336210816\" Value=\"1.83\" SpecialBetValue=\"127.5\"/><Odd Name=\"Under\" ID=\"336210817\" Value=\"1.90\" SpecialBetValue=\"127.5\"/><Odd Name=\"Over\" ID=\"336210818\" Value=\"2.02\" SpecialBetValue=\"130.5\"/><Odd Name=\"Under\" ID=\"336210819\" Value=\"1.73\" SpecialBetValue=\"130.5\"/></Bet></Match><Match Name=\"Dallas Mavericks (Lalkoff) - Minnesota Timberwolves (resarke)\" ID=\"3118819\" StartDate=\"2023-09-25T07:23:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850094\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210425\" Value=\"1.90\"/><Odd Name=\"2\" ID=\"336210426\" Value=\"1.83\"/></Bet><Bet Name=\"Spread\" ID=\"48850095\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210853\" Value=\"1.86\" SpecialBetValue=\"2.5\"/><Odd Name=\"2\" ID=\"336210856\" Value=\"1.86\" SpecialBetValue=\"2.5\"/><Odd Name=\"1\" ID=\"336210795\" Value=\"1.79\" SpecialBetValue=\"1.5\"/><Odd Name=\"2\" ID=\"336210794\" Value=\"1.93\" SpecialBetValue=\"1.5\"/><Odd Name=\"1\" ID=\"336210423\" Value=\"2.06\" SpecialBetValue=\"-1.5\"/><Odd Name=\"2\" ID=\"336210424\" Value=\"1.70\" SpecialBetValue=\"-1.5\"/></Bet><Bet Name=\"Total\" ID=\"48850093\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210563\" Value=\"1.90\" SpecialBetValue=\"133.5\"/><Odd Name=\"Under\" ID=\"336210564\" Value=\"1.83\" SpecialBetValue=\"133.5\"/><Odd Name=\"Over\" ID=\"336210815\" Value=\"1.93\" SpecialBetValue=\"137.5\"/><Odd Name=\"Under\" ID=\"336210813\" Value=\"1.79\" SpecialBetValue=\"137.5\"/><Odd Name=\"Over\" ID=\"336210783\" Value=\"2.02\" SpecialBetValue=\"135.5\"/><Odd Name=\"Under\" ID=\"336210782\" Value=\"1.73\" SpecialBetValue=\"135.5\"/><Odd Name=\"Over\" ID=\"336210855\" Value=\"1.86\" SpecialBetValue=\"139.5\"/><Odd Name=\"Under\" ID=\"336210854\" Value=\"1.86\" SpecialBetValue=\"139.5\"/><Odd Name=\"Over\" ID=\"336210857\" Value=\"1.86\" SpecialBetValue=\"136.5\"/><Odd Name=\"Under\" ID=\"336210858\" Value=\"1.86\" SpecialBetValue=\"136.5\"/></Bet></Match><Match Name=\"Cleveland Cavaliers (yangrainmaker) - Atlanta Hawks (Tapachan)\" ID=\"3118820\" StartDate=\"2023-09-25T07:23:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850090\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210406\" Value=\"3.47\"/><Odd Name=\"2\" ID=\"336210409\" Value=\"1.27\"/></Bet><Bet Name=\"Spread\" ID=\"48850091\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210825\" Value=\"1.79\" SpecialBetValue=\"6.5\"/><Odd Name=\"2\" ID=\"336210824\" Value=\"1.93\" SpecialBetValue=\"6.5\"/></Bet><Bet Name=\"Total\" ID=\"48850092\" IsLive=\"true\"><Odd Name=\"Over\" ID=\"336210405\" Value=\"2.11\" SpecialBetValue=\"142.5\"/><Odd Name=\"Under\" ID=\"336210408\" Value=\"1.67\" SpecialBetValue=\"142.5\"/><Odd Name=\"Over\" ID=\"336210784\" Value=\"2.02\" SpecialBetValue=\"143.5\"/><Odd Name=\"Under\" ID=\"336210786\" Value=\"1.73\" SpecialBetValue=\"143.5\"/><Odd Name=\"Over\" ID=\"336210662\" Value=\"1.83\" SpecialBetValue=\"140.5\"/><Odd Name=\"Under\" ID=\"336210661\" Value=\"1.90\" SpecialBetValue=\"140.5\"/><Odd Name=\"Over\" ID=\"336210612\" Value=\"1.86\" SpecialBetValue=\"141.5\"/><Odd Name=\"Under\" ID=\"336210610\" Value=\"1.86\" SpecialBetValue=\"141.5\"/><Odd Name=\"Over\" ID=\"336210800\" Value=\"2.02\" SpecialBetValue=\"144.5\"/><Odd Name=\"Under\" ID=\"336210801\" Value=\"1.73\" SpecialBetValue=\"144.5\"/></Bet></Match><Match Name=\"Phoenix Suns (padiy) - Boston Celtics (yaro)\" ID=\"3118833\" StartDate=\"2023-09-25T07:55:00\" MatchType=\"PreMatch\"><Bet Name=\"Money Line\" ID=\"48850026\" IsLive=\"false\"><Odd Name=\"1\" ID=\"336210022\" Value=\"1.18\"/><Odd Name=\"2\" ID=\"336210021\" Value=\"4.40\"/></Bet><Bet Name=\"Spread\" ID=\"48850025\" IsLive=\"false\"><Odd Name=\"1\" ID=\"336210023\" Value=\"1.86\" SpecialBetValue=\"-9.5\"/><Odd Name=\"2\" ID=\"336210018\" Value=\"1.86\" SpecialBetValue=\"-9.5\"/></Bet><Bet Name=\"Total\" ID=\"48850027\" IsLive=\"false\"><Odd Name=\"Over\" ID=\"336210019\" Value=\"1.83\" SpecialBetValue=\"119.5\"/><Odd Name=\"Under\" ID=\"336210020\" Value=\"1.89\" SpecialBetValue=\"119.5\"/></Bet></Match></Event></Sport>",
        //        "<Sport Name=\"eSports\" ID=\"2357\"><Event Name=\"NBA2K, NBA Battle\" ID=\"83063\" IsLive=\"false\" CategoryID=\"9357\"><Match Name=\"L.A. Lakers (dema21) - Boston Celtics (yaro)\" ID=\"3118821\" StartDate=\"2023-09-25T07:20:00\" MatchType=\"Live\"><Bet Name=\"Money Line\" ID=\"48850075\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336210324\" Value=\"1.52\"/><Odd Name=\"2\" ID=\"336210323\" Value=\"2.40\"/></Bet><Bet Name=\"Spread\" ID=\"48850074\" IsLive=\"true\"><Odd Name=\"1\" ID=\"336211027\" Value=\"2.02\" SpecialBetValue=\"-5.5\"/><Odd Name=\"2\" ID=\"336211026\" Value=\"1.73\" SpecialBetValue=\"-5.5\"/><Odd Name=\"1\" ID=\"336210431\" Value=\"1.97\" SpecialBetValue=\"-2.5\"/><Odd Name=\"2\" ID=\"336210432\" Value=\"1.76\" SpecialBetValue=\"-2.5\"/><Odd Name=\"1\" ID=\"336210574\" Value=\"1.79\" SpecialBetValue=\"-3.5\"/><Odd Name=\"2\" ID=\"336210573\" Value=\"1.93\" SpecialBetValue=\"-3.5\"/></Bet></Match></Event></Sport>" };

        //    var resultStream = Encoding.UTF8.GetBytes(strings[1]); ;

        //    var serializer = new XmlSerializer(typeof(Sport));
        //    using (TextReader reader = new StreamReader(new MemoryStream(resultStream), Encoding.UTF8))
        //    {
        //        var parsedXml = (Sport)serializer.Deserialize(reader);
        //        return parsedXml;
        //    }

        //    return null;
        //}

        public bool SaveSportData(Sport newSportData)
        {
            // maybe first check if ANY sport data instead of active, not sure if needed
            var oldSportData = _sportRepository.GetActiveSportData();

            if (newSportData is not null)
            {
                // not sure what to do if Sport exists but no Events
                if (oldSportData is null)
                {
                    _sportRepository.SaveSportData(newSportData);
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

            var updatedSportData = ProcessEvents(oldEvents, newEvents);

            oldSportData.Events = updatedSportData;

            _sportRepository.UpdateSportData(oldSportData);

            return true;
        }

        private HashSet<Event> ProcessEvents(HashSet<Event> oldEvents, HashSet<Event> newEvents)
        {
            HashSet<Event> updatedEvents = new HashSet<Event>();

            foreach (Event newEvent in newEvents)
            {
                var updatedEvent = oldEvents.FirstOrDefault(e => e.XmlId == newEvent.XmlId);

                // add new event
                if (updatedEvent is null)
                {
                    updatedEvents.Add(newEvent);
                    continue;
                }

                // update old active event
                updatedEvent.Matches = ProcessMatches(updatedEvent.Matches, newEvent.Matches);

                oldEvents.Remove(updatedEvent);

                updatedEvents.Add(updatedEvent);
            }

            // set old active event to inactive
            updatedEvents.UnionWith(ProcessInactiveEvents(oldEvents));

            return updatedEvents;
        }

        private HashSet<Match> ProcessMatches(HashSet<Match> oldMatches, HashSet<Match> newMatches)
        {
            HashSet<Match> updatedMatches = new HashSet<Match>();

            foreach (var newMatch in newMatches)
            {
                var updatedMatch = oldMatches.FirstOrDefault(e => e.XmlId == newMatch.XmlId);

                if (updatedMatch is null)
                {
                    updatedMatches.Add(newMatch);
                    continue;
                }

                updatedMatch.MatchType = newMatch.MatchType;
                updatedMatch.StartDate = newMatch.StartDate;
                updatedMatch.Bets = ProcessBets(updatedMatch.Bets, newMatch.Bets);

                oldMatches.Remove(updatedMatch);

                updatedMatches.Add(updatedMatch);
            }

            updatedMatches.UnionWith(ProcessInactiveMatches(oldMatches));

            return updatedMatches;
        }

        private HashSet<Bet> ProcessBets(HashSet<Bet> oldBets, HashSet<Bet> newBets)
        {
            HashSet<Bet> updatedBets = new HashSet<Bet>();

            foreach (var newBet in newBets)
            {
                var updatedBet = oldBets.FirstOrDefault(e => e.XmlId == newBet.XmlId);

                if (updatedBet is null)
                {
                    updatedBets.Add(newBet);
                    continue;
                }

                updatedBet.Odds = ProcessOdds(updatedBet.Odds, newBet.Odds);

                oldBets.Remove(updatedBet);

                updatedBets.Add(updatedBet);
            }

            updatedBets.UnionWith(ProcessInactiveBets(oldBets));

            return updatedBets;
        }

        private HashSet<Odd> ProcessOdds(HashSet<Odd> oldOdds, HashSet<Odd> newOdds)
        {
            HashSet<Odd> updatedOdds = new HashSet<Odd>();

            foreach (var newOdd in newOdds)
            {
                var updatedOdd = oldOdds.FirstOrDefault(e => e.XmlId == newOdd.XmlId);

                if (updatedOdd is null)
                {
                    updatedOdds.Add(newOdd);
                    continue;
                }

                updatedOdd.Value = newOdd.Value;
                updatedOdd.SpecialBetValue = newOdd.SpecialBetValue;

                oldOdds.Remove(updatedOdd);

                updatedOdds.Add(updatedOdd);
            }

            updatedOdds.UnionWith(ProcessInactiveOdds(oldOdds));

            return updatedOdds;
        }

        //private HashSet<T> ProcessInactiveEntities<T>(HashSet<T> inactiveEntities, HashSet<T> updatedEntities) where T : IActivatable
        //{
        //    foreach (var inactiveEntity in inactiveEntities)
        //    {
        //        inactiveEntity.IsActive = false;

        //        updatedEntities.Add(inactiveEntity);
        //    }

        //    return updatedEntities;
        //}

        private HashSet<Event> ProcessInactiveEvents(HashSet<Event> inactiveEvents)
        {
            foreach (var inactiveEvent in inactiveEvents)
            {
                inactiveEvent.IsActive = false;
                inactiveEvent.Matches = ProcessInactiveMatches(inactiveEvent.Matches);
            }

            return inactiveEvents;
        }

        private HashSet<Match> ProcessInactiveMatches(HashSet<Match> inactiveMatches)
        {
            foreach (var inactiveMatch in inactiveMatches)
            {
                inactiveMatch.IsActive = false;
                inactiveMatch.Bets = ProcessInactiveBets(inactiveMatch.Bets);
            }
            return inactiveMatches;
        }

        private HashSet<Bet> ProcessInactiveBets(HashSet<Bet> inactiveBets)
        {
            foreach (var inactiveBet in inactiveBets)
            {
                inactiveBet.IsActive = false;
                inactiveBet.Odds = ProcessInactiveOdds(inactiveBet.Odds);
            }
            return inactiveBets;
        }

        private HashSet<Odd> ProcessInactiveOdds(HashSet<Odd> inactiveOdds)
        {
            foreach (var inactiveOdd in inactiveOdds)
            {
                inactiveOdd.IsActive = false;
            }
            return inactiveOdds;
        }
    }
}
