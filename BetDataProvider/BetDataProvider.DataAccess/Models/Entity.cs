using BetDataProvider.DataAccess.Models.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetDataProvider.DataAccess.Models
{
    [Index(nameof(XmlId), IsUnique = true)]
    public abstract class Entity : IAuditable, IEquatable<Entity>
    {
        [Key]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public int XmlId { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        public DateTime CreatedOn => DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool Equals(Entity? entity)
        {
            if (entity is null)
                return false;

            return this.XmlId == entity.XmlId;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Entity)
                return Equals((Entity)obj);

            return false;
        }

        public static bool operator ==(Entity? entity1, Entity? entity2)
        {
            if (ReferenceEquals(entity1, entity2))
            {
                return true;
            }

            if (entity1 is null || entity2 is null)
            {
                return false;
            }

            return entity1.XmlId == entity2.XmlId;
        }

        public static bool operator !=(Entity? entity1, Entity? entity2)
        {
            return !(entity1 == entity2);
        }


        public override int GetHashCode()
        {
            return XmlId.GetHashCode();
        }
    }
}
