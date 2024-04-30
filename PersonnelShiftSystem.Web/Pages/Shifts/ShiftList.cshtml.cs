using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Application.ViewModels.ShiftView;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Shifts
{
    [Authorize(Roles = "Admin")]

    public class ShiftListModel : PageModel
    {
        public IBaseModel baseModel;
        private ShiftService shiftService;

        [BindProperty]
        public List<ShiftViewModel> ShiftModel { get; set; }
        [BindProperty]
        public int RoleId { get; set; }
        public ShiftListModel(IBaseModel _baseModel, ShiftService _shiftService)
        {
            baseModel = _baseModel;
            shiftService = _shiftService;
        }
        public async Task OnGet()
        {
            await baseModel.SaveVisitorInfo();

            var shifts = (await baseModel.BaseUnitOfWork.ShiftRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).ToList();
            ShiftModel = baseModel.Mapper.Map(shifts, ShiftModel);

            var siteUsers = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).ToList();

            ShiftModel.ForEach(x =>
            {
                x.ShiftDate = $"{x.StartingDate.ToString("dd.mm.yyyy")} - {x.EndingDate.ToString("dd.mm.yyyy")}";
                x.ShiftStartEnd = $"{x.ShiftStart} - {x.ShiftEnd}";

                var createdUser = siteUsers.First(z => z.Id == x.CreatedBy); x.CreatedName = $"{createdUser.Name} {createdUser.Surname}";
                var updatedUser = siteUsers.FirstOrDefault(c => c.Id == x.UpdatedBy); x.UpdatedName = (updatedUser == null) ? string.Empty : $"{updatedUser.Name} {updatedUser.Surname}";
            });

            RoleId = Convert.ToInt32(baseModel.ReadFromSession("RoleId"));
        }
        
        public IActionResult OnPostAddShift()
        {
            return RedirectToPage("ShiftAdd");
        }

        public async Task<IActionResult> OnPostDeleteShift()
        {
            int shiftId = Convert.ToInt32(HttpContext.Request.Form["Id"]);
            var deleteResult = await shiftService.DeleteShiftAsync(shiftId);

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
