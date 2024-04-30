using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Domain.Models
{
    public class VisitorInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserIpAddress { get; set; } = null!;
        public string OperatingSystemName { get; set; } = null!;
        public int OperatingSystemVersionMajor { get; set; }
        public string OperatingSystemProcessor { get; set; } = null!;
        public string OperatingSystemLanguage { get; set; } = null!;
        public string BrowserName { get; set; } = null!;
        public int BrowserVersionMajor { get; set; }
        public string BrowserLanguage { get; set; } = null!;
        public string DeviceType { get; set; } = null!;
        public string EngineType { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public DateTime VisitDate { get; set; }
        public string Path { get; set; } = null!;

        public Siteuser User { get; set; }

    }
}
