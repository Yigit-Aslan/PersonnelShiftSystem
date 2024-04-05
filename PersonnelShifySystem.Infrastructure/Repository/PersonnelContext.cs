using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonnelShiftSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelShiftSystem.Infrastructure.Repository
{
    public class PersonnelContext : DbContext
    {
        public PersonnelContext(DbContextOptions<PersonnelContext> options) : base(options)
        {
        }

        public DbSet<Siteuser> Siteuser { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<PersonnelTeam> PersonnelTeam { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<AssignTeamShift> AssignTeamShift { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
		    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
		    .AddJsonFile("appsettings.json")
		    .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new ProductConfiguration());
            //// Diğer özelleştirmeleri buraya ekleyebilirsiniz
        }
    }
}
