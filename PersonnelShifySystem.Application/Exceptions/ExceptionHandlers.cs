using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonnelShiftSystem.Infrastructure.Repository;

namespace PersonnelShiftSystem.Application.Exceptions
{
    public class ExceptionHandlers
    {
        private readonly PersonnelContext _context;

        public ExceptionHandlers(PersonnelContext context)
        {
            _context = context;
        }
        public bool CheckMail(string mail)
        {
            string checkMail = mail.Trim();

            var validation = new EmailAddressAttribute();
            var userMails = (from su in _context.Siteuser
                             where su.MailAddress == checkMail
                             select su.MailAddress).ToList();


            if ((validation.IsValid(checkMail)))
            {
                if (userMails.Contains(checkMail))
                {
                    return false;

                }
                return true;
            }

            return false;
        }


        public bool ValidMail(string mail)
        {
            // E-posta adresini temizle
            string validMail = mail.Trim();

            // E-posta adresinin geçerliliğini kontrol et
            var validation = new EmailAddressAttribute();
            if (!validation.IsValid(validMail))
            {
                return false;
            }

            // E-posta adresinin uzantısını kontrol et
            string[] validExtensions = { ".com", ".net", ".tr" }; // Geçerli uzantılar listesi
            string[] parts = validMail.Split('@');
            string[] domainParts = parts[1].Split('.');
            string extension = "." + domainParts[domainParts.Length - 1].ToLower();

            foreach (var validExtension in validExtensions)
            {
                if (extension == validExtension)
                {
                    return true;
                }
            }

            return false;
        }


        public bool ValidPhone(string phone)
        {
            string checkPhone = phone.Trim();
            var userPhones = (from su in _context.Siteuser
                              where su.PhoneNumber == checkPhone
                              select su.PhoneNumber).ToList();

            if (!(userPhones.Contains(checkPhone)))
            {
                return false;
            }

            return true;
        }

        //public string AddressValidation(AddressViewModel AddressModel)
        //{
        //    #region |Check Model Field|
        //    if (AddressModel.AddressTitle.IsNullOrEmpty())
        //        return ExceptionMessageHelper.CannotEmptyField("Adres Başlığı");
        //    if (AddressModel.AddressDetails.IsNullOrEmpty())
        //        return ExceptionMessageHelper.CannotEmptyField("Adres Detayı");
        //    if (AddressModel.CountryId == 0 || AddressModel.CountryId < 0)
        //        return ExceptionMessageHelper.CannotEmptyField("Ülke");
        //    if (AddressModel.ProvinceId == 0 || AddressModel.ProvinceId < 0)
        //        return ExceptionMessageHelper.CannotEmptyField("İl");
        //    if (AddressModel.DistrictId == 0 || AddressModel.DistrictId < 0)
        //        return ExceptionMessageHelper.CannotEmptyField("İlçe");
        //    if (AddressModel.PostCode.IsNullOrEmpty())
        //        return ExceptionMessageHelper.CannotEmptyField("Posta Kodu");

        //    #endregion

        //    #region |Size Control|
        //    if (AddressModel.AddressTitle.Length > 32)
        //        return ExceptionMessageHelper.LengthError("Adres Başlığı");
        //    if (AddressModel.AddressDetails.Length > 16777215)
        //        return ExceptionMessageHelper.LengthError("Adres Detayı");
        //    if (AddressModel.PostCode.Length > 5)
        //        return ExceptionMessageHelper.LengthError("Posta Kodu");


        //    #endregion
        //    return string.Empty;
        //}
    }
}
