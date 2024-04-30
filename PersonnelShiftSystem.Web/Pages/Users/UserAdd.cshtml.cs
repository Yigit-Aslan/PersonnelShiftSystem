using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Domain.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
namespace PersonnelShiftSystem.Web.Pages.Users
{
    [Authorize(Roles = "Admin")]

    public class UserAddModel : PageModel
    {
        public IBaseModel baseModel;
        private UserService userService;

        [BindProperty]
        public UserDto UserModel { get; set; }
        [BindProperty]
        public List<RoleDto> RoleModel { get; set; }
        [BindProperty]
        public string PasswordAgain { get; set; }
        public UserAddModel(IBaseModel _baseModel, UserService _userService)
        {
            baseModel = _baseModel;
            userService = _userService;
        }

        public async void OnGet()
        {
           var roleModel = await baseModel.BaseUnitOfWork.RoleRepository.GetAllAsync();
            RoleModel = baseModel.Mapper.Map(roleModel, RoleModel);
        }

        public async Task<IActionResult> OnPostAddUser()
        {
            ModelState.Remove("PersonnelCode");
            if (ModelState.IsValid)
            {
               
                var result = await userService.AddUserAsync(UserModel, PasswordAgain);

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
    }
}
