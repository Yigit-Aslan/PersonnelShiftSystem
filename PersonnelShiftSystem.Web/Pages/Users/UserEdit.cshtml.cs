using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Users
{
    [Authorize(Roles = "Admin")]

    public class UserEditModel : PageModel
    {
        public IBaseModel baseModel;
        private UserService userService;

        [BindProperty]
        public UserDto UserModel { get; set; }
        [BindProperty]
        public List<RoleDto> RoleModel { get; set; }
        [BindProperty]
        public string PasswordAgain { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public UserEditModel(IBaseModel _baseModel, UserService _userService)
        {
            baseModel = _baseModel;
            userService = _userService;
        }

        public async Task OnGet()
        {
            await baseModel.SaveVisitorInfo();

            var roleModel = await baseModel.BaseUnitOfWork.RoleRepository.GetAllAsync();
            RoleModel = baseModel.Mapper.Map(roleModel, RoleModel);
        }

        public async Task<IActionResult> OnPostEditUser()
        {
            ModelState.Remove("PersonnelCode");
            if (ModelState.IsValid)
            {
                UserModel.Id = Id;
                var result = await userService.EditUserAsync(UserModel, PasswordAgain);

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
            return await Task.FromResult(RedirectToPage("UserList"));
        }
    }
}

