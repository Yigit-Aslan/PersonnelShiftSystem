using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonnelShiftSystem.Domain.Models;
using System.Text.RegularExpressions;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class SiteUserRepository : Repository<Siteuser>
    {
        private readonly PersonnelContext _context;

        public SiteUserRepository(PersonnelContext context) : base(context)
        {
            _context = context;
        }
        public static bool ValidatePassword(string password)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*\d).{6,}$";


            return Regex.IsMatch(password, pattern);
        }

        public static string EncryptPassword(string password)
        {
            string hashedPW = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

            return hashedPW;
        }

        public static bool VerifyPassword(string password, string cryptedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, cryptedPassword);
        }

        public static string GenerateFollowUpCode()
        {
            string guid1 = Guid.NewGuid().ToString().Replace("-", "");
            string guid2 = Guid.NewGuid().ToString().Replace("-", "");

            return $"{guid1}{guid2}";
        }


        public static string GeneratePersonnelCode()
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder code = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                code.Append(chars[random.Next(chars.Length)]);
            }
            return code.ToString().ToUpper();
        }
    }
}
