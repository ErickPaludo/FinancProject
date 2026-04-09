using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.Movimentações
{
    public class Categoria
    {
        public int Id { get; private set; }
        public int IdConta { get; private set; }
        public string Nome { get; private set; }
        public Cor Cor { get; private set; }
        public Conta Conta { get; private set; }
        private Categoria() { }
        public Categoria(Conta conta, string nome, string cor)
        {
            IdConta = conta.Id;
            Conta = conta;
            ValidaNome(nome);
            Cor = new Cor(cor);
        }
        public Categoria(string nome, string cor)
        {
            ValidaNome(nome);
            Cor = new Cor(cor);
        }
        private void ValidaNome(string valor)
        {
            MovimentacaoValidacao.Verifica(string.IsNullOrWhiteSpace(valor), MensagemCategoria.NOME_OBRIGATORIO);
            MovimentacaoValidacao.Verifica(valor.Length < 3 || valor.Length > 50, MensagemCategoria.CARACTERES_INVALIDOS);
            Nome = valor;
        }
    }
}
