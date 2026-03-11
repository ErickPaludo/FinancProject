using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Mensagens
{
    public static class MensagensContasUsuarios
    {
        public const string IDCONTA_IGUAL_MENOR_ZERO = "IdConta não pode ser menor que zero";
        public const string CONTA_NAO_ESTA_ATIVA = "A conta selecionada não está ativa!";
        public const string CONTA_NAO_PODE_SER_NULA = "A conta não pode ser nula!";
        public const string IDUSUARIO_INVALIDO = "IdUsuario não deve ser vazio!";
        public const string ACESSO_INVALIDO = "O acesso informado é inválido.";
        public const string ACESSO_NEGADO = "O usuário não possui permissão para esta ação!";
        public const string ACESSO_NEGADO_POR_STATUS = "O usuário remetente não está ativo!";
        public const string USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO = "Não é possível alterar um usuário mestre!";
        public const string USUARIO_INATIVO_NAO_PODE_SER_ATUALIZADO = "O usuário não está ativo!";
        public const string USUARIO_NAO_PODE_SE_ATUALIZAR = "Não é possível alterar sua propria conta!";
        public const string MAX_MESTRES_CONVERTE_PARA_ADMIN = "Permissão de administrador concedida.";
        public const string UNICO_USUARIO_MESTRE_NA_CONTA = "A conta possui apenas um usuário mestre, é nescessário elevar o nivel de acesso de outro colaborador antes de sair";
        public const string USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO = "Não é possível remover um usuário mestre.";
        public const string USUARIO_TENTA_SE_EXPULSAR = "Não é possivel remover a si mesmo";
        public const string USUARIO_TENTA_SE_ATUALIZAR = "Não é possivel atualizar a si mesmo";

    }
}
