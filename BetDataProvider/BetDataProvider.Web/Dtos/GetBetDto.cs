namespace BetDataProvider.Web.Dtos
{
    public class GetBetDto
    {
        public string Name { get; set; }

        public bool IsLive { get; set; }

        public HashSet<GetOddDto> Odds { get; set; } = new HashSet<GetOddDto>();

        public bool IsActive { get; set; }
    }
}
