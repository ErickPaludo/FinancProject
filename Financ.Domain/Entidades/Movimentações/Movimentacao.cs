using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.Movimentações
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public TipoMovimentacaoCorrente Tipo { get; set; }
        public int IdConta { get; set; }
        public int IdContaUsuario { get; set; }
        public int IdCategoria { get; set; }
        public int IdFixo { get; set; }
        public decimal Valor { get; set; }
        public TipoStatusMovimentacao Status { get; set; }
        public string Titulo { get; set; }
        public string Observacao { get; set; }
        public DateTime DthrReg { get; set; }
        public DateTime DthrPagamento { get; set; }
        public Conta Conta { get; set; }
        public ContaUsuario ContaUsuario { get; set; }
        public CategoriaMovimentacao Categoria { get; set; }
        public Movimentacao(){}
    }
}
