using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Application.ViewModels.UserView;
using PersonnelShiftSystem.Application.ViewModels.TeamView;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Teams
{
    [Authorize]

    public class TeamListModel : PageModel
    {
        private IBaseModel baseModel;
        private readonly TeamService teamService;


        public List<TeamViewModel> TeamModel { get; set; }
        [BindProperty]
        public int RoleId { get; set; }
        public TeamListModel(IBaseModel _baseModel, TeamService _teamService)
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
            var teams = (await baseModel.BaseUnitOfWork.TeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            TeamModel = baseModel.Mapper.Map(teams, TeamModel);
            var siteUsers = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            var getTeamMembers = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            TeamModel.ForEach(x =>
            {
                var createdUser = siteUsers.First(c => c.Id == x.CreatedBy); x.CreatedName = $"{createdUser.Name} {createdUser.Surname}";
                var updatedUser = siteUsers.FirstOrDefault(c => c.Id == x.UpdatedBy); x.UpdatedName = (updatedUser == null) ? string.Empty : $"{updatedUser.Name} {updatedUser.Surname}";
                var getLeadInfo = getTeamMembers.First(c => c.TeamId == x.Id && c.IsTeamLeader == baseModel.ItemActive() && c.IsActive == baseModel.ItemActive());
                var getLead = siteUsers.First(c => c.Id == getLeadInfo.UserId && x.IsActive == baseModel.ItemActive()); x.TeamLead = $"{getLead.Name} {getLead.Surname}";
            });

            RoleId = Convert.ToInt32(baseModel.ReadFromSession("RoleId"));
        }

        public IActionResult OnPostAddTeam()
        {
            return RedirectToPage("TeamAdd");
        }
        public IActionResult OnPostTransfer()
        {
            return RedirectToPage("TeamTransfer");
        }
        public async Task<IActionResult> OnPostDeleteTeam()
        {
                int teamId = Convert.ToInt32(HttpContext.Request.Form["Id"]);
                var deleteResult = await teamService.DeleteTeamAsync(teamId);

                if (deleteResult is OkObjectResult okResult)
                {
                var dataString = JsonConvert.SerializeObject(okResult.Value);
                var data = JObject.Parse(dataString);
                    var isSuccess = (bool)data["isSuccess"];

                    return new JsonResult(new { isSuccess});
                }
                else if (deleteResult is BadRequestObjectResult badRequestResult)
                {
                    var dataString = JsonConvert.SerializeObject(badRequestResult.Value);
                    var data = JObject.Parse(dataString);
                    var isSuccess = (bool)data["isSuccess"];
                    var message = (string)data["message"];

                    return new JsonResult(new { isSuccess, message });

                }           

            return new JsonResult(new { isSuccess = false });
        }
    }
}
