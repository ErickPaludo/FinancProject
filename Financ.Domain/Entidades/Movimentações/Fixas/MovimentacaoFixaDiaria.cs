using Financ.Domain.Validacoes.Movimentações.Fixas;
using Financ.Domain.Validacoes.Movimentações.Fixas.Mensagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.Movimentações.Fixas
{
    public class MovimentacaoFixaDiaria
    {
        public int Id { get; private set; }
        public int IdFixo { get; private set; }
        public int DiaSemana { get; private set; }
        public MovimentacaoFixa MovimentacaoFixa { get; private set; }
        public MovimentacaoFixaDiaria() { }
        public MovimentacaoFixaDiaria(MovimentacaoFixa movimentacaoFixa, int diaSemana)
        {
            MovimentacaoFixa = movimentacaoFixa;
            IdFixo = movimentacaoFixa.Id;
            MovimentacaoFixaDiariaValidacao.Verifica(diaSemana < 0 && diaSemana > 6, MensagemMovimentacaoFixaDiaria.DIA_DA_SEMANA_INVALIDO);
            DiaSemana = diaSemana;
        }
    }
}
