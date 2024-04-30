using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    internal class VisitorInfoRepository : Repository<VisitorInfo>
    {
        private readonly PersonnelContext _context;

        public VisitorInfoRepository(PersonnelContext context) : base(context)
        {
            _context = context;
        }
    }
}
