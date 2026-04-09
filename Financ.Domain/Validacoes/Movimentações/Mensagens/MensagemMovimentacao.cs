using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Movimentações.Mensagens
{
    public class MensagemMovimentacao
    {
        public static string TIPO_MOV_INVALIDO => "Tipo de movimentação inválido!";
        public static string STATUS_INVALIDO => "Status inválido!";
        public static string TITULO_OBRIGATORIO => "Obrigatório informar o título!";
        public static string TITULO_LIMITE_CARACTERES => "Titulo deve possuir entre 3 e 80 caracteres.";
        public static string OBSERVACAO_LIMITE_CARACTERES => "Observação deve ter no máximo 255 caracteres.";
        public static string USUARIO_NAO_PERTENCE_A_CONTA => "Usuário não pertence a esta conta";
        public static string CONTA_NAO_ENCONTRADA => "Conta não encontrada";
        public static string CATEGORIA_NAO_PERTENCA_A_CONTA => "Categoria não pertence a esta conta";
        public static string USUARIO_INATIVO => "Usuário não está ativo.";
        public static string USUARIO_EXPIRADO => "Usuário com tempo expirado.";
        public static string USUARIO_SEM_PERMISSAO => "Usuário não possui permissão para este tipo de ação.";
        public static string VALOR_DEVE_SER_MAIOR_QUE_ZERO => "Valor deve ser maior que 0";
        public static string DATAS_INVALIDAS => "Data de pagamento não pode ser superior a data de registro.";

    }
}
