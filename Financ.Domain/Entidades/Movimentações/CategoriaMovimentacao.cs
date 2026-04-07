using Financ.Domain.Entidades.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.Movimentações
{
    public class CategoriaMovimentacao
    {
        public int Id { get; set; }
        public int IdConta { get; set; }
        public string Nome { get; set; }

        public Conta Conta { get; set; }
        public CategoriaMovimentacao(){}

    }
}
