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
                            Path = "TeamAdd"

                        };
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
                            Path = "TeamAdd"

                        };
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
                            Path = "TeamAdd"

                        };
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
                        Path = "TeamAdd"

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
    }
}
