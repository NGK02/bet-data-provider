using BetDataProvider.DataAccess.Models.Contracts;
using BetDataProvider.DataAccess.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Models
{
    public class BetChangeMessage : IAuditable
    {
        public BetChangeMessage(int betId, MessageType messageType) 
        {
            BetId = betId;
            MessageType = messageType;
        }

        public int Id { get; set; }

        public int BetId { get; set; }
        public Bet Bet { get; set; }

        public MessageType MessageType { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
