using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Application.ViewModels.ShiftView
{
    public class ShiftViewModel
    {
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string ShiftDate { get; set; }
        public string ShiftStartEnd { get; set; }

        public string CreatedName { get; set;}
        public string UpdatedName { get; set; }

    }
}
