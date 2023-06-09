using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection="Data Source=.\\SQLEXPRESS;Initial Catalog=DevIO-02;Integrated Security=True;pooling=false";
            optionsBuilder
                .UseSqlServer(strConnection)
                //.UseSqlServer(strConnection, p=>p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            //modelBuilder.Entity<Departamento>().HasQueryFilter(p => !p.Excluido);
        }
    }
}