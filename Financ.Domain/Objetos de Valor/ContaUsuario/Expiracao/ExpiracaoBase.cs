using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Financ.Domain.Objetos_de_Valor.ContaUsuario
{
    public abstract record ExpiracaoBase
    {
        protected DateTime Data { get; private set; }
        public int Minutos { get; }
        protected ExpiracaoBase(DateTime data) =>  Data = data;
        public bool EstaExpirado() => Data < DateTime.UtcNow;
    }
}
