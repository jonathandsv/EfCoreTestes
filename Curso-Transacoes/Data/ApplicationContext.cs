using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Curso.Domain;
using Curso.Funcoes;
using Curso.Interceptadores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Livro> Livros { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection="Data Source=.\\SQLEXPRESS;Initial Catalog=DevIO-02;Integrated Security=True;pooling=false";
            optionsBuilder
                .UseSqlServer(strConnection)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                ;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            //Com data anatotions
            //Curso.Funcoes.MinhasFuncoes.RegistrarFuncoes(modelBuilder);

            modelBuilder.HasDbFunction(_minhaFuncao)
                .HasName("LEFT")
                .IsBuiltIn();

            modelBuilder.HasDbFunction(_letrasMaiusculas)
                .HasName("ConverterParaLetrasMaiusculas")
                .HasSchema("dbo");

            modelBuilder.HasDbFunction(_dateDiff)
                .HasName("DATEDIFF")
                .HasTranslation(P => 
                {
                    var argumentos = P.ToList();

                    var constante = (SqlConstantExpression)argumentos[0];
                    argumentos[0] = new SqlFragmentExpression(constante.Value.ToString());

                    return new SqlFunctionExpression("DATEDIFF", argumentos, false, new[]{false, false, false}, typeof(int), null);
                })
                .IsBuiltIn();
        }

        private static MethodInfo _minhaFuncao = typeof(MinhasFuncoes)
            .GetRuntimeMethod("Left", new[] {typeof(string), typeof(int)});
        private static MethodInfo _letrasMaiusculas = typeof(MinhasFuncoes)
            .GetRuntimeMethod(nameof(MinhasFuncoes.LetrasMaiusculas), new[] {typeof(string)});
        private static MethodInfo _dateDiff = typeof(MinhasFuncoes)
            .GetRuntimeMethod(nameof(MinhasFuncoes.DateDiff), new[] {typeof(string), typeof(DateTime), typeof(DateTime)});

        // [DbFunction(name: "LEFT", IsBuiltIn = true)]
        // public static string Left(string dados, int quantidade) 
        // {
        //     throw new NotImplementedException();
        // }
    }
}