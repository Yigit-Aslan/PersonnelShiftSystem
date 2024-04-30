using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Services;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using Wangkanai.Extensions;
using PersonnelShiftSystem.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace PersonnelShiftSystem.Web.Pages.Teams
{
    [Authorize(Roles = "Admin")]

    public class TeamEditModel : PageModel
    {
        private IBaseModel baseModel;
        private readonly TeamService teamService;

        [BindProperty]
        public TeamDto TeamModel { get; set; }
        [BindProperty]
        public PersonnelTeamDto PersonnelTeamModel { get; set; }
        public List<PersonnelTeamDto> PersonnelModel { get; set; }
        public List<Siteuser> LeadUserModel { get; set; }
        public List<Siteuser> PersonnelUserModel { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        [BindProperty]
        public int TeamLeadId { get; set; }
        [BindProperty]
        public List<int> PersonnelIds { get; set; }

        public TeamEditModel(IBaseModel _baseModel, TeamService _teamService)
        {
            baseModel = _baseModel;
            teamService = _teamService;
        }
        public async Task OnGetAsync()
        {
            await LoadInitialsAsync();
        }

        private async Task LoadInitialsAsync()
        {
            TeamModel = new TeamDto();
            PersonnelIds = new List<int>();
            TeamLeadId = new int();

            var teamModel = (await baseModel.BaseUnitOfWork.TeamRepository.GetFirstOrDefaultAsync(x => x.Id == Id && x.IsActive == baseModel.ItemActive()));
            TeamModel = baseModel.Mapper.Map(teamModel, TeamModel);
            LeadUserModel = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            PersonnelUserModel = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            var getPersonnelTeam = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
            var getUserIds = getPersonnelTeam.Select(u => u.UserId).ToHashSet();
            LeadUserModel = LeadUserModel.Where(u => !getUserIds.Contains(u.Id)).ToList();
            PersonnelUserModel = PersonnelUserModel.Where(u => !getUserIds.Contains(u.Id)).ToList();

            TeamLeadId = getPersonnelTeam.First(x => x.IsTeamLeader == baseModel.ItemActive() && x.TeamId == Id).UserId;
            var getLeadData = await baseModel.BaseUnitOfWork.SiteUserRepository.GetFirstOrDefaultAsync(x => x.Id == TeamLeadId && x.IsActive == baseModel.ItemActive());
            LeadUserModel.Add(getLeadData);

            var getPersonnelData = getPersonnelTeam.Where(x => x.IsTeamLeader == baseModel.ItemPassive() && x.TeamId == Id).ToList();
            var getSiteuser = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();

            PersonnelUserModel.AddRange(
                from user in getPersonnelData
                join siteUser in getSiteuser
                    on user.UserId equals siteUser.Id
                where siteUser.IsActive == baseModel.ItemActive()
                select siteUser
            );

            LeadUserModel.AddRange(
                from user in getPersonnelData
                join siteUser in getSiteuser
                    on user.UserId equals siteUser.Id
                where siteUser.IsActive == baseModel.ItemActive()
                select siteUser
            );

            PersonnelIds.AddRange(getPersonnelData.Select(item => item.UserId));


        }

        public async Task<IActionResult> OnGetFilterPersonnel(int teamLeadId)
        {
            try
            {
                List<Siteuser> PersonnelModel = new List<Siteuser>();

                PersonnelModel = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
                var getPersonnelTeam = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
                var getUserIds = getPersonnelTeam.Select(x => x.UserId).Distinct().ToList();

                PersonnelModel = PersonnelModel.Where(x => !getUserIds.Contains(x.Id)).ToList();

                var personnels = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x => x.TeamId == Id && x.IsActive == baseModel.ItemActive())).ToList();
                var getUsers = (await baseModel.BaseUnitOfWork.SiteUserRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).ToList();
                var personnelUsers = from personnel in personnels
                                     join user in getUsers
                                     on personnel.UserId equals user.Id
                                     select user;

                PersonnelModel.AddRange(personnelUsers);

                PersonnelModel.RemoveAll(x => x.Id == teamLeadId);


                return new JsonResult(new { Result = "Success", PersonnelModel = PersonnelModel });
            }
            catch (Exception)
            {
                return new JsonResult(new { Result = "Failed" });

            }
            
        }

        public async Task<IActionResult> OnPostEditTeam()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rData = HttpContext.Request.Form;
                    var personnelIds = rData["PersonnelIds[]"].ToList();
                    var teamLeadId = rData["TeamLeadId"];
                    PersonnelModel = new List<PersonnelTeamDto>();


                    PersonnelTeamModel = new PersonnelTeamDto()
                    {
                        UserId = Convert.ToInt32(teamLeadId),
                        TeamId = Id,
                        IsTeamLeader = baseModel.ItemActive(),
                        IsActive = baseModel.ItemActive(),
                        CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        CreatedDate = DateTime.Now,

                    };
                    PersonnelModel.Add(PersonnelTeamModel);
                    foreach (var personnelId in personnelIds)
                    {
                        PersonnelTeamModel = new PersonnelTeamDto()
                        {
                            UserId = Convert.ToInt32(personnelId),
                            TeamId = Id,
                            IsTeamLeader = baseModel.ItemPassive(),
                            IsActive = baseModel.ItemActive(),
                            CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            CreatedDate = DateTime.Now,

                        };
                        PersonnelModel.Add(PersonnelTeamModel);

                    }
                    TeamModel.Id = Id;
                    var result = await teamService.EditTeamAsync(TeamModel, PersonnelModel, Id);

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
            catch(Exception ex)
            {
                return new JsonResult(new { isSuccess = false, message = "Beklenmedik Sistem Hatası" });

            }
            return new JsonResult(new { isSuccess = false});

        }
        public async Task<IActionResult> OnPostCancel()
        {
            return RedirectToPage("TeamList");
        }
    }
}
