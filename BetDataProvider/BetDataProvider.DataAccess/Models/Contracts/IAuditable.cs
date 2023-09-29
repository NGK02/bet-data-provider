using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Models.Contracts
{
    public interface IAuditable
    {
        public DateTime CreatedOn => DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
