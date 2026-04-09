using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.ContasBancarias.Mensagens
{
    public static class MensagensContas 
    {
        public const string ID_NAO_PODE_SER_IGUAL_A_ZERO = "O id da conta deve ser superior a zero.";
        public const string TITULO_OBRIGATORIO = "O título é obrigatório.";
        public const string TITULO_TAMANHO_INVALIDO = "O título deve possuir entre 3 e 100 caracteres.";
        public const string TIPO_CONTA_INVALIDO = "Tipo de conta inválido.";
        public const string FECHAMENTO_INVALIDO = "Dia de fechamento inválido, deve estar entre 1 e 16.";
        public const string VENCIMENTO_MENOR_FECHAMENTO = "O vencimento deve ser maior que o fechamento.";
        public const string VENCIMENTO_MINIMO_7_DIAS = "O vencimento deve ter pelo menos 7 dias de diferença entre fechamento e vencimento da fatura.";
        public const string VENCIMENTO_MAXIMO_12_DIAS = "O vencimento deve ser de no máximo 12 dias após o fechamento.";
        public const string CREDITO_MENOR_QUE_ZERO = "O Crédito informado deve ser maior que zero!";
        public const string ATUALIZA_CONTA_USUARIO_SEM_PERMISSAO = "Usuário sem permissão para atualizar a conta!";
        public const string ATUALIZA_CONTA_CREDITO_ATIVO = "Não é possível alterar crédito ativo pois o mesmo já está ativo!";
        public const string ATUALIZA_CONTA_CREDITO_MAXIMO_NULO = "O limite de crédito está ativo, portanto deve ser informado o crédito máximo!";
        public const string CONTA_INATIVA = "Esta conta não está ativa.";
        public static string SALDO_INSUFICIENTE => "Saldo insuficiente";

    }
}
