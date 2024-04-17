using Microsoft.AspNetCore.Mvc;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Exceptions;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using PersonnelShiftSystem.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Wangkanai.Extensions;

namespace PersonnelShiftSystem.Application.Services
{
    public class TeamService
    {
        private IBaseModel baseModel;
        public Exceptions.ExceptionHandlers ExceptionHandler { get; set; }
        public PersonnelTeamDto PersonnelTeamModel { get; set; }
        public TeamService(IBaseModel _baseModel, Exceptions.ExceptionHandlers exceptionHandler)
        {
            baseModel = _baseModel;
            ExceptionHandler = exceptionHandler;

        }

        public async Task<IActionResult> AddTeamAsync(TeamDto teamModel, List<PersonnelTeamDto> personnelTeamModel)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var getTeamLead = personnelTeamModel.First(x => x.IsTeamLeader == baseModel.ItemActive() && x.IsActive == baseModel.ItemActive());
                    var personnelCheck = personnelTeamModel.Where(x => x.IsTeamLeader == baseModel.ItemPassive()).ToList();
                    string dtosCheck = await TeamModelValidation(teamModel, getTeamLead, personnelCheck);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/TeamList/TeamAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool teamCheck = await TeamCheck(teamModel);

                    if (teamCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Kayıtlı Takım",
                            CreatedDate = DateTime.Now,
                            Path = "/TeamList/TeamAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool personnelTeamCheck = await PersonnelTeamCheck(personnelTeamModel);

                    if (personnelTeamCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Bir personel birden fazla takımda bulunamaz",
                            CreatedDate = DateTime.Now,
                            Path = "/TeamList/TeamAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }
                    teamModel.IsActive = baseModel.ItemActive();
                    teamModel.CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                    teamModel.CreatedDate = DateTime.Now;

                    var mappedTeam = baseModel.Mapper.Map<Team>(teamModel);
                    await baseModel.BaseUnitOfWork.TeamRepository.AddAsync(mappedTeam);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    var teamId = mappedTeam.Id;

                    foreach (var item in personnelTeamModel)
                    {
                        if(item.IsTeamLeader == baseModel.ItemActive())
                        {
                            PersonnelTeamModel = new PersonnelTeamDto()
                            {
                                TeamId = teamId,
                                UserId = Convert.ToInt32(item.UserId),
                                IsTeamLeader = baseModel.ItemActive(),
                                IsActive = baseModel.ItemActive(),
                                CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                                CreatedDate = DateTime.Now,
                            };
                            var mappedPersonnel = baseModel.Mapper.Map<PersonnelTeam>(PersonnelTeamModel);
                            await baseModel.BaseUnitOfWork.PersonnelTeamRepository.AddAsync(mappedPersonnel);

                        }
                        else
                        {
                            PersonnelTeamModel = new PersonnelTeamDto()
                            {
                                TeamId = teamId,
                                UserId = Convert.ToInt32(item.UserId),
                                IsTeamLeader = baseModel.ItemPassive(),
                                IsActive = baseModel.ItemActive(),
                                CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                                CreatedDate = DateTime.Now,
                            };
                            var mappedPersonnel = baseModel.Mapper.Map<PersonnelTeam>(PersonnelTeamModel);
                            await baseModel.BaseUnitOfWork.PersonnelTeamRepository.AddAsync(mappedPersonnel);
                        }
                        
                    }

                    await baseModel.BaseUnitOfWork.SaveChangesAsync();
                    baseModel.BaseUnitOfWork.CommitTransaction();
                    return new OkObjectResult(new { isSuccess = true, message = "Takım Başarıyla Oluşturuldu", url = "TeamList" });

                }
                catch (Exception)
                {
                    ErrorLog log = new ErrorLog()
                    {
                        UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Error),
                        ExMessage = ExceptionMessageHelper.UnexpectedSystemError,
                        CreatedDate = DateTime.Now,
                        Path = "/TeamList/TeamAdd"

                    };
                    baseModel.BaseUnitOfWork.RollbackTransaction();

                    await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.UnexpectedSystemError });
                }
            }
        }

        public async Task<IActionResult> OnPostEditTeam(TeamDto teamModel, List<PersonnelTeamDto> personnelTeamModel)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var getTeamLead = personnelTeamModel.First(x => x.IsTeamLeader == baseModel.ItemActive() && x.IsActive == baseModel.ItemActive());
                    var personnelCheck = personnelTeamModel.Where(x => x.IsTeamLeader == baseModel.ItemPassive()).ToList();
                    string dtosCheck = await TeamModelValidation(teamModel, getTeamLead, personnelCheck);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/TeamList/TeamEdit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool teamCheck = await TeamEditCheck(teamModel);

                    if (teamCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Kayıtlı Takım",
                            CreatedDate = DateTime.Now,
                            Path = "/TeamList/TeamEdit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool teamPersonnelCheck = await EditPersonnelTeam(personnelTeamModel);
                    if (!teamPersonnelCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Personellerin Eklenmesi Sırasında sorun oluştu",
                            CreatedDate = DateTime.Now,
                            Path = "/TeamList/TeamEdit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    await baseModel.BaseUnitOfWork.SaveChangesAsync();
                    baseModel.BaseUnitOfWork.CommitTransaction();

                    return new BadRequestObjectResult(new { isSuccess = true, message = "Takım başarıyla düzenlendi", url = "TeamList" });

                }
                catch (Exception)
                {
                    ErrorLog log = new ErrorLog()
                    {
                        UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Error),
                        ExMessage = ExceptionMessageHelper.UnexpectedSystemError,
                        CreatedDate = DateTime.Now,
                        Path = "/TeamList/TeamEdit"

                    };
                    baseModel.BaseUnitOfWork.RollbackTransaction();

                    await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.UnexpectedSystemError });
                }
            }
        }


        private async Task<string> TeamModelValidation(TeamDto teamModel, PersonnelTeamDto personnelLead, List<PersonnelTeamDto> personnelTeam)
        {
            if (teamModel.TeamName.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Takım Adı");
            if (personnelLead.UserId <= 0)
                return ExceptionMessageHelper.CannotEmptyField("Takım Lideri");
            if(personnelTeam.Count <= 0)
                return ExceptionMessageHelper.CannotEmptyField("Takım Üyeleri");

            return string.Empty;
        }

        private async Task<bool> TeamCheck(TeamDto teamModel)
        {
            var teamCheck = await baseModel.BaseUnitOfWork.TeamRepository.GetFirstOrDefaultAsync(x=>x.TeamName.ToLower().Trim() == teamModel.TeamName.ToLower().Trim() && 
                                                                                              x.IsActive == baseModel.ItemActive());
            if (teamCheck != null)
            {
                return true;
            }

            return false;
        }
        private async Task<bool> TeamEditCheck(TeamDto teamModel)
        {
            var teamCheck = await baseModel.BaseUnitOfWork.TeamRepository.GetFirstOrDefaultAsync(x=>x.Id == teamModel.Id && 
                                                                                                    x.TeamName.ToLower().Trim() == teamModel.TeamName.ToLower().Trim());

            var teamNameCheck = await baseModel.BaseUnitOfWork.TeamRepository.GetFirstOrDefaultAsync(x => x.TeamName.ToLower().Trim() == teamModel.TeamName.ToLower().Trim());

            if (teamCheck != null)
            {
                return false;
            }
            else if(teamNameCheck != null)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> PersonnelTeamCheck(List<PersonnelTeamDto> personnelTeamModel)
        {
            var personnels = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x=>x.IsActive == baseModel.ItemActive())).Select(x=>x.UserId).ToList();

            foreach (var personnel in personnels)
            {
                var check = personnels.Contains(personnel);

                if(check)
                {
                    return true;
                }

            }
            return false;
        }

        private async Task<bool> EditPersonnelTeam(List<PersonnelTeamDto> personnelTeamModel)
        {
            if(personnelTeamModel.Count <= 0)
            {
                List<PersonnelTeamDto> personnelTeam = new List<PersonnelTeamDto>();
                var getTeamId = personnelTeamModel.First().TeamId;

                var getPersonnels = (await baseModel.BaseUnitOfWork.PersonnelTeamRepository.QueryAsync(x => x.TeamId == getTeamId && x.IsActive == baseModel.ItemActive())).ToList();
                List<PersonnelTeamDto> personnelTeamDto = new List<PersonnelTeamDto>();
                personnelTeamDto = baseModel.Mapper.Map(getPersonnels, personnelTeamDto);

                var getOldTeamLead = getPersonnels.First(x => x.IsTeamLeader == baseModel.ItemActive());
                var getNewTeamLead = personnelTeamModel.First(x => x.IsTeamLeader == baseModel.ItemActive());

                if (getOldTeamLead.UserId != getNewTeamLead.UserId)
                {

                    var teamLeader = getPersonnels.First(x => x.IsTeamLeader == baseModel.ItemActive());
                    teamLeader.IsActive = baseModel.ItemPassive();
                    teamLeader.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                    teamLeader.UpdatedDate = DateTime.Now;

                    await baseModel.BaseUnitOfWork.PersonnelTeamRepository.UpdateAsync(teamLeader);



                    PersonnelTeamModel = new PersonnelTeamDto()
                    {
                        TeamId = getTeamId,
                        UserId = Convert.ToInt32(getNewTeamLead.UserId),
                        IsTeamLeader = baseModel.ItemActive(),
                        IsActive = baseModel.ItemActive(),
                        CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        CreatedDate = DateTime.Now,
                    };
                    var mappedPersonnel = baseModel.Mapper.Map<PersonnelTeam>(PersonnelTeamModel);
                    await baseModel.BaseUnitOfWork.PersonnelTeamRepository.AddAsync(mappedPersonnel);

                }

                personnelTeamModel.RemoveAll(x => x.IsTeamLeader == baseModel.ItemActive());
                personnelTeamDto.RemoveAll(x => x.IsTeamLeader == baseModel.ItemActive());

                var passivePersonnels = personnelTeamDto.Except(personnelTeamModel);

                if (passivePersonnels != null)
                {
                    foreach (var item in passivePersonnels)
                    {
                        var getExistingPersonnels = personnelTeamDto.First(x => x.UserId == item.UserId);
                        getExistingPersonnels.IsActive = baseModel.ItemPassive();
                        getExistingPersonnels.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                        getExistingPersonnels.UpdatedDate = DateTime.Now;
                        var mappedPersonnels = baseModel.Mapper.Map<PersonnelTeam>(getExistingPersonnels);
                        await baseModel.BaseUnitOfWork.PersonnelTeamRepository.UpdateAsync(mappedPersonnels);
                    }
                }

                var newPersonnels = personnelTeamDto.Except(personnelTeamModel);
                if(newPersonnels != null)
                {
                    foreach (var item in newPersonnels)
                    {
                        PersonnelTeamModel = new PersonnelTeamDto()
                        {
                            TeamId = getTeamId,
                            UserId = Convert.ToInt32(item.UserId),
                            IsTeamLeader = baseModel.ItemPassive(),
                            IsActive = baseModel.ItemActive(),
                            CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            CreatedDate = DateTime.Now,
                        };
                        var mappedPersonnel = baseModel.Mapper.Map<PersonnelTeam>(PersonnelTeamModel);
                        await baseModel.BaseUnitOfWork.PersonnelTeamRepository.AddAsync(mappedPersonnel);

                    }
                }

                return true;
            }

            return false;
            
        }
        
    }
}
