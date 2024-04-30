using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using PersonnelShiftSystem.Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.AssignShiftTeams
{
    [Authorize(Roles = "Admin")]

    public class AssignShiftTeamAddModel : PageModel
    {
        private IBaseModel baseModel;
        private readonly AssignShiftTeamService assignService;


        [BindProperty]
        public AssignShiftTeamDto AssignShiftModel { get; set; }

        public List<Team> TeamModel { get; set; }
        public List<Shift> ShiftModel { get; set; }

        public List<TeamDto> TeamDtoModel { get; set; }

        [BindProperty]
        public int ShiftId { get; set; }
        [BindProperty]
        public List<int> TeamIds { get; set; }

        public AssignShiftTeamAddModel(IBaseModel _baseModel, AssignShiftTeamService _assignService)
        {
            baseModel = _baseModel;
            assignService = _assignService;
        }
        public void OnGet()
        {
            LoadInitials();
        }

        private async void LoadInitials()
        {
            TeamModel = (await baseModel.BaseUnitOfWork.TeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            ShiftModel = (await baseModel.BaseUnitOfWork.ShiftRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            var getAssignedTeam = (await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            var getTeamIds = getAssignedTeam.Select(u => u.TeamId).ToHashSet();
            TeamModel = TeamModel.Where(u => !getTeamIds.Contains(u.Id)).ToList();
        }

       
        public async Task<IActionResult> OnPostAddTeam()
        {

            if (ModelState.IsValid)
            {
                var rData = HttpContext.Request.Form;
                var teamIds = rData["TeamIds[]"].ToList();
                var shiftId = Convert.ToInt32(rData["ShiftId"]);
                TeamDtoModel = new List<TeamDto>();

                foreach (var teamId in teamIds)
                {
                    TeamDto teamDto = new TeamDto()
                    {
                        Id = Convert.ToInt32(teamId),
                        IsActive = baseModel.ItemActive(),

                    };
                    TeamDtoModel.Add(teamDto);

                }
                var result = await assignService.AddAssignTeamAsync(TeamDtoModel, shiftId);

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

            return new JsonResult(new { isSuccess = false });

        }

        public async Task<IActionResult> OnPostCancel()
        {
            return RedirectToPage("AssignShiftTeamList");
        }
    }
}
