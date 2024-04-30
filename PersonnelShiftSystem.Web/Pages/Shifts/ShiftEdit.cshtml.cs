using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Shifts
{
    [Authorize(Roles = "Admin")]

    public class ShiftEditModel : PageModel
    {
        public IBaseModel baseModel;
        private ShiftService shiftService;

        [BindProperty]
        public ShiftDto ShiftModel { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public ShiftEditModel(IBaseModel _baseModel, ShiftService _shiftService)
        {
            baseModel = _baseModel;
            shiftService = _shiftService;
        }
        public async void OnGet()
        {
            var shift = await baseModel.BaseUnitOfWork.ShiftRepository.GetFirstOrDefaultAsync(x => x.Id == Id && x.IsActive == baseModel.ItemActive());

            ShiftModel = new ShiftDto()
            {
                Id = Id,
                ShiftName = shift.ShiftName,
                StartingDate = shift.StartingDate.ToString("dd.MM.yyyy"),
                EndingDate = shift.EndingDate.ToString("dd.MM.yyyy"),
                ShiftStart = shift.ShiftStart.ToString(),
                ShiftEnd = shift.ShiftEnd.ToString(),
                CreatedBy = shift.CreatedBy,
                CreatedDate = shift.CreatedDate,
            };

        }

        public async Task<IActionResult> OnPostEditShift()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ShiftModel.ShiftName = HttpContext.Request.Form["ShiftName"];
                    var result = await shiftService.EditShiftAsync(ShiftModel);

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
            return RedirectToPage("ShiftList");
        }
    }
}
