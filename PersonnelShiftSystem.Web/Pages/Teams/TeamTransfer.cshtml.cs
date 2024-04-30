using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Teams
{
    [Authorize(Roles = "Admin")]

    public class TeamTransferModel : PageModel
    {
        private IBaseModel baseModel;
        private TeamService _teamService;

        public List<TeamDto> TeamListModel { get; set; }
        public List<Siteuser> UserModel { get; set; }
        public PersonnelTeamDto PersonnelModel { get; set; }
        public TeamTransferModel(IBaseModel _baseModel, TeamService teamService)
        {
            baseModel = _baseModel;
            _teamService = teamService;
        }
        public async void OnGet()
        {
            var teamModel = (await baseModel.BaseUnitOfWork.TeamRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).ToList();
            TeamListModel = baseModel.Mapper.Map(teamModel, TeamListModel);

            var users = baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive());

        }

        public async Task<IActionResult> OnGetFilterTeamPersonnel(int teamId)
        {
            try
            {
                List<Siteuser> PersonnelModel = new List<Siteuser>();
                var teamUsers = ( await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x=>x.TeamId == teamId && x.IsActive == baseModel.ItemActive()));
                var users = ( await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).ToList();

                PersonnelModel = (from user in users
                                  join siteuser in teamUsers
                                  on user.Id equals siteuser.UserId
                                  where user.IsActive == baseModel.ItemActive()
                                  select user).ToList();
                var getTeamLeadId = teamUsers.First(x=>x.IsTeamLeader == baseModel.ItemActive() && x.IsActive == baseModel.ItemActive()).UserId;
                PersonnelModel.RemoveAll(x => x.Id == getTeamLeadId);


                return new JsonResult(new { Result = "Success", PersonnelModel = PersonnelModel });
            }
            catch (Exception)
            {
                return new JsonResult(new { Result = "Failed" });

            }

        }

        public async Task<IActionResult> OnGetFilterOtherTeams(int teamId)
        {
            try
            {
                List<Team> TeamModel = new List<Team>();

                TeamModel = ( await baseModel.BaseUnitOfWork.TeamRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).ToList();

                TeamModel.RemoveAll(x=>x.Id == teamId);



                return new JsonResult(new { Result = "Success", TeamModel = TeamModel });
            }
            catch (Exception)
            {
                return new JsonResult(new { Result = "Failed" });

            }

        }


        public async Task<IActionResult> OnPostTransfer()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rData = HttpContext.Request.Form;
                    int teamId = Convert.ToInt32(rData["TeamId"]);
                    int personnelId = Convert.ToInt32(rData["PersonnelId"]);
                    int teamTransferId = Convert.ToInt32(rData["TeamTransferId"]);
                    PersonnelModel = new PersonnelTeamDto()
                    {
                        TeamId = teamId,
                        UserId = personnelId,
                        IsTeamLeader = baseModel.ItemPassive(),
                        
                    };
                    var result = await _teamService.TransferTeamAsync(PersonnelModel, teamTransferId);

                    if (result is OkObjectResult okResult)
                    {
                        var dataString = JsonConvert.SerializeObject(okResult.Value);
                        var data = JObject.Parse(dataString);
                        var isSuccess = (bool)data["isSuccess"];
                        var message = (string)data["message"];
                        var url = (string)data["url"];

                        return new JsonResult(new { isSuccess, message, url });
                    }
                    else if (result is BadRequestObjectResult badRequestResult)
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
                    ModelState.Clear();

                    return new JsonResult(new { isSuccess = false, message = "Eksik alanları doldurunuz" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { isSuccess = false, message = "Beklenmedik Sistem Hatası" });

            }
            return new JsonResult(new { isSuccess = false });

        }

        public async Task<IActionResult> OnPostCancel()
        {
            return RedirectToPage("TeamList");
        }
    }
}
