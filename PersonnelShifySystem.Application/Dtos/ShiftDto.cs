using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Application.Dtos
{
    public class ShiftDto
    {
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
