using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class ShiftRepository : Repository<PersonnelTeam>
    {
        private readonly PersonnelContext _context;

        public ShiftRepository(PersonnelContext context) : base(context)
        {
            _context = context;
        }
    }
}
