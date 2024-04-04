using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Domain.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExceptionLevel { get; set; }
        public string ExMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Path { get; set; }
    }
}
