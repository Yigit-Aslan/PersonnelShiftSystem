using Microsoft.AspNetCore.Mvc;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Exceptions;
using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wangkanai.Extensions;

namespace PersonnelShiftSystem.Application.Services
{
    public class ShiftService
    {
        private IBaseModel baseModel;
        public Exceptions.ExceptionHandlers ExceptionHandler { get; set; }
        public PersonnelTeamDto PersonnelTeamModel { get; set; }
        public ShiftService(IBaseModel _baseModel, Exceptions.ExceptionHandlers exceptionHandler)
        {
            baseModel = _baseModel;
            ExceptionHandler = exceptionHandler;

        }

        public async Task<IActionResult> AddShiftAsync(ShiftDto ShiftModel)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    string dtosCheck = await ShiftModelValidation(ShiftModel);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/ShiftList/ShiftAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }
                    bool shiftCheck = await ShiftCheck(ShiftModel);

                    if (shiftCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Kayıtlı Takım",
                            CreatedDate = DateTime.Now,
                            Path = "/ShiftList/ShiftAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    var shift = await ConvertShiftData(ShiftModel);

                    if (shift == null)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "İşlem sırasında hata oluştu",
                            CreatedDate = DateTime.Now,
                            Path = "/ShiftList/ShiftAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    await baseModel.BaseUnitOfWork.ShiftRepository.AddAsync(shift);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();
                    baseModel.BaseUnitOfWork.CommitTransaction();

                    return new OkObjectResult(new { isSuccess = true, message = "Vardiya Başarıyla Oluşturuldu", url = "/Shifts/ShiftList" });
                }
                catch(Exception Ex)
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

        public async Task<IActionResult> EditShiftAsync(ShiftDto ShiftModel)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    string dtosCheck = await ShiftModelValidation(ShiftModel);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/ShiftList/Edit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool shiftCheck = await ShiftEditCheck(ShiftModel);

                    if (shiftCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Kayıtlı Takım",
                            CreatedDate = DateTime.Now,
                            Path = "/ShiftList/ShiftEdit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    var existingShift = await ConvertShiftEditData(ShiftModel);

                    if (existingShift == null)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "İşlem sırasında hata oluştu",
                            CreatedDate = DateTime.Now,
                            Path = "/ShiftList/ShiftEdit"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    await baseModel.BaseUnitOfWork.ShiftRepository.UpdateAsync(existingShift);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();
                    baseModel.BaseUnitOfWork.CommitTransaction();
                    return new OkObjectResult(new { isSuccess = true, message = "Vardiya Başarıyla Düzenlendi", url = "/Shifts/ShiftList" });
                }


                catch(Exception ex)
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

