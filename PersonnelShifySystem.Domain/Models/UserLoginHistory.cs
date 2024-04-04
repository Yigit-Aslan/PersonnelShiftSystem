using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Domain.Models
{
    public class UserLoginHistory
    {
        public int Id { get; set; }
        public string MailAddress { get; set; }
        public string LoginResult { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
