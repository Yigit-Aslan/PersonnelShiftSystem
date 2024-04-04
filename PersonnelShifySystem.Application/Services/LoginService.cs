using Microsoft.AspNetCore.Http;
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

namespace PersonnelShiftSystem.Application.Services
{
    public class LoginService
    {
        private IBaseModel baseModel;
        public Exceptions.ExceptionHandlers ExceptionHandler { get; set; }
        public LoginService(IBaseModel _baseModel, Exceptions.ExceptionHandlers exceptionHandler)
        {
            baseModel = _baseModel;
            ExceptionHandler = exceptionHandler;

        }

        public async Task<IActionResult> LoginAsync(LoginDto model)
        {
            try
            {
                string hashedPassword = string.Empty;
                UserLoginHistory userloginhistory = new UserLoginHistory();


                if (!(ExceptionHandler.ValidMail(model.MailAddress)))
                {
                    ErrorLog log = new ErrorLog()
                    {
                        UserId = 0,
                        ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                        ExMessage = ExceptionMessageHelper.EmailNotValid,
                        CreatedDate = DateTime.Now,
                        Path = "Login"
                    };
                    userloginhistory.MailAddress = model.MailAddress;
                    userloginhistory.CreatedDate = DateTime.Now;
                    userloginhistory.LoginResult = "Failed";

                   await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                   await baseModel.BaseUnitOfWork.UserLoginHistoryRepository.AddAsync(userloginhistory);
                   await baseModel.BaseUnitOfWork.SaveChangesAsync();
                    return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.EmailNotValid });

                }

                var currentUser = await baseModel.BaseUnitOfWork.SiteUserRepository.GetFirstOrDefaultAsync(x => x.MailAddress == model.MailAddress);

                if (currentUser != null)
                {
                    if ((!(SiteUserRepository.VerifyPassword(model.Password, currentUser.Password))))
                    {
                        ErrorLog log = new ErrorLog()
                        {
                            UserId = 0,
                            ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                            ExMessage = ExceptionMessageHelper.LoginError,
                            CreatedDate = DateTime.Now,
                            Path = "Login"

                        };

                        userloginhistory.MailAddress = model.MailAddress;
                        userloginhistory.CreatedDate = DateTime.Now;
                        userloginhistory.LoginResult = "Failed";

                       await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                       await baseModel.BaseUnitOfWork.UserLoginHistoryRepository.AddAsync(userloginhistory);
                       await baseModel.BaseUnitOfWork.SaveChangesAsync();

                        return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.LoginError });
                    }

                }
                else
                {
                    ErrorLog log = new ErrorLog()
                    {
                        UserId = 0,
                        ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Warning),
                        ExMessage = ExceptionMessageHelper.UserEmailNotFound,
                        CreatedDate = DateTime.Now,
                        Path = "Login"

                    };

                    userloginhistory.MailAddress = model.MailAddress;
                    userloginhistory.CreatedDate = DateTime.Now;
                    userloginhistory.LoginResult = "Failed";

                   await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
                   await baseModel.BaseUnitOfWork.UserLoginHistoryRepository.AddAsync(userloginhistory);
                   await baseModel.BaseUnitOfWork.SaveChangesAsync();

                    return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.UserEmailNotFound });

                }


                var getRole = await baseModel.BaseUnitOfWork.RoleRepository.GetFirstOrDefaultAsync(x => x.Id == currentUser.RoleId);

                baseModel.WriteToSession("UserId", currentUser.Id.ToString());
                baseModel.WriteToSession("FirstName", currentUser.Name);
                baseModel.WriteToSession("LastName", currentUser.Surname);
                baseModel.WriteToSession("Email", currentUser.MailAddress);
                baseModel.WriteToSession("RoleName", getRole.RoleName);

                baseModel.ClaimCookies();

                userloginhistory.MailAddress = currentUser.MailAddress;
                userloginhistory.CreatedDate = DateTime.Now;
                userloginhistory.LoginResult = "Success";

               await baseModel.BaseUnitOfWork.UserLoginHistoryRepository.AddAsync(userloginhistory);
               await baseModel.BaseUnitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                ErrorLog log = new ErrorLog()
                {
                    UserId = 0,
                    ExceptionLevel = Convert.ToInt32(ExceptionTypeEnum.Error),
                    ExMessage = ExceptionMessageHelper.UnexpectedSystemError,
                    CreatedDate = DateTime.Now,
                    Path = "Login"

                };
               await baseModel.BaseUnitOfWork.ErrorLogRepository.AddAsync(log);
               await baseModel.BaseUnitOfWork.SaveChangesAsync();
                return new BadRequestObjectResult(new { isSuccess = false, message = ExceptionMessageHelper.UnexpectedSystemError });
            }

            return new OkObjectResult(new {isSuccess = true});
        }
    }
}
