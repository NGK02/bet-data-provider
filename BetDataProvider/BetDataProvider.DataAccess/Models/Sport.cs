using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [XmlRoot(ElementName = "Sport")]
    public class Sport : Entity
    {
        [XmlElement(ElementName = "Event")]
        public List<Event> Events { get; set; } = new List<Event>();
    }
}
