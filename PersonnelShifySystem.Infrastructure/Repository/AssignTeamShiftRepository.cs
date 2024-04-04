using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class AssignTeamShiftRepository : Repository<AssignTeamShift>
    {
        private readonly PersonnelContext _context;

        public AssignTeamShiftRepository(PersonnelContext context) : base(context)
        {
            _context = context;
        }
    }
}
