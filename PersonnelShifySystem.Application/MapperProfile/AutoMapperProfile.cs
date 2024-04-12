using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Application.MapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ViewModels.UserView.UserViewModel, Domain.Models.Siteuser>().ReverseMap();
            CreateMap<ViewModels.TeamView.TeamViewModel, Domain.Models.Team>().ReverseMap();
            CreateMap<Dtos.TeamDto, Domain.Models.Team>().ReverseMap();
            CreateMap<Dtos.PersonnelTeamDto, Domain.Models.PersonnelTeam>().ReverseMap();
        }
    }
}