        public async Task<IActionResult> DeleteShiftAsync(int shiftId)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {

                    var getAssingedTeams = (await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.QueryAsync(x => x.ShiftId == shiftId && x.IsActive == baseModel.ItemActive())).ToList();
                    if (getAssingedTeams.Count > 0)
                    {
                        foreach (var item in getAssingedTeams)
                        {
                            item.IsActive = baseModel.ItemPassive();
                            item.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                            item.UpdatedDate = DateTime.Now;

                            await baseModel.BaseUnitOfWork.AssignShiftTeamRepository.UpdateAsync(item);
                        }
                    }

                    var getShift = await baseModel.BaseUnitOfWork.ShiftRepository.GetFirstOrDefaultAsync(x => x.Id == shiftId && x.IsActive == baseModel.ItemActive());
                    if (getShift != null)
                    {
                        var existingShift = await baseModel.BaseUnitOfWork.ShiftRepository.GetFirstOrDefaultAsync(x => x.Id == getShift.Id && x.IsActive == baseModel.ItemActive());
                        existingShift.IsActive = baseModel.ItemPassive();
                        existingShift.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                        existingShift.UpdatedDate = DateTime.Now;

                        await baseModel.BaseUnitOfWork.ShiftRepository.UpdateAsync(existingShift);

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

        private async Task<string> ShiftModelValidation(ShiftDto ShiftModel)
        {
            if (ShiftModel.ShiftName.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Vardiya Adı");
            if (ShiftModel.StartingDate.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Vardiya Başlangıç Tarihi");
            if (ShiftModel.EndingDate.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Vardiya Bitiş Tarihi");
            if (ShiftModel.ShiftStart.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Vardiya Başlangıç Saati");
            if (ShiftModel.ShiftEnd.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Vardiya Bitiş Saati");


            if (DateTime.ParseExact(ShiftModel.StartingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) < DateTime.Now)
                return "Vardiya başlangıç tarihi bugünden önce olamaz.";
            if (DateTime.ParseExact(ShiftModel.EndingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) < DateTime.Now)
                return "Vardiya bitiş tarihi bugünden önce olamaz.";
            if (DateTime.ParseExact(ShiftModel.EndingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) < DateTime.ParseExact(ShiftModel.StartingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture))
                return "Vardiya bitiş tarihi, vardiya başlangıç tarihinden önce olamaz";

            return string.Empty;
        }
        private async Task<bool> ShiftCheck(ShiftDto ShiftModel)
        {
            var shiftCheck = await baseModel.BaseUnitOfWork.ShiftRepository.GetFirstOrDefaultAsync(x => x.ShiftName.ToLower().Trim() == ShiftModel.ShiftName.ToLower().Trim() &&
                                                                                              x.IsActive == baseModel.ItemActive());
            if (shiftCheck != null)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> ShiftEditCheck(ShiftDto ShiftModel)
        {
            DateTime startingDate = DateTime.ParseExact(ShiftModel.StartingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime endingDate = DateTime.ParseExact(ShiftModel.EndingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            TimeSpan shiftStart = TimeSpan.Parse(ShiftModel.ShiftStart);
            TimeSpan shiftEnd = TimeSpan.Parse(ShiftModel.ShiftEnd);

            var shiftCheck = await baseModel.BaseUnitOfWork.ShiftRepository.GetFirstOrDefaultAsync(x => x.ShiftName.ToLower().Trim() == ShiftModel.ShiftName.ToLower().Trim() &&
                                                                                                        x.StartingDate == startingDate &&
                                                                                                        x.EndingDate == endingDate &&
                                                                                                        x.ShiftStart == shiftStart &&
                                                                                                        x.ShiftEnd == shiftEnd &&
                                                                                                        x.IsActive == baseModel.ItemActive());
            if (shiftCheck != null)
            {
                return true;
            }

            return false;
        }

        private async Task<Shift> ConvertShiftData(ShiftDto ShiftModel)
        {
            var mappedShift = baseModel.Mapper.Map<Shift>(ShiftModel);
            if (mappedShift != null)
            {
                mappedShift.StartingDate = DateTime.ParseExact(ShiftModel.StartingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                mappedShift.EndingDate = DateTime.ParseExact(ShiftModel.EndingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                mappedShift.ShiftStart = TimeSpan.ParseExact(ShiftModel.ShiftStart, "hh\\:mm", CultureInfo.InvariantCulture);
                mappedShift.ShiftEnd = TimeSpan.ParseExact(ShiftModel.ShiftEnd, "hh\\:mm", CultureInfo.InvariantCulture);
                mappedShift.CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                mappedShift.CreatedDate = DateTime.Now;
                mappedShift.IsActive = baseModel.ItemActive();

                return mappedShift;

            }
            mappedShift = null;
            return mappedShift;
        }

        private async Task<Shift> ConvertShiftEditData(ShiftDto ShiftModel)
        {
            var existingShift = await baseModel.BaseUnitOfWork.ShiftRepository.GetFirstOrDefaultAsync(x=>x.Id == ShiftModel.Id && x.IsActive == baseModel.ItemActive());
            if (existingShift != null)
            {
                existingShift.ShiftName = ShiftModel.ShiftName;
                existingShift.StartingDate = DateTime.ParseExact(ShiftModel.StartingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                existingShift.EndingDate = DateTime.ParseExact(ShiftModel.EndingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                existingShift.ShiftStart = TimeSpan.ParseExact(ShiftModel.ShiftStart, "hh\\:mm", CultureInfo.InvariantCulture);
                existingShift.ShiftEnd = TimeSpan.ParseExact(ShiftModel.ShiftEnd, "hh\\:mm", CultureInfo.InvariantCulture);
                existingShift.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                existingShift.UpdatedDate = DateTime.Now;
                existingShift.IsActive = baseModel.ItemActive();

                return existingShift;

            }
            existingShift = null;
            return existingShift;
        }

    }
}
