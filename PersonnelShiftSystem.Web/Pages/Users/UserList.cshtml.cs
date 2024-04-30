using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Application.ViewModels.UserView;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Users
{
    [Authorize]

    public class UserListModel : PageModel
    {
        private IBaseModel baseModel;
        private UserService userService;

        public List<UserViewModel> UserModel { get; set; }
        [BindProperty]
        public int RoleId { get; set; }

        public UserListModel(IBaseModel _baseModel, UserService _userService)
        {
            baseModel = _baseModel;
            userService = _userService;
        }
        public void OnGet()
        {
            LoadInitials();
        }

        private async void LoadInitials()
        {
            var siteUsers = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == Convert.ToSByte(baseModel.ItemActive()))).ToList();

            UserModel =  baseModel.Mapper.Map(siteUsers, UserModel);


            UserModel.ForEach(x =>
            {
                var createdName = siteUsers.FirstOrDefault(z => z.Id == x.CreatedBy); x.CreatedName = createdName == null ? string.Empty : $"{createdName.Name} {createdName.Surname}";
                var updatedName = siteUsers.FirstOrDefault(z => z.Id == x.UpdatedBy); x.UpdatedName = updatedName == null ? string.Empty : $"{updatedName.Name} {updatedName.Surname}";
                x.NameSurname = $"{x.Name} {x.Surname}";
            });

            RoleId = Convert.ToInt32(baseModel.ReadFromSession("RoleId"));
        }
        public async Task<IActionResult> OnPostDeleteUser()
        {
            int userId = Convert.ToInt32(HttpContext.Request.Form["Id"]);
            var deleteResult = await userService.DeleteUserAsync(userId);

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
        public IActionResult OnPostAddUser()
        {
            return RedirectToPage("UserAdd");
        }
    }
}
