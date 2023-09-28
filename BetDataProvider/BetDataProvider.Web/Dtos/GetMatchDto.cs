using BetDataProvider.DataAccess.Models.Enums;

namespace BetDataProvider.Web.Dtos
{
    public class GetMatchDto
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string MatchType { get; set; }

        public HashSet<GetBetDto> Bets { get; set; } = new HashSet<GetBetDto>();

        public bool IsActive { get; set; }
    }
}
