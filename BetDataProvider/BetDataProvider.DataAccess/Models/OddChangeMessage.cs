using BetDataProvider.DataAccess.Models.Contracts;
using BetDataProvider.DataAccess.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Models
{
    public class OddChangeMessage : IAuditable
    {
        public OddChangeMessage(int oddId, MessageType messageType)
        {
            OddId = oddId;
            MessageType = messageType;
        }

        public int Id { get; set; }

        public int OddId { get; set; }
        public Odd Odd { get; set; }

        public MessageType MessageType { get; set; }

        public DateTime CreatedOn => DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
