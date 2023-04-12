﻿using System;
using System.Collections.Generic;
using System.Linq;
using Curso.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Collations();
            //PropagarDados();
            //Esquema();
            //ConversorDeValor();
            //ConversorCustomizado();
            //PropriedadeDeSombra();
            //TrabalhandoComPropriedadeDeSombra();
            //TiposDePropriedades();
            //Relacionamento1Para1();
            //Relacionamento1ParaMuitos();
            //RelacionamentoMuitosParaMuitos();
            //CampoDeApoio();
            //ExemploTPH();
            //PacotesDePropriedades();
            //Atributos();
            //FuncoesDeDatas();
            //FuncaoLike();
            //FuncaoDataLength();
            //FuncaoProperty();
            //FuncaoCollate();
            TesteInterceptacao();
        }

        static void TesteInterceptacao()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                var consulta = db
                    .Funcoes
                    .TagWith("Use NOLOCK")
                    .FirstOrDefault(); 

                Console.WriteLine($"Consulta: {consulta?.Descricao1}");
            }
        }

        static void FuncaoCollate() 
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                var consulta1 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CS_AS") == "tela");

                var consulta2 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CI_AS") == "tela");

                Console.WriteLine($"Consulta1: {consulta1?.Descricao1}");
                Console.WriteLine($"Consulta2: {consulta2?.Descricao1}");
            }
        }

        static void FuncaoProperty() 
        {
            ApagarCriarBancoDeDados();

            using (var db = new Curso.Data.ApplicationContext())
            {
                var resultado = db
                    .Funcoes
                    //.AsNoTracking()
                    .FirstOrDefault(p => EF.Property<string>(p, "PropriedadeSombra") == "Teste");
                
                var propriedadeSombra = db
                    .Entry(resultado)
                    .Property<string>("PropriedadeSombra")
                    .CurrentValue;
                
                Console.WriteLine("Resultado: ");

                Console.WriteLine(propriedadeSombra);
            }
        }

        static void FuncaoDataLength() 
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                var resultado = db
                    .Funcoes
                    .AsNoTracking()
                    .Select(p => new 
                    {
                        TotalBytesCampoData = EF.Functions.DataLength(p.Data1),
                        TotalBytes1 = EF.Functions.DataLength(p.Descricao1),
                        TotalBytes2 = EF.Functions.DataLength(p.Descricao2),
                        Total1 = p.Descricao1.Length,
                        Total2 = p.Descricao2.Length
                    })
                    .FirstOrDefault();
                
                Console.WriteLine("Resultado: ");

                Console.WriteLine(resultado);
            }
        }

        static void FuncaoLike() 
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                var dados = db
                    .Funcoes
                    .AsNoTracking()
                    //.Where(p => EF.Functions.Like(p.Descricao1, "Bo%"))
                    .Where(p => EF.Functions.Like(p.Descricao1, "B[ao]%"))
                    .Select(p => p.Descricao1)
                    .ToArray();
                
                Console.WriteLine("Resultado: ");

                foreach (var descricao in dados)
                {
                    Console.WriteLine(descricao);
                }

            }
        }

        static void FuncoesDeDatas()
        {
            ApagarCriarBancoDeDados();

            using (var db = new Curso.Data.ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                var dados = db.Funcoes.AsNoTracking().Select(p =>
                   new
                   {
                       Dias = EF.Functions.DateDiffDay(DateTime.Now, p.Data1),
                       Meses = EF.Functions.DateDiffMonth(DateTime.Now, p.Data1),
                       Data = EF.Functions.DateFromParts(2021, 1, 2),
                       DataValida = EF.Functions.IsDate(p.Data2),
                   });

                foreach (var f in dados)
                {
                    Console.WriteLine(f);
                }

            }
        }

        static void ApagarCriarBancoDeDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Funcoes.AddRange(
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(2),
                Data2 = "2021-01-01",
                Descricao1 = "Bala 1 ",
                Descricao2 = "Bala 1 "
            },
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(1),
                Data2 = "XX21-01-01",
                Descricao1 = "Bola 2",
                Descricao2 = "Bola 2"
            },
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(1),
                Data2 = "XX21-01-01",
                Descricao1 = "Tela",
                Descricao2 = "Tela"
            });

            db.SaveChanges();
        }

        // static void Atributos() 
        // {
        //     using (var db = new Curso.Data.ApplicationContext()) 
        //     {
        //         db.Database.EnsureDeleted();
        //         db.Database.EnsureCreated();

        //         var script = db.Database.GenerateCreateScript();

        //         Console.WriteLine(script);

        //         db.Atributos.Add(new Atributo {
        //             Descricao = "Exemplo",
        //             Observacao = "Observação"
        //         });

        //         db.SaveChanges();
        //     }
        // }

        // static void PacotesDePropriedades() 
        // {
        //     using (var db = new Curso.Data.ApplicationContext()) 
        //     {
        //         db.Database.EnsureDeleted();
        //         db.Database.EnsureCreated();

        //         var configuracao = new Dictionary<string, object>
        //         {
        //             ["Chave"] = "SenhaBnacoDeDados",
        //             ["Valor"] = Guid.NewGuid().ToString()
        //         };

        //         db.Configuracoes.Add(configuracao);
        //         db.SaveChanges();

        //         var configuracoes = db
        //             .Configuracoes
        //             .AsNoTracking()
        //             .Where(p => p["Chave"] == "SenhaBancoDeDados")
        //             .ToArray();
                
        //         foreach (var dic in configuracoes)
        //         {
        //             Console.WriteLine($"Chave> {dic["Chave"]} - Valor: {dic["Valor"]}");
        //         }
        //     }
        // }

        // static void ExemploTPH()
        // {
        //     using (var db = new Curso.Data.ApplicationContext()) 
        //     {
        //         db.Database.EnsureDeleted();
        //         db.Database.EnsureCreated();

        //         var pessoa = new Pessoa { Nome = "Fulano de Tal" };

        //         var instrutor = new Instrutor { Nome = "Rafael Almeida", Tecnologia = ".NET", Desde = DateTime.Now };

        //         var aluno = new Aluno { Nome = "Maria Thysbe", Idade = 31, DataContrato = DateTime.Now.AddDays(-1) };

        //         db.AddRange(pessoa, instrutor, aluno);
        //         db.SaveChanges();

        //         var pessoas = db.Pessoas.AsNoTracking().ToArray();
        //         var instrutores = db.Instrutores.AsNoTracking().ToArray();
        //         //var alunos = db.Alunos.AsNoTracking().ToArray();
        //         var alunos = db.Pessoas.OfType<Aluno>().AsNoTracking().ToArray();

        //         Console.WriteLine("Pessoas **************");
        //         foreach (var p in pessoas)
        //         {
        //             Console.WriteLine($"Id: {p.Id} -> {p.Nome}");
        //         }

        //         Console.WriteLine("Instrutores **************");
        //         foreach (var p in instrutores)
        //         {
        //             Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Tecnologia: {p.Tecnologia}, Desde: {p.Desde}");
        //         }

        //         Console.WriteLine("Alunos **************");
        //         foreach (var p in alunos)
        //         {
        //             Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Idade: {p.Idade}, Data do Contrato: {p.DataContrato}");
        //         }
        //     }
        // }

        // static void CampoDeApoio()
        // {
        //     using (var db = new Curso.Data.ApplicationContext())
        //     {
        //         db.Database.EnsureDeleted();
        //         db.Database.EnsureCreated();

        //         var documento = new Documento();
        //         documento.SetCPF("12345678933");

        //         db.Documentos.Add(documento);

        //         db.SaveChanges();

        //         foreach (var doc in db.Documentos.AsNoTracking())
        //         {
        //             Console.WriteLine($"CPF: {doc.GetCPF()}");
        //         }
        //     }
        // }

        // static void RelacionamentoMuitosParaMuitos()
        // {
        //     using (var db = new Curso.Data.ApplicationContext())
        //     {
        //         db.Database.EnsureDeleted();
        //         db.Database.EnsureCreated();

        //         var ator1 = new Ator { Nome = "Rafael" };
        //         var ator2 = new Ator { Nome = "Pires" };
        //         var ator3 = new Ator { Nome = "Bruno" };

        //         var filme1 = new Filme { Descricao = "A volta dos que não foram" };
        //         var filme2 = new Filme { Descricao = "De volta para o futuro" };
        //         var filme3 = new Filme { Descricao = "Poeira em alto mar filme" };

        //         ator1.Filmes.Add(filme1);
        //         ator1.Filmes.Add(filme2);

        //         ator2.Filmes.Add(filme1);

        //         filme3.Atores.Add(ator1);
        //         filme3.Atores.Add(ator2);
        //         filme3.Atores.Add(ator3);

        //         db.AddRange(ator1, ator2, filme3);

        //         db.SaveChanges();

        //         foreach (var ator in db.Atores.Include(e => e.Filmes))
        //         {
        //             Console.WriteLine($"Ator: {ator.Nome}");

        //             foreach (var filme in ator.Filmes)
        //             {
        //                 Console.WriteLine($"\tFilme: {filme.Descricao}");
        //             }
        //         }
        //     }
        // }

        // static void Relacionamento1ParaMuitos() 
        // {
        //     using (var db = new Curso.Data.ApplicationContext()) 
        //     {
        //         db.Database.EnsureDeleted();
        //         db.Database.EnsureCreated();

        //         var estado = new Estado
        //         {
        //             Nome = "Sergipe",
        //             Governador = new Governador { Nome = "Rafael Almeida"}
        //         };

        //         estado.Cidades.Add(new Cidade { Nome = "Itabainana"});

        //         db.Estados.Add(estado);

        //         db.SaveChanges();
        //     };

        //     using (var db = new Curso.Data.ApplicationContext()) 
        //     {
        //         var estados = db.Estados.ToList();

        //         estados[0].Cidades.Add(new Cidade { Nome = "Aracaju"});

        //         db.SaveChanges();

        //         foreach (var est in db.Estados.Include(p=>p.Cidades).AsNoTracking())
        //         {
        //             Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");

        //             foreach (var cidade in est.Cidades)
        //             {
        //                 Console.WriteLine($"\t Cidade: {cidade.Nome}");
        //             }
        //         }
        //     }
        // }

        // static void Relacionamento1Para1() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();

        //     var estado = new Estado
        //     {
        //         Nome = "Sergipe",
        //         Governador = new Governador { Nome = "Rafael Almeida"}
        //     };

        //     db.Estados.Add(estado);

        //     db.SaveChanges();

        //     var estados = db.Estados.AsNoTracking().ToList();

        //     estados.ForEach(est => 
        //     {
        //         Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");
        //     });
        // }

        // static void TiposDePropriedades() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();

        //     var cliente = new Cliente 
        //     {
        //         Nome = "Fulano de tal",
        //         Telefone = "(79) 98888-9999",
        //         Endereco = new Endereco { Bairro = "Centro", Cidade = "São Paulo"}
        //     };

        //     db.Clientes.Add(cliente);

        //     db.SaveChanges();

        //     var clientes = db.Clientes.AsNoTracking().ToList();

        //     var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

        //     clientes.ForEach(cli => 
        //     {
        //         var json = System.Text.Json.JsonSerializer.Serialize(cli, options);

        //         Console.WriteLine(json);
        //     });
        // }

        // static void TrabalhandoComPropriedadeDeSombra() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     // db.Database.EnsureDeleted();
        //     // db.Database.EnsureCreated();

        //     // var departamento = new Departamento 
        //     // {
        //     //     Descricao = "Departamento Propriedade de Sombra"
        //     // };

        //     // db.Departamentos.Add(departamento);

        //     // db.Entry(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;

        //     // db.SaveChanges();

        //     var departamentos = 
        //         db.Departamentos
        //         .Where(p=> 
        //             EF.Property<DateTime>(p, "UltimaAtualizacao") < DateTime.Now)
        //         .ToArray();
        // }

        // static void PropriedadeDeSombra() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();
        // }

        // static void ConversorCustomizado() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();

        //     db.Conversores.Add(
        //         new Curso.Domain.Conversor 
        //         {
        //             Status = Curso.Domain.Status.Devolvido,

        //         });
            
        //     db.SaveChanges();

        //     var conversorEmAnalise = db.Conversores
        //         .AsNoTracking()
        //         .FirstOrDefault(p=>p.Status == Curso.Domain.Status.Analise);
        //     var conversorDevolvido = db.Conversores
        //         .AsNoTracking()
        //         .FirstOrDefault(p=>p.Status == Curso.Domain.Status.Devolvido);
        // }

        // static void ConversorDeValor() => Esquema();

        // static void Esquema() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();

        //     var script = db.Database.GenerateCreateScript();
        //     Console.WriteLine(script);
        // }

        // static void PropagarDados() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();

        //     var script = db.Database.GenerateCreateScript();
        //     Console.WriteLine(script);
        // }

        // static void Collations() 
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();
        // }

        
    }
}