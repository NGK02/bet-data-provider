using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Models;
using BetDataProvider.DataAccess.Repositories.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.Business.Services
{
    public class XmlHandler : IXmlHandler
    {
        private const string _xmlFeedUrl = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";
        private readonly TimeSpan _feedPullDelay = TimeSpan.FromSeconds(60);
        private Timer _timer;

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

        // improve this method
        public async Task SaveSportData() 
        {
            var sportData = await GetAndParseXmlDataAsync();
            if (sportData is not null)
            {
                _sportRepository.SaveSportData(sportData);
            }
        }
    }
}
