using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Domain.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set;}
        public DateTime? UpdatedDate { get; set; }


		public ICollection<PersonnelTeam> PersonnelTeam { get; set; }
		public ICollection<AssignShiftTeam> AssignTeamShift { get; set; }

	}
}
