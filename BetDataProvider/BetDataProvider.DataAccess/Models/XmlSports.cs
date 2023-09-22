using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "XmlSports")]
    public class XmlSports
    {
        [XmlElement(ElementName = "Sport")]
        public Sport Sport { get; set; }

        [XmlAttribute(AttributeName = "CreateDate")]
        public DateTime CreateDate { get; set; }
    }
}
