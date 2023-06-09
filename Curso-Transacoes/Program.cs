﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Curso.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //GerenciandoTransacaoManualmente();
            //ComportamentoPadrao();
            //ReverterTransacao();
            //SalvarPontoTransacao();
            //TransactionScope();

            //Funções UDFS

            //FuncaoLEFT();
            //FuncaoDefinidaPeloUsuario();
            DateDIFF();
        }

        static void DateDIFF() 
        {
            CadastrarLivro();

            using var db = new Curso.Data.ApplicationContext();

            var resultado = db.Livros.Select(p => Curso.Funcoes.MinhasFuncoes.DateDiff("DAY", p.CadastradoEm, DateTime.Now));

            foreach (var diff in resultado)
            {
                Console.WriteLine(diff);
            }
        }

        static void FuncaoDefinidaPeloUsuario() 
        {
            CadastrarLivro();

            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(@"
                CREATE FUNCTION ConverterParaLetrasMaiusculas(@dados VARCHAR(100))
                RETURNS VARCHAR(100)
                BEGIN
                    RETURN UPPER(@dados)
                END");
            
            var resultado = db.Livros.Select(p=> Curso.Funcoes.MinhasFuncoes.LetrasMaiusculas(p.Titulo));

            foreach (var parteTitulo in resultado)
            {
                Console.WriteLine(parteTitulo);
            }
        }

        static void FuncaoLEFT()
        {
            CadastrarLivro();

            using var db = new Curso.Data.ApplicationContext();

            // var resultado = db.Livros.Select(p=> Curso.Data.ApplicationContext.Left(p.Titulo, 10));
            var resultado = db.Livros.Select(p=> Curso.Funcoes.MinhasFuncoes.Left(p.Titulo, 10));

            foreach (var parteTitulo in resultado)
            {
                Console.WriteLine(parteTitulo);
            }
        }

        static void TransactionScope()
        {
            CadastrarLivro();

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            };

            using(var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                ConsultarAtualizar();
                CadastraLivroEnterprise();
                CadastrarLivroDominandoEFCore();

                scope.Complete();
            }             
        } 

        static void ConsultarAtualizar()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {    
                var livro = db.Livros.FirstOrDefault(p=>p.Id == 1);
                livro.Autor = "Rafael Almeida"; 
                db.SaveChanges();
            }
        }

        static void CadastraLivroEnterprise()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {                  
                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "ASP.NET Core Enterprise Applications",
                        Autor = "Eduardo Pires"
                    });  
                db.SaveChanges();    
            }
        }

        static void CadastrarLivroDominandoEFCore()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {   
                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida"
                    });  
                db.SaveChanges();      
           
            }
        }
        static void SalvarPontoTransacao()
        {
            CadastrarLivro();

            using (var db = new Curso.Data.ApplicationContext())
            {                 
                using var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p=>p.Id == 1);
                    livro.Autor = "Rafael Almeida"; 
                    db.SaveChanges();

                    transacao.CreateSavepoint("desfazer_apenas_insercao");

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "ASP.NET Core Enterprise Applications",
                            Autor = "Eduardo Pires"
                        });  
                    db.SaveChanges();    

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "Dominando o Entity Framework Core",
                            Autor = "Rafael Almeida".PadLeft(16,'*')
                        });  
                    db.SaveChanges();     

                    transacao.Commit();            
                }
                catch(DbUpdateException e)
                {
                    transacao.RollbackToSavepoint("desfazer_apenas_insercao");

                    if(e.Entries.Count(p=>p.State == EntityState.Added) == e.Entries.Count)
                    {
                        transacao.Commit();
                    }
                }
                
            }
        } 

        static void ReverterTransacao()
        {
            CadastrarLivro();

            using (var db = new Curso.Data.ApplicationContext())
            {                 
                var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p=>p.Id == 1);
                    livro.Autor = "Rafael Almeida"; 
                    db.SaveChanges();

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "Dominando o Entity Framework Core",
                            Autor = "Rafael Almeida".PadLeft(16,'*')
                        }); 

                    db.SaveChanges();     

                    transacao.Commit();            
                }
                catch(Exception e)
                {
                    transacao.Rollback();
                }
                
            }
        }

        static void GerenciandoTransacaoManualmente()
        {
            CadastrarLivro();

            using (var db = new Curso.Data.ApplicationContext())
            {                 
                var transacao = db.Database.BeginTransaction();

                var livro = db.Livros.FirstOrDefault(p=>p.Id == 1);
                livro.Autor = "Rafael Almeida"; 
                db.SaveChanges();

                Console.ReadKey();

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida"
                    }); 

                db.SaveChanges();     

                transacao.Commit();            
            }
        }

        static void ComportamentoPadrao()
        {
            CadastrarLivro();

            using (var db = new Curso.Data.ApplicationContext())
            {                 
                var livro = db.Livros.FirstOrDefault(p=>p.Id == 1);
                livro.Autor = "Rafael Almeida"; 

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida"
                    }); 

                db.SaveChanges();                 
            }
        }

        static void CadastrarLivro()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Introdução ao Entity Framework Core",
                        Autor = "Rafael",
                        CadastradoEm = DateTime.Now.AddDays(-1)
                    }); 

                db.SaveChanges();
            }
        }

    }
}
