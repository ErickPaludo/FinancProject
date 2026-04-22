using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.ContasBancarias.Mensagens
{
    public static class MensagensContasUsuarios
    {
 
            public const string IDCONTA_IGUAL_MENOR_ZERO = "O Id da conta deve ser maior que zero.";
            public const string CONTA_NAO_ESTA_ATIVA = "A conta selecionada está inativa.";
            public const string CONTA_NAO_PODE_SER_NULA = "A conta não pode ser nula.";
            public const string IDUSUARIO_INVALIDO = "O Id do usuário deve ser informado.";
            public const string ACESSO_INVALIDO = "O nível de acesso informado é inválido.";
            public const string ACESSO_NEGADO = "O usuário não possui permissão para realizar esta ação.";
            public const string ACESSO_NEGADO_POR_STATUS = "O usuário está inativo.";
            public const string USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO = "Não é permitido alterar um usuário mestre.";
            public const string USUARIO_NAO_PODE_SE_ATUALIZAR = "Não é permitido alterar a própria conta.";
            public const string MAX_MESTRES_CONVERTE_PARA_ADMIN = "Permissão alterada para administrador.";
            public const string UNICO_USUARIO_MESTRE_NA_CONTA = "A conta possui apenas um usuário mestre. É necessário promover outro usuário antes de sair.";
            public const string USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO = "Não é permitido remover um usuário mestre.";
            public const string USUARIO_TENTA_SE_EXPULSAR = "Não é permitido remover a própria conta.";
            public const string USUARIO_TENTA_SE_ATUALIZAR = "Não é permitido atualizar a própria conta.";
            public const string USUARIO_NAO_PERTENCE_A_CONTA = "O usuário não pertence a esta conta.";
            public const string USUARIO_POSSUI_CONVITES_EM_ANDAMENTO = "Não é possível sair da conta enquanto houver convites em andamento.";
            public const string ATUALIZA_PARA_USUARIO_MESTRE_DIFERENTE_DE_ATIVO = "Usuários mestres devem estar sempre com status ativo.";
            public const string MESTRE_NAO_POSSUI_TEMPO_LIMITE = "Usuários mestres não podem possuir tempo limite.";
            public const string TEMPO_MIN_EXPIRACAO = "O tempo mínimo de expiração é de 15 minutos.";
            public const string USUARIO_MESTRE_COM_TEMPO_LIMITE_JA_DEFINIDO = "Este usuário possui tempo limite definido. Remova o tempo limite antes de elevar o nível de acesso.";
            public const string ATUALIZANDO_TEMPO_LIMITE_PARA_USUARIO_MESTRE = "Não é permitido definir tempo limite para usuários mestres.";
            public const string CONFLITO_AO_EXPIRAR = "Operação inválida! Não é possível definir tempo de expirãção e cancelar ao mesmo tempo.";
            public const string CONVITE_NAO_PODE_SER_NULO = "Informe o convite.";
            public const string USUARIO_EXPIRADO = "Seu tempo na conta bancária está expirado.";
            public const string CONTA_NÃO_ENCONTRADA = "Conta não encontrada.";


    }
}
