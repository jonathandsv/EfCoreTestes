using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Conversor> Conversores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection="Data Source=.\\SQLEXPRESS;Initial Catalog=DevIO-02;Integrated Security=True;pooling=false";
            optionsBuilder
                .UseSqlServer(strConnection)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                //.EnableDetailedErrors()
                ;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            // modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

            // modelBuilder.Entity<Departamento>()
            //     .Property(p=>p.Descricao)
            //     .UseCollation("SQL_Latin1_General_CP1_CS_AS");

            // modelBuilder.HasSequence<int>("MinhaSequencia", "sequencias")
            //     .StartsAt(1)
            //     .IncrementsBy(2)
            //     .HasMin(1)
            //     .HasMax(10)
            //     .IsCyclic();

            // modelBuilder.Entity<Departamento>()
            //     .Property(p=>p.Id)
            //     .HasDefaultValueSql("NEXT VALUE FOR sequencias.MinhaSequencia");

            //Criando index
            // modelBuilder
            //     .Entity<Departamento>()
            //     .HasIndex(p=> new { p.Descricao, p.Ativo })
            //     .HasDatabaseName("idx_meu_indice_composto")
            //     .HasFilter("Descricao IS NOT NULL")
            //     //Fator de preenchimento
            //     .HasFillFactor(80)
            //     .IsUnique();

            // modelBuilder.Entity<Estado>()
            //     .HasData(new[] 
            //     {
            //         new Estado { Id = 1, Nome = "São Paulo"},
            //         new Estado { Id = 2, Nome = "Sergipe" }
            //     });

            //Criando schemas
            // modelBuilder.HasDefaultSchema("cadastros");

            // modelBuilder.Entity<Estado>().ToTable("Estados", "SegundoEsquema");

            var conversao = 
                new ValueConverter<Versao, string>(
                    p=>p.ToString(), p=> (Versao)Enum.Parse(typeof(Versao), p));

            // Todos os convertores
            //Microsoft.EntityFrameworkCore.Storage.ValueConversion                   

            var conversao1 = new EnumToStringConverter<Versao>();

            modelBuilder.Entity<Conversor>()
                .Property(p=>p.Versao)
                .HasConversion(conversao1)
                //.HasConversion(conversao)
                //.HasConversion(p=>p.ToString(), p=> (Versao)Enum.Parse(typeof(Versao), p))
                //.HasConversion<string>()
                ;

            modelBuilder.Entity<Conversor>()
                .Property(p=>p.Status)
                .HasConversion(new Curso.Conversores.ConversorCustomizado())
                ;

            modelBuilder.Entity<Departamento>().Property<DateTime>("UltimaAtualizacao");

            modelBuilder.Entity<Cliente>(p => 
            {
                p.OwnsOne(x=> x.Endereco, end => 
                {
                    end.Property(p=>p.Bairro).HasColumnName("Bairro");

                    end.ToTable("Endereco");
                });
            });
        }
    }
}