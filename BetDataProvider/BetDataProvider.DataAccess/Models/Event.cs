using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Event")]
    public class Event : Entity
    {
        [XmlAttribute(AttributeName = "IsLive")]
        public bool IsLive { get; set; }

        [XmlAttribute(AttributeName = "CategoryID")]
        public int CategoryID { get; set; }

        [XmlElement(ElementName = "Match")]
        public List<Match> Matches { get; set; } = new List<Match>();

        public int SportId { get; set; }
        public Sport Sport { get; set; }
    }
}
