using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class ErrorLogRepository : Repository<ErrorLog>
    {
        private readonly PersonnelContext _context;

        public ErrorLogRepository(PersonnelContext context) : base(context)
        {
            _context = context;
        }
    }
}
