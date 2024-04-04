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
        public DbSet<Role> Roles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new ProductConfiguration());
            //// Diğer özelleştirmeleri buraya ekleyebilirsiniz
        }
    }
}
