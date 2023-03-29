using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //EnsureCreatedAndDeleted();
            //HelthCheckBancoDeDados();

            //warmup
            // new Curso.Data.ApplicationContext().Departamentos.AsNoTracking().Any();
            // _count = 0;
            // GerenciarEstadoDaConexao(false);
            // _count = 0;
            // GerenciarEstadoDaConexao(true);
            SqlInjection();
        }

        static void SqlInjection() 
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Curso.Domain.Departamento 
                {
                    Descricao = "Departamento 01"
                },
                new Curso.Domain.Departamento 
                {
                    Descricao = "Departamento 02"

                });
            db.SaveChanges();

            //Forma correta
            // var descricao = "Departamento 01";
            // db.Database.ExecuteSqlRaw("update departamentos set descricao='DepartamentoAlterado' where descricao={0}", descricao);

            //SQLINJECTION 
            var descricao = "TESTE ' or 1='1";
            db.Database.ExecuteSqlRaw($"update departamentos set descricao='AtaqueSQLINJECTION' where descricao='{descricao}'");

            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descricao: {departamento.Descricao}");
            }

        }

        static void ExecuteSQL() 
        {
            using var db = new Curso.Data.ApplicationContext();

            //Primeira opção 
            using(var cmd = db.Database.GetDbConnection().CreateCommand()) 
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            //Segunda opção **Protege conta SQL INJECTION
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamenteos set descricao={0} where id=1", descricao);

            //Terceira opção **Protege conta SQL INJECTION
            db.Database.ExecuteSqlInterpolated($"update departamenteos set descricao={descricao} where id=1");
        }

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new Curso.Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();
            
            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __) => ++_count;

            if (gerenciarEstadoConexao) 
            {
                conexao.Open();
            }

            for (int i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }

        static void HelthCheckBancoDeDados() 
        {
            using var db = new Curso.Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if (canConnect) 
            {
                Console.WriteLine("Posso me conectar");
            }
            else 
            {
                Console.WriteLine("Não posso me conectar");
            }

            // try
            // {
            //     //1
            //     var connection = db.Database.GetDbConnection();
            //     connection.Open();

            //     //2
            //     db.Departamentos.Any();

            //     Console.WriteLine("Posso me conectar");
            // }
            // catch (System.Exception)
            // {
            //     Console.WriteLine("Não posso me conectar");
            // }

        }

        static void EnsureCreatedAndDeleted() 
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureCreated();
            //db.Database.EnsureDeleted();
        }
    }
}
