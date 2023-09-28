using AutoMapper;
using BetDataProvider.DataAccess.Models;
using BetDataProvider.Web.Dtos;

namespace BetDataProvider.Web.AutoMapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Match, GetMatchDto>();
            CreateMap<Match, GetUpcomingMatchDto>();

            CreateMap<Bet, GetBetDto>();

            CreateMap<Odd, GetOddDto>();
        }
    }
}
