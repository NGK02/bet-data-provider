using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Match")]
    public class Match : Entity
    {
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "StartDate")]
        public DateTime StartDate { get; set; }

        [XmlAttribute(AttributeName = "MatchType")]
        public MatchType MatchType { get; set; }

        [XmlElement(ElementName = "Bet")]
        public List<Bet> Bets { get; set; } = new List<Bet>();

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
