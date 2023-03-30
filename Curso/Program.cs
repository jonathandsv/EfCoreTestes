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
            //SqlInjection();
            //MigracoesPendentes();
            //AplicarMigracaoEmTempodeExecucao();
            //TodasMigracoes();
            //MigracoesJaAplicadas();
            //ScriptGeralDoBancoDeDados();
            //CarregamentoAdiantado();
            //CarregamentoExplicito();
            CarregamentoLento();
        }

        static void CarregamentoLento() 
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;
            
            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false) 
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else 
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoExplicito() 
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                if (departamento.Id == 2) 
                {
                    //db.Entry(departamento).Collection(p=>p.Funcionarios).Load();
                    db.Entry(departamento).Collection(p=>p.Funcionarios).Query().Where(p=>p.Id > 2).ToList();
                }
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false) 
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else 
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoAdiantado() 
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .Include(p=>p.Funcionarios);

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false) 
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else 
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void SetupTiposCarregamentos(Curso.Data.ApplicationContext db) 
        {
            if (!db.Departamentos.Any()) 
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento 
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario> 
                        {
                            new Curso.Domain.Funcionario 
                            {
                                Nome = "Rafael Almeida",
                                CPF = "999999999911",
                                RG = "2100062"
                            }
                        }
                    },
                    new Curso.Domain.Departamento 
                    {
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario> 
                        {
                            new Curso.Domain.Funcionario 
                            {
                                Nome = "Bruno Brito",
                                CPF = "88888888811",
                                RG = "3100062"
                            },
                            new Curso.Domain.Funcionario 
                            {
                                Nome = "Eduardo Pires",
                                CPF = "77777777711",
                                RG = "1100062"
                            }
                        }
                    }
                );
                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        static void ScriptGeralDoBancoDeDados() 
        {
            using var db = new Curso.Data.ApplicationContext();

            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        static void MigracoesJaAplicadas() 
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migracao: {migracao}");
            }
        }

        static void TodasMigracoes() 
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migracao: {migracao}");
            }
        }

        static void AplicarMigracaoEmTempodeExecucao() 
        {
            using var db = new Curso.Data.ApplicationContext();

            db.Database.Migrate();
        }

        static void MigracoesPendentes() 
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migracoes: {migracao}");
            }
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
