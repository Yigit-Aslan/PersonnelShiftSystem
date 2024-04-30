using Microsoft.AspNetCore.Mvc;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Exceptions;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using PersonnelShiftSystem.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wangkanai.Extensions;

namespace PersonnelShiftSystem.Application.Services
{
    public class AssignShiftTeamService
    {
        private IBaseModel baseModel;
        public Exceptions.ExceptionHandlers ExceptionHandler { get; set; }
        public AssignShiftTeamDto AssignShiftTeamModel { get; set; }
        public AssignShiftTeamService(IBaseModel _baseModel, Exceptions.ExceptionHandlers exceptionHandler)
        {
            baseModel = _baseModel;
            ExceptionHandler = exceptionHandler;

        }


        public async Task<IActionResult> AddAssignTeamAsync(List<TeamDto> TeamModel, int shiftId)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {

                    string dtosCheck = await AssignShiftTeamValidation(TeamModel, shiftId);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/AssignShiftTeams/AssignShiftTeamAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }



                    bool assignedTeamCheck = await AssignedTeamCheck(TeamModel);

                    if (assignedTeamCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Bir takım birden fazla vardiya da bulunamaz",
                            CreatedDate = DateTime.Now,
                            Path = "/AssignShiftTeams/AssignShiftTeamAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }


                    foreach (var item in TeamModel)
                    {

                        AssignShiftTeamModel = new AssignShiftTeamDto()
                        {
                            TeamId = item.Id,
                            ShiftId = shiftId,
                            IsActive = baseModel.ItemActive(),
                            CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            CreatedDate = DateTime.Now,
                        };
                        var mappedAssign = baseModel.Mapper.Map<AssignShiftTeam>(AssignShiftTeamModel);
                        await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.AddAsync(mappedAssign);

                    }

                    await baseModel.BaseUnitOfWork.SaveChangesAsync();
                    baseModel.BaseUnitOfWork.CommitTransaction();
                    return new OkObjectResult(new { isSuccess = true, message = "Takıma Vardiya Başarıyla Atandı", url = "/AssignShiftTeams/AssignShiftTeamList" });

                }
                catch (Exception ex)
                {
                    ErrorLog log = new ErrorLog()
                    {
                        UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Error),
                        ExMessage = ExceptionMessageHelper.UnexpectedSystemError,
                        CreatedDate = DateTime.Now,
                        Path = "/AssignShiftTeams/AssignShiftTeamAdd"

                    };
                    baseModel.BaseUnitOfWork.RollbackTransaction();

                    await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.UnexpectedSystemError });
                }
            }
        }

        public async Task<IActionResult> EditAssignTeamAsync(AssignShiftTeamDto AssignShiftTeamModel)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    string dtosCheck = await AssignShiftTeamEditValidation(AssignShiftTeamModel);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/AssignShiftTeams/AssignShiftTeamEdit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    var assignShiftTeam = await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.GetFirstOrDefaultAsync(x => x.Id == AssignShiftTeamModel.Id && x.IsActive == baseModel.ItemActive());

                    if (assignShiftTeam != null)
                    {
                        var existingAssignShiftTeam = await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.GetFirstOrDefaultAsync(x => x.Id == assignShiftTeam.Id && x.IsActive == baseModel.ItemActive());
                        existingAssignShiftTeam.ShiftId = AssignShiftTeamModel.ShiftId;
                        existingAssignShiftTeam.TeamId = AssignShiftTeamModel.TeamId;
                        existingAssignShiftTeam.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                        existingAssignShiftTeam.UpdatedDate = DateTime.Now;

                        var mappedData = baseModel.Mapper.Map<AssignShiftTeam>(existingAssignShiftTeam);
                        await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.UpdateAsync(mappedData);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    }
                    else
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Geçersiz Vardiya ve Takım",
                            CreatedDate = DateTime.Now,
                            Path = "/AssignShiftTeams/AssignShiftTeamEdit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    return new BadRequestObjectResult(new { isSuccess = true, message ="Vardiyaya takım ekleme işlemi başarıyla gerçekleştirildi.", url = "/AssignShiftTeams/AssignShiftTeamList" });
                }
                catch (Exception ex)
                {
                    ErrorLog log = new ErrorLog()
                    {
                        UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Error),
                        ExMessage = ExceptionMessageHelper.UnexpectedSystemError,
                        CreatedDate = DateTime.Now,
                        Path = "/AssignShiftTeams/AssignShiftTeamAdd"

                    };
                    baseModel.BaseUnitOfWork.RollbackTransaction();

                    await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.UnexpectedSystemError });
                }



            }

        }


        public async Task<IActionResult> DeleteAssignAsync(int assignedId)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {

                try
                {
                    var getAssignedTeam = await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.GetFirstOrDefaultAsync(x => x.Id == assignedId && x.IsActive == baseModel.ItemActive());
                    if (getAssignedTeam != null)
                    {
                        var existingAssignedTeam = await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.GetFirstOrDefaultAsync(x => x.Id == getAssignedTeam.Id && x.IsActive == baseModel.ItemActive());
                        existingAssignedTeam.IsActive = baseModel.ItemPassive();
                        existingAssignedTeam.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                        existingAssignedTeam.UpdatedDate = DateTime.Now;

                        await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.UpdateAsync(existingAssignedTeam);

                    }

                    await baseModel.BaseUnitOfWork.SaveChangesAsync();
                    baseModel.BaseUnitOfWork.CommitTransaction();

                    return new OkObjectResult(new { isSuccess = true });

                }
                catch (Exception ex)
                {
                    ErrorLog log = new ErrorLog()
                    {
                        UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                        ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Error),
                        ExMessage = ExceptionMessageHelper.UnexpectedSystemError,
                        CreatedDate = DateTime.Now,
                        Path = "/ShiftList"

                    };
                    baseModel.BaseUnitOfWork.RollbackTransaction();

                    await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.UnexpectedSystemError });
                }
            }

            
        }

        private async Task<string> AssignShiftTeamValidation(List<TeamDto> teamModel, int shiftId)
        {
            if (teamModel.Count <= 0)
                return ExceptionMessageHelper.CannotEmptyField("Takım");
            if (shiftId <= 0)
                return ExceptionMessageHelper.CannotEmptyField("Vardiya");


            return string.Empty;
        }

        private async Task<string> AssignShiftTeamEditValidation(AssignShiftTeamDto assignShiftTeamModel)
        {
            if (assignShiftTeamModel.TeamId <= 0)
                return ExceptionMessageHelper.CannotEmptyField("Takım");
            if (assignShiftTeamModel.ShiftId <= 0)
                return ExceptionMessageHelper.CannotEmptyField("Vardiya");


            return string.Empty;
        }


        private async Task<bool> AssignedTeamCheck(List<TeamDto> teamModel)
        {
            // Personel kimliklerini al
            var teams = (await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.QueryAsync(x => x.IsActive == baseModel.ItemActive())).Select(x => x.TeamId).ToList();

            // Personel kimliklerini PersonelTeamDto'ya dönüştür
            var mappedTeams = teams.Select(teamId => new PersonnelTeamDto { TeamId = teamId }).ToList();

            // Her personel modelini kontrol et
            foreach (var team in teamModel)
            {
                // personnelModel listede var mı kontrol et
                var check = mappedTeams.Any(x => x.TeamId == team.Id);

                if (check)
                {
                    return true; // Eğer listede varsa true dön
                }
            }

            return false; // Hiçbiri listede değilse false dön
        }
    }
}