using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        [XmlAttribute(AttributeName = "ID")]
        public int XmlId { get; set; }

        //[Required]
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
