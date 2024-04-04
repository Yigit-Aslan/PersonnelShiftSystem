using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonnelShiftSystem.Domain.Models;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class RoleRepository : Repository<Role>
    {
        private readonly PersonnelContext _context;

        public RoleRepository(PersonnelContext context) : base(context)
        {
            _context = context;
        }
    }
}
