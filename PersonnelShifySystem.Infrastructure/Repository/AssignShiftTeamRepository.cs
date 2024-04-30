using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class AssignShiftTeamRepository : Repository<AssignShiftTeam>
    {
        private readonly PersonnelContext _context;

        public AssignShiftTeamRepository(PersonnelContext context) : base(context)
        {
            _context = context;
        }
    }
}
