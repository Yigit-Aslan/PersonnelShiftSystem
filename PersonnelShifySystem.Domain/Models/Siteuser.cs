using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Domain.Models
{
    public class Siteuser
    {
        public int Id {  get; set; }
        public string PersonnelCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MailAddress { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Role Role { get; set; }
		public ICollection<PersonnelTeam> PersonnelTeam { get; set; }
		public ICollection<ErrorLog> ErrorLog { get; set; }
        public ICollection<VisitorInfo> VisitorInfo { get; set; }


	}
}
