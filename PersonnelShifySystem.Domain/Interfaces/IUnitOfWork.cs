using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>() where T : class;
        IRepository<Siteuser> SiteUserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<Team> TeamRepository { get; }
        IRepository<PersonnelTeam> PersonnelTeamRepository { get; }
        IRepository<Shift> ShiftRepository { get; }
        IRepository<AssignTeamShift> AssignTeamShiftRepository { get; }
        IRepository<ErrorLog> ErrorLogRepository { get; }
        IRepository<UserLoginHistory> UserLoginHistoryRepository { get; }

        Task BeginTransactionAsync();
        public void CommitTransaction();
        public void RollbackTransaction();

        Task<int> SaveChangesAsync();
        Task RollbackAsync();
        void Dispose();
    }
}
