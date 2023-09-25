using BetDataProvider.DataAccess.Models.Contracts;
using BetDataProvider.DataAccess.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Match")]
    public class Match : Entity, IActivatable
    {
        [XmlAttribute(AttributeName = "StartDate")]
        public DateTime StartDate { get; set; }

        [XmlAttribute(AttributeName = "MatchType")]
        public MatchTypeEnum MatchType { get; set; }

        [XmlElement(ElementName = "Bet")]
        public HashSet<Bet> Bets { get; set; } = new HashSet<Bet>();

        public int EventId { get; set; }
        public Event Event { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
