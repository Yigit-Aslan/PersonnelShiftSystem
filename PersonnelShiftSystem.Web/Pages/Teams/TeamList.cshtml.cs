using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Application.ViewModels.UserView;
using PersonnelShiftSystem.Application.ViewModels.TeamView;

namespace PersonnelShiftSystem.Web.Pages.Teams
{
    public class TeamListModel : PageModel
    {
        private IBaseModel baseModel;

        List<TeamViewModel> TeamModel { get; set; }

        public TeamListModel(IBaseModel _baseModel)
        {
            baseModel = _baseModel;
        }
        public void OnGet()
        {
            LoadInitials();
        }

        private async void LoadInitials()
        {
            var teams = (await baseModel.BaseUnitOfWork.TeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            TeamModel = baseModel.Mapper.Map(teams, TeamModel);
            var siteUsers = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).ToList();
            var getTeamMembers = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            TeamModel.ForEach(x =>
            {
                var createdUser = siteUsers.First(c=>c.Id == x.CreatedBy); x.CreatedName = $"{createdUser.Name} {createdUser.Surname}";
                var updatedUser = siteUsers.First(c => c.Id == x.UpdatedBy); x.UpdatedName = $"{updatedUser.Name} {updatedUser.Surname}";
                var getLeadInfo = getTeamMembers.First(c=>c.TeamId == x.Id && c.IsTeamLeader == baseModel.ItemActive() && c.IsActive == baseModel.ItemActive());
                var getLead = siteUsers.First(c => c.Id == getLeadInfo.UserId && x.IsActive == baseModel.ItemActive()); x.TeamLead = $"{getLead.Name} {getLead.Surname}";
            });
        }
    }
}
