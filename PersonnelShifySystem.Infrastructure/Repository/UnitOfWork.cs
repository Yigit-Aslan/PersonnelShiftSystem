using PersonnelShiftSystem.Domain.Interfaces;
using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private PersonnelContext _context;
        private Repository<Siteuser> _siteUserRepository;
        private Repository<Role> _roleRepository;
        private Repository<Team> _teamRepository;
        private Repository<PersonnelTeam> _personnelTeamRepository;
        private Repository<Shift> _shiftRepository;
        private Repository<AssignTeamShift> _assignTeamShiftRepository;
        private Repository<ErrorLog> _errorLogRepository;
        private Repository<UserLoginHistory> _userLoginHistoryRepository;
        public IRepository<Siteuser> SiteUserRepository => _siteUserRepository ?? (this._siteUserRepository = new Repository<Siteuser>(_context));
        public IRepository<Role> RoleRepository => _roleRepository ?? (this._roleRepository = new Repository<Role>(_context));
        public IRepository<Team> TeamRepository => _teamRepository ?? (this._teamRepository = new Repository<Team>(_context));
        public IRepository<PersonnelTeam> PersonnelTeamRepository => _personnelTeamRepository ?? (this._personnelTeamRepository = new Repository<PersonnelTeam>(_context));
        public IRepository<Shift> ShiftRepository => _shiftRepository ?? (this._shiftRepository = new Repository<Shift>(_context));
        public IRepository<AssignTeamShift> AssignTeamShiftRepository => _assignTeamShiftRepository ?? (this._assignTeamShiftRepository = new Repository<AssignTeamShift>(_context));
        public IRepository<ErrorLog> ErrorLogRepository => _errorLogRepository ?? (this._errorLogRepository = new Repository<ErrorLog>(_context));
        public IRepository<UserLoginHistory> UserLoginHistoryRepository => _userLoginHistoryRepository ?? (this._userLoginHistoryRepository = new Repository<UserLoginHistory>(_context));


        public UnitOfWork(PersonnelContext context)
        {
            this._context = context;
        }


        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }


        public async Task<int> SaveChangesAsync()
        {
            int result = -1;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (_context == null)
                        throw new ArgumentException("Context is null");

                    result = await _context.SaveChangesAsync();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    var foo = ex.Message;

                    transaction.Dispose();
                }
            }

            return result;
        }

        public async Task RollbackAsync()
        {
            await Task.CompletedTask; 
            _context.Dispose();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
