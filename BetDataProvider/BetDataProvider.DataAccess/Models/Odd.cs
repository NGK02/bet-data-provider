using BetDataProvider.DataAccess.Models.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Odd")]
    public class Odd : Entity, IActivatable
    {
        //[Required]
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "SpecialBetValue")]
        public string? SpecialBetValue { get; set; }

        public int BetId { get; set; }
        public Bet Bet { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
