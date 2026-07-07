using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Financ.Domain.Objetos_de_Valor.ContaUsuario
{
    public sealed record ExpiracaoContaUsuario : ExpiracaoBase
    {
        private readonly int _expiracaoMinutos = 15;
        private ExpiracaoContaUsuario(int minutos) : base(DateTime.UtcNow.AddMinutes(minutos)) {
            Valida(minutos);
        }
        public static ExpiracaoContaUsuario Create(int minutos) => new(minutos);
        private void Valida(int minutos)
         => ContasUsuariosValidacao.Verifica(minutos < _expiracaoMinutos, MensagensConvite.TEMPO_MIN_EXPIRACAO(_expiracaoMinutos));
    }
}
