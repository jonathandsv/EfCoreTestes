﻿using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //FiltroGlobal();
            //IgnoreFiltroGlobal();
            //ConsultaProjetada();
            //ConsultaParametrizada();
            //ConsultaInterpolada();
            //ConsultaComTag();
            //EntendendoConsulta1NN1();
            DivisaoDeConsulta();
        }

        static void DivisaoDeConsulta() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                .Where(p => p.Id < 3)
                //.AsSingleQuery()
                .AsSingleQuery()
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descricao: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\t Nome: {funcionario.Nome}");
                }
            }
        }

        static void EntendendoConsulta1NN1() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var funcinoarios = db.Funcionarios
                .Include(p => p.Departamento)
                .ToList();

            foreach (var funcionario in funcinoarios)
            {
                Console.WriteLine($"\t Nome: {funcionario.Nome} / Descricao Dep: {funcionario.Departamento.Descricao}");
            }

            // var departamentos = db.Departamentos
            //     .Include(p => p.Funcionarios)
            //     .ToList();

            // foreach (var departamento in departamentos)
            // {
            //     Console.WriteLine($"Descricao: {departamento.Descricao}");

            //     foreach (var funcionario in departamento.Funcionarios)
            //     {
            //         Console.WriteLine($"\t Nome: {funcionario.Nome}");
            //     }
            // }
        }

        static void ConsultaComTag() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var departamentos = db.Departamentos
                .TagWith(@"Estou enviando um comentario para o servidor
                    Segundo comentario
                    Terceiro comentario
                    ")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descricao: {departamento.Descricao}");
            }
        }

        static void ConsultaInterpolada() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var id = 1;
            var departamentos = db.Departamentos
                .FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id>{id}")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descricao: {departamento.Descricao}");
            }
        }

        static void ConsultaParametrizada() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var id = new SqlParameter 
            {
                Value = 1,
                SqlDbType = System.Data.SqlDbType.Int
            };
            var departamentos = db.Departamentos
                .FromSqlRaw("SELECT * FROM Departamentos WHERE Id>{0}", id)
                .Where(p => !p.Excluido)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descricao: {departamento.Descricao}");
            }
        }

        static void ConsultaProjetada() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var departamentos = db.Departamentos
                .Where(p => p.Id > 0)
                .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome)})
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descricao: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($" \t Nome: {funcionario}");
                }
            }
        }

        static void IgnoreFiltroGlobal() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descricao: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void FiltroGlobal() 
        {
            using var db = new Curso.Data.ApplicationContext();

            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descricao: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void Setup(Curso.Data.ApplicationContext db) 
        {
            if (db.Database.EnsureCreated()) 
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "99999999911",
                                RG= "2100062"
                            }
                        },
                        Excluido = true
                    },
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Bruno Brito",
                                CPF = "88888888811",
                                RG= "3100062"
                            },
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Eduardo Pires",
                                CPF = "77777777711",
                                RG= "1100062"
                            }
                        }
                    }
                );

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }

        }

        
    }
}