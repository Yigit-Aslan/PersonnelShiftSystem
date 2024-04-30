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
            CreateMap<ViewModels.ShiftView.ShiftViewModel, Domain.Models.Shift>().ReverseMap();
            CreateMap<ViewModels.AssignView.AssignShiftViewModel, Domain.Models.AssignShiftTeam>().ReverseMap();

            CreateMap<Dtos.TeamDto, Domain.Models.Team>().ReverseMap();
            CreateMap<Dtos.PersonnelTeamDto, Domain.Models.PersonnelTeam>().ReverseMap();
            CreateMap<Domain.Models.PersonnelTeam, Dtos.PersonnelTeamDto>().ReverseMap();
            CreateMap<Dtos.ShiftDto, Domain.Models.Shift>().ReverseMap();
            CreateMap<Dtos.AssignShiftTeamDto, Domain.Models.AssignShiftTeam>().ReverseMap();
            CreateMap<Dtos.RoleDto, Domain.Models.Role>().ReverseMap();
            CreateMap<Dtos.UserDto, Domain.Models.Siteuser>().ReverseMap();
        }
    }
}
