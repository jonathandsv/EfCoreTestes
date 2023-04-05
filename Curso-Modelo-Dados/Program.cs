using System;
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
            TiposDePropriedades();
        }

        static void TiposDePropriedades() 
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente 
            {
                Nome = "Fulano de tal",
                Telefone = "(79) 98888-9999",
                Endereco = new Endereco { Bairro = "Centro", Cidade = "São Paulo"}
            };

            db.Clientes.Add(cliente);

            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

            clientes.ForEach(cli => 
            {
                var json = System.Text.Json.JsonSerializer.Serialize(cli, options);

                Console.WriteLine(json);
            });
        }

        static void TrabalhandoComPropriedadeDeSombra() 
        {
            using var db = new Curso.Data.ApplicationContext();
            // db.Database.EnsureDeleted();
            // db.Database.EnsureCreated();

            // var departamento = new Departamento 
            // {
            //     Descricao = "Departamento Propriedade de Sombra"
            // };

            // db.Departamentos.Add(departamento);

            // db.Entry(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;

            // db.SaveChanges();

            var departamentos = 
                db.Departamentos
                .Where(p=> 
                    EF.Property<DateTime>(p, "UltimaAtualizacao") < DateTime.Now)
                .ToArray();
        }

        static void PropriedadeDeSombra() 
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        static void ConversorCustomizado() 
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Conversores.Add(
                new Curso.Domain.Conversor 
                {
                    Status = Curso.Domain.Status.Devolvido,

                });
            
            db.SaveChanges();

            var conversorEmAnalise = db.Conversores
                .AsNoTracking()
                .FirstOrDefault(p=>p.Status == Curso.Domain.Status.Analise);
            var conversorDevolvido = db.Conversores
                .AsNoTracking()
                .FirstOrDefault(p=>p.Status == Curso.Domain.Status.Devolvido);
        }

        static void ConversorDeValor() => Esquema();

        static void Esquema() 
        {
            using var db = new Curso.Data.ApplicationContext();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }

        static void PropagarDados() 
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }

        static void Collations() 
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        
    }
}
