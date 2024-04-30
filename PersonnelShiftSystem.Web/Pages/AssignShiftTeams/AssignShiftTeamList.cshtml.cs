using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Application.ViewModels.AssignView;
using PersonnelShiftSystem.Application.ViewModels.ShiftView;
using PersonnelShiftSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.AssignShiftTeams
{
    [Authorize]

    public class AssignShiftTeamListModel : PageModel
    {
        public IBaseModel baseModel;
        private AssignShiftTeamService assignShiftTeamService;

        [BindProperty]
        public List<AssignShiftViewModel> AssignShiftModel { get; set; }

        [BindProperty]
        public int RoleId { get; set; }
        public AssignShiftTeamListModel(IBaseModel _baseModel, AssignShiftTeamService _assignShiftTeamService)
        {
            baseModel = _baseModel;
            assignShiftTeamService = _assignShiftTeamService;
        }
        public async Task OnGet()
        {
            await baseModel.SaveVisitorInfo();

            var assignedShifts = (await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            AssignShiftModel = baseModel.Mapper.Map(assignedShifts, AssignShiftModel);

            var siteUsers = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            var getTeams = await baseModel.BaseUnitOfWork.TeamRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive());

            var getShifts = await baseModel.BaseUnitOfWork.ShiftRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive());


            AssignShiftModel.ForEach(x =>
            {
                x.ShiftName = getShifts.First(z=>z.Id == x.ShiftId).ShiftName;
                x.StartingDate = getShifts.First(z => z.Id == x.ShiftId).StartingDate;
                x.EndingDate = getShifts.First(z => z.Id == x.ShiftId).EndingDate;
                x.ShiftStart = getShifts.First(z => z.Id == x.ShiftId).ShiftStart;
                x.ShiftEnd = getShifts.First(z => z.Id == x.ShiftId).ShiftEnd;

                x.TeamName = getTeams.First(z=>z.Id == x.TeamId).TeamName;

                x.ShiftDate = $"{x.StartingDate.ToString("dd.mm.yyyy")} - {x.EndingDate.ToString("dd.mm.yyyy")}";
                x.ShiftStartEnd = $"{x.ShiftStart} - {x.ShiftEnd}";

                var createdUser = siteUsers.First(z => z.Id == x.CreatedBy); x.CreatedName = $"{createdUser.Name} {createdUser.Surname}";
                var updatedUser = siteUsers.FirstOrDefault(c => c.Id == x.UpdatedBy); x.UpdatedName = (updatedUser == null) ? string.Empty : $"{updatedUser.Name} {updatedUser.Surname}";
            });

            RoleId = Convert.ToInt32(baseModel.ReadFromSession("RoleId"));
        }

        public IActionResult OnPostAddTeamShift()
        {
            return RedirectToPage("AssignShiftTeamAdd");
        }

        public async Task<IActionResult> OnPostDeleteTeam()
        {
            int assignedId = Convert.ToInt32(HttpContext.Request.Form["Id"]);
            var deleteResult = await assignShiftTeamService.DeleteAssignAsync(assignedId);

            if (deleteResult is OkObjectResult okResult)
            {
                var dataString = JsonConvert.SerializeObject(okResult.Value);
                var data = JObject.Parse(dataString);
                var isSuccess = (bool)data["isSuccess"];

                return new JsonResult(new { isSuccess });
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
