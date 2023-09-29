using BetDataProvider.DataAccess.Models.Contracts;
using BetDataProvider.DataAccess.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.DataAccess.Models
{
    public sealed class MatchChangeMessage : IAuditable
    {
        public MatchChangeMessage(int matchId, MessageType messageType)
        {
            MatchId = matchId;
            MessageType = messageType;
        }

        public int Id { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; }

        public MessageType MessageType { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
