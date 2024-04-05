using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Domain.Interfaces
{
    public interface IBaseModel
    {
        IUnitOfWork BaseUnitOfWork { get; set; }
        IMapper Mapper { get; set; }

        byte ItemActive();

        byte ItemPassive();

        int? ItemNullInt();

        void WriteToSession(string key, string value);

        string ReadFromSession(string key);

        void SaveVisitorInfo();

        string GetGuidId();

        void ClaimCookies();
    }
}
