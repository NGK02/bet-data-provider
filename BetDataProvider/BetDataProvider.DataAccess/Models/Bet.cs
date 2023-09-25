using BetDataProvider.DataAccess.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Bet")]
    public class Bet : Entity, IActivatable
    {
        [XmlAttribute(AttributeName = "IsLive")]
        public bool IsLive { get; set; }

        [XmlElement(ElementName = "Odd")]
        public HashSet<Odd> Odds { get; set; } = new HashSet<Odd>();

        public int MatchId { get; set; }
        public Match Match { get; set; }

        public bool IsActive { get; set; } = true;
    }
}