using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _write = new StreamWriter("meu_log_do_ef_core.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection="Data Source=.\\SQLEXPRESS;Initial Catalog=DevIO-02;Integrated Security=True;pooling=false";
            optionsBuilder
                //.UseSqlServer(strConnection)
                .UseSqlServer(
                    strConnection, 
                    o=>o
                        .MaxBatchSize(100)
                        .CommandTimeout(5)
                        .EnableRetryOnFailure(4, TimeSpan.FromSeconds(10), null))
                .LogTo(Console.WriteLine, LogLevel.Information)
                // .LogTo(Console.WriteLine, 
                //     new[] { CoreEventId.ContextInitialized, RelationalEventId.CommandExecuting},
                //     LogLevel.Information,
                //     DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine);
                //.LogTo(_write.WriteLine, LogLevel.Information)
                //.EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                ;
        }

        public override void Dispose()
        {
            base.Dispose();
            _write.Dispose();
        }
    }
}