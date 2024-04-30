using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonnelShiftSystem.Application.Dtos;
using PersonnelShiftSystem.Application.Exceptions;
using PersonnelShiftSystem.Application.ViewModels.UserView;
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
    public class UserService
    {
        private IBaseModel baseModel;
        private ExceptionHandlers exceptionHandlers;
        public UserService(IBaseModel _baseModel, ExceptionHandlers _exceptionHandlers)
        {
            baseModel = _baseModel;
            exceptionHandlers = _exceptionHandlers;

        }
        public async Task<IActionResult> AddUserAsync(UserDto UserModel, string PasswordAgain)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    string dtosCheck = UserValidation(UserModel, PasswordAgain);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool mailValidCheck = exceptionHandlers.ValidMail(UserModel.MailAddress);

                    if (mailValidCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Yanlış Mail Adres Formatı",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool mailCheck = exceptionHandlers.CheckMail(UserModel.MailAddress);
                    if (mailCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Kayıtlı Mail Adresi",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    string phoneNumber = System.Text.RegularExpressions.Regex.Replace(UserModel.PhoneNumber, @"[^\d]", "");

                    if ((exceptionHandlers.ValidPhone(phoneNumber)))
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = ExceptionMessageHelper.AlreadyTakenPhone,
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new JsonResult(new { isSuccess = false, message = ExceptionMessageHelper.AlreadyTakenPhone });
                    }

                    if (UserModel.Password != PasswordAgain)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Girilen Şifreler Eşleşmiyor",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    if (!(SiteUserRepository.ValidatePassword(UserModel.Password)))
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Girilen Şifre Standartlara Uymuyor.",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }
                    UserModel.PhoneNumber = phoneNumber;
                    UserModel.PersonnelCode = SiteUserRepository.GeneratePersonnelCode();
                    UserModel.Password = SiteUserRepository.EncryptPassword(UserModel.Password);
                    UserModel.IsActive = baseModel.ItemActive();
                    UserModel.CreatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                    UserModel.CreatedDate = DateTime.Now;
                    var mappedUser = baseModel.Mapper.Map<Siteuser>(UserModel);
                    await baseModel.BaseUnitOfWork.SiteUserRepository.AddAsync(mappedUser);
                    await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    baseModel.BaseUnitOfWork.CommitTransaction();

                    return new OkObjectResult(new { isSuccess = true });
                }
                catch (Exception Ex)
                {

                    throw;
                }
                
            }
        }
        public async Task<IActionResult> EditUserAsync(UserDto UserModel, string PasswordAgain)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    string dtosCheck = UserValidation(UserModel, PasswordAgain);

                    if (dtosCheck.IsNotNullOrEmpty())
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = dtosCheck,
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool mailValidCheck = exceptionHandlers.ValidMail(UserModel.MailAddress);

                    if (mailValidCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Yanlış Mail Adres Formatı",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    bool mailCheck = exceptionHandlers.CheckMail(UserModel.MailAddress);
                    if (mailCheck)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Kayıtlı Mail Adresi",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    string phoneNumber = System.Text.RegularExpressions.Regex.Replace(UserModel.PhoneNumber, @"[^\d]", "");

                    if ((exceptionHandlers.ValidPhone(phoneNumber)))
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = ExceptionMessageHelper.AlreadyTakenPhone,
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new JsonResult(new { isSuccess = false, message = ExceptionMessageHelper.AlreadyTakenPhone });
                    }

                    if (UserModel.Password != PasswordAgain)
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Girilen Şifreler Eşleşmiyor",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    if (!(SiteUserRepository.ValidatePassword(UserModel.Password)))
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = Convert.ToInt32(baseModel.ReadFromSession("UserId")),
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = "Girilen Şifre Standartlara Uymuyor.",
                            CreatedDate = DateTime.Now,
                            Path = "/Users/UserAdd"

                        };
                        baseModel.BaseUnitOfWork.RollbackTransaction();

                        await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();
                        return new BadRequestObjectResult(new { isSuccess = false, message = log.ExMessage });
                    }

                    var existingUser = await baseModel.BaseUnitOfWork.SiteUserRepository.GetFirstOrDefaultAsync(x=>x.Id == UserModel.Id && x.IsActive == baseModel.ItemActive());
                    if(existingUser != null)
                    {
                        var existingUserData = await baseModel.BaseUnitOfWork.SiteUserRepository.GetFirstOrDefaultAsync(x => x.Id == UserModel.Id && x.IsActive == baseModel.ItemActive());
                        existingUserData.Name = UserModel.Name;
                        existingUserData.Surname = UserModel.Surname;
                        existingUserData.PhoneNumber = phoneNumber;
                        existingUserData.MailAddress = UserModel.MailAddress;
                        existingUserData.Password = SiteUserRepository.EncryptPassword(UserModel.Password);
                        existingUserData.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                        existingUserData.UpdatedDate = DateTime.Now;

                        await baseModel.BaseUnitOfWork.SiteUserRepository.UpdateAsync(existingUserData);
                        await baseModel.BaseUnitOfWork.SaveChangesAsync();

                        baseModel.BaseUnitOfWork.CommitTransaction();

                        return new OkObjectResult(new { isSuccess = true, message = "Personel Başarıyla Oluşturuldu", url = "/Users/UserList" });

                    }

                    return new BadRequestObjectResult(new { isSuccess = false,});

                }
                catch (Exception Ex)
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

        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            using (var transaction = baseModel.BaseUnitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var personnelTeam = await baseModel.BaseUnitOfWork.PersonnelTeamRepository.GetFirstOrDefaultAsync(x => x.UserId == userId && x.IsActive == baseModel.ItemActive());
                    if (personnelTeam != null)
                    {
                        var existingPersonnelTeam = await baseModel.BaseUnitOfWork.PersonnelTeamRepository.GetFirstOrDefaultAsync(x => x.UserId == personnelTeam.UserId && x.IsActive == baseModel.ItemActive());
                        existingPersonnelTeam.IsActive = baseModel.ItemPassive();
                        existingPersonnelTeam.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                        existingPersonnelTeam.UpdatedDate = DateTime.Now;

                        await baseModel.BaseUnitOfWork.PersonnelTeamRepository.UpdateAsync(existingPersonnelTeam);

                    }

                    var user = await baseModel.BaseUnitOfWork.SiteUserRepository.GetFirstOrDefaultAsync(x => x.Id == userId && x.IsActive == baseModel.ItemActive());
                    if (user != null)
                    {
                        var existingUser = await baseModel.BaseUnitOfWork.SiteUserRepository.GetFirstOrDefaultAsync(x => x.Id == user.Id && x.IsActive == baseModel.ItemActive());
                        existingUser.IsActive = baseModel.ItemPassive();
                        existingUser.UpdatedBy = Convert.ToInt32(baseModel.ReadFromSession("UserId"));
                        existingUser.UpdatedDate = DateTime.Now;

                        await baseModel.BaseUnitOfWork.SiteUserRepository.UpdateAsync(existingUser);

                        await baseModel.BaseUnitOfWork.SaveChangesAsync();


                    }
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
        private string UserValidation(UserDto UserModel, string PasswordAgain)
        {
            if (UserModel.Name.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Ad");
            if (UserModel.Surname.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Soyad");
            if (UserModel.MailAddress.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Email");
            if (UserModel.Password.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Şifre");
            if (UserModel.Password.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Şifre Tekrarı");
            if (UserModel.PhoneNumber.IsNullOrEmpty())
                return ExceptionMessageHelper.CannotEmptyField("Telefon Numarası");
            if(UserModel.RoleId <= 0)
                return ExceptionMessageHelper.CannotEmptyField("Rol");


            return string.Empty;
        }
    }
}
