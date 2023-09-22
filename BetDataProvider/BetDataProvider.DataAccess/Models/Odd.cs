using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Odd")]
    public class Odd : Entity
    {
        [XmlAttribute(AttributeName = "Value")]
        public decimal Value { get; set; }

        //[XmlAttribute(AttributeName = "SpecialBetValue")]
        public decimal? SpecialBetValue { get; set; }

        public int BetId { get; set; }
        public Bet Bet { get; set; }
    }
}
