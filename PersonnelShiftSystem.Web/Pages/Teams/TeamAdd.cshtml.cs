using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Application.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PersonnelShiftSystem.Application.ViewModels.TeamView;
using PersonnelShiftSystem.Application.ViewModels.UserView;
using PersonnelShiftSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JetBrains.Annotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace PersonnelShiftSystem.Web.Pages.Teams
{
    public class TeamAddModel : PageModel
    {
        private IBaseModel baseModel;
        private readonly TeamService teamService;

        [BindProperty]
        public TeamDto TeamModel { get; set; }
        [BindProperty]
        public PersonnelTeamDto PersonnelTeamModel { get; set; }
        public List<PersonnelTeamDto> PersonnelModel { get; set; }
        public List<Siteuser> LeadUserModel { get; set; }
        public List<Siteuser> PersonnelUserModel { get; set; }

        [BindProperty]
        public int TeamLeadId { get; set; }
        [BindProperty]
        public List<int> PersonnelIds { get; set; }

        public TeamAddModel(IBaseModel _baseModel, TeamService _teamService)
        {
            baseModel = _baseModel;
            teamService = _teamService;
        }
        public void OnGet()
        {
            LoadInitials();
        }

        private async void LoadInitials()
        {
            LeadUserModel = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).ToList();
            PersonnelUserModel = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            var getPersonnelTeam = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            var getUserIds = getPersonnelTeam.Select(u => u.UserId).ToHashSet();
            LeadUserModel = LeadUserModel.Where(u => !getUserIds.Contains(u.Id)).ToList();
            PersonnelUserModel = LeadUserModel.Where(u => !getUserIds.Contains(u.Id)).ToList();

        }

        public async Task<IActionResult> OnPostAddTeam()
        {

			if (ModelState.IsValid)
            {
                var rData = HttpContext.Request.Form;
                var personnelIds = rData["PersonnelIds"];
                var teamLeadId = rData["TeamLeadId"];
                                PersonnelModel = new List<PersonnelTeamDto>();


                PersonnelTeamModel = new PersonnelTeamDto()
                {
                    UserId = Convert.ToInt32(teamLeadId),
                    IsTeamLeader = baseModel.ItemActive(),
                    IsActive = baseModel.ItemActive(),
                    CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                    CreatedDate = DateTime.Now,
                    
                };
                PersonnelModel.Add(PersonnelTeamModel);
                foreach ( var personnelId in personnelIds )
                {
                    PersonnelTeamModel = new PersonnelTeamDto()
                    {
                        UserId = Convert.ToInt32(personnelId),
                        IsTeamLeader = baseModel.ItemPassive(),
                        IsActive = baseModel.ItemActive(),
                        CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        CreatedDate = DateTime.Now,

                    };
                    PersonnelModel.Add(PersonnelTeamModel);

                }
                var loginResult = await teamService.AddTeamAsync(TeamModel, PersonnelModel);

				if (loginResult is OkObjectResult okResult)
				{
                    var dataString = okResult.Value.ToString();
                    var data = JObject.Parse(dataString);
                    var isSuccess = (bool)data["isSuccess"];
                    var message = (string)data["message"];
                    var url = (string)data["url"];

                    return new JsonResult(new { isSuccess, message, url });
                }
				else if (loginResult is BadRequestObjectResult badRequestResult)
				{
                    var dataString = JsonConvert.SerializeObject(badRequestResult.Value);
                    var data = JObject.Parse(dataString);
                    var isSuccess = (bool)data["isSuccess"];
                    var message = (string)data["message"];

                    return new JsonResult(new { isSuccess, message });

                }

			}
            else
            {
				return new JsonResult(new { isSuccess = false, message = "Eksik alanları doldurunuz" });
			}

            return new JsonResult(new { isSuccess = false });

        }
    }
}
