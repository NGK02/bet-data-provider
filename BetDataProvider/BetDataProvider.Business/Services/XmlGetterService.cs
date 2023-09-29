using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.Business.Services
{
    public class XmlGetterService : IXmlGetterService
    {
        private const string _xmlFeedUrl = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";

        private readonly HttpClient _httpClient;

        public XmlGetterService(HttpClient httpClient) 
        {
            this._httpClient = httpClient;
        }

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
                    var parsedXml = (XmlSports?)serializer.Deserialize(reader);

                    if (parsedXml is not null)
                    {
                        return parsedXml.Sport;
                    }
                }
            }

            return null;
        }
    }
}
