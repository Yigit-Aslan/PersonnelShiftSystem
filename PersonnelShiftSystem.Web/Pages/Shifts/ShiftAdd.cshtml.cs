using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;
using System;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Shifts
{
    [Authorize(Roles = "Admin")]

    public class ShiftAddModel : PageModel
    {
        private IBaseModel baseModel;
        private ShiftService shiftService;
        [BindProperty]
        public ShiftDto ShiftModel { get; set; }
       
        public ShiftAddModel(IBaseModel _baseModel, ShiftService _shiftService)
        {
            baseModel = _baseModel;
            shiftService = _shiftService;

        }

        public async Task OnGet()
        {
            await baseModel.SaveVisitorInfo();

        }

        public async Task<IActionResult> OnPostAddShift()
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
                    var result = await shiftService.AddShiftAsync(ShiftModel);

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
            return await Task.FromResult(RedirectToPage("ShiftList"));
        }
    }
}
