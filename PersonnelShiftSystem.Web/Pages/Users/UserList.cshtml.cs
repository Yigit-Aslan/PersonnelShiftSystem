using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Application.ViewModels.UserView;

namespace PersonnelShiftSystem.Web.Pages.Users
{
    public class UserListModel : PageModel
    {
        private IBaseModel baseModel;

        public List<UserViewModel> UserModel { get; set; }
        public UserListModel(IBaseModel _baseModel)
        {
            baseModel = _baseModel;

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
        }

        public IActionResult OnPostAddUser()
        {
            return RedirectToPage("UserAdd");
        }
    }
}
