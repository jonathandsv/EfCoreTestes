using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Curso.Domain
{
    public class Ator
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<Filme> Filmes { get; } = new List<Filme>();
    }

    public class Filme 
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public ICollection<Ator> Atores { get; } = new List<Ator>();
    }
}