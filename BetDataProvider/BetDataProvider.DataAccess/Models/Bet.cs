using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Bet")]
    public class Bet : Entity
    {
        [XmlAttribute(AttributeName = "IsLive")]
        public bool IsLive { get; set; }

        [XmlElement(ElementName = "Odd")]
        public List<Odd> Odds { get; set; } = new List<Odd>();

        public int MatchId { get; set; }
        public Match Match { get; set; }
    }
}
