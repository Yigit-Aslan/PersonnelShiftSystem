using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.AssignShiftTeams
{
    [Authorize(Roles = "Admin")]

    public class AssignShiftTeamEditModel : PageModel
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
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public AssignShiftTeamEditModel(IBaseModel _baseModel, AssignShiftTeamService _assignService)
        {
            baseModel = _baseModel;
            assignService = _assignService;
        }
        public async Task OnGet()
        {
            await baseModel.SaveVisitorInfo();

            await LoadInitials();
        }

        private async Task LoadInitials()
        {
            var assigned = await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.GetFirstOrDefaultAsync(x=>x.Id == Id && x.IsActive == baseModel.ItemActive());
            AssignShiftModel = baseModel.Mapper.Map(assigned, AssignShiftModel);

            TeamModel = (await baseModel.BaseUnitOfWork.TeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            ShiftModel = (await baseModel.BaseUnitOfWork.ShiftRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            var getAssignedTeam = (await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            var getTeamIds = getAssignedTeam.Select(u => u.TeamId).ToHashSet();
            TeamModel = TeamModel.Where(u => !getTeamIds.Contains(u.Id)).ToList();

            var teamData = await baseModel.BaseUnitOfWork.TeamRepository.GetFirstOrDefaultAsync(x=>x.Id == AssignShiftModel.TeamId && x.IsActive == baseModel.ItemActive());

            TeamModel.Add(teamData);
        }

        public async Task<IActionResult> OnPostEditTeam()
        {

            if (ModelState.IsValid)
            {
                var rData = HttpContext.Request.Form;
                var teamId = Convert.ToInt32(rData["TeamId"]);
                var shiftId = Convert.ToInt32(rData["ShiftId"]);

                AssignShiftModel = new AssignShiftTeamDto()
                {
                    Id = Id,
                    TeamId = teamId,
                    ShiftId = shiftId,
                };
                var result = await assignService.EditAssignTeamAsync(AssignShiftModel);

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
            return await Task.FromResult(RedirectToPage("AssignShiftTeamList"));
        }
    }
}
