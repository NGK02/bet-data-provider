using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.Business.Services
{
    public class XmlHandler : IXmlHandler
    {
        private const string _xmlFeedUrl = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";

        private readonly HttpClient _httpClient;

        public XmlHandler(HttpClient httpClient) 
        {
            this._httpClient = httpClient;
        }

        public async Task<byte[]> GetXmlDataAsync()
        {
            var uri = new Uri(_xmlFeedUrl);
            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new Exception("Failed fetching the XML data!");
            }
        }

        public Sport? ParseXmlData(byte[] xmlData)
        {
            if (xmlData.IsNullOrEmpty())
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(XmlSports));

            using (TextReader reader = new StreamReader(new MemoryStream(xmlData), Encoding.UTF8))
            {
                var parsedXml = (XmlSports?)serializer.Deserialize(reader);

                if (parsedXml is not null)
                {
                    return parsedXml.Sport;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
