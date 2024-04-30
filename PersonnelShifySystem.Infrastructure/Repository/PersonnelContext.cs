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
        public DbSet<AssignShiftTeam> AssignShiftTeam { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistory { get; set; }
        public DbSet<VisitorInfo> VisitorInfo { get; set; }

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
            modelBuilder.Entity<Siteuser>()
                .HasOne(s => s.Role)
                .WithMany(r => r.Siteuser)
                .HasForeignKey(s => s.RoleId);

            modelBuilder.Entity<Siteuser>()
                .HasMany(s => s.ErrorLog)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Siteuser>()
                .HasMany(s => s.PersonnelTeam)
                .WithOne(pt => pt.User)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<Siteuser>()
                .HasMany(s => s.VisitorInfo)
                .WithOne(r => r.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.PersonnelTeam)
                .WithOne(pt => pt.Team)
                .HasForeignKey(pt => pt.TeamId);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.AssignTeamShift)
                .WithOne(at => at.Team)
                .HasForeignKey(at => at.TeamId);

            modelBuilder.Entity<Shift>()
                .HasMany(s => s.AssignShiftTeam)
                .WithOne(at => at.Shift)
                .HasForeignKey(at => at.ShiftId);

            
        }
    }
}
