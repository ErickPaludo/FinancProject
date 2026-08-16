using Financ.Domain.Enums.Convites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.ContasBancarias.Mensagens
{
    public static class MensagensConvite
    {
        public const string USUARIO_REMETENTE_INVALIDO = "O usuário remetente é obrigatório.";
        public const string USUARIO_DESTINATARIO_INVALIDO = "O usuário destinatário é obrigatório.";

        public const string USUARIO_SEM_PERMISSAO = "Você não tem permissão para convidar usuários.";
        public const string USUARIO_CONTA_REMETENTE_INATIVO = "Seu usuário está inativo nesta conta.";
        public const string CONTA_JA_POSSUI_UM_USUARIO_MESTRES = "A conta já possui um usuário com acesso mestre.";

        public const string CONVITE_JA_VISUALIZADO = "Este convite já foi ";
        public const string CONVITE_EXPIRADO = "O convite expirou.";
        public const string USUARIO_REMETENTE_NAO_AUTORIZADO = "Você não é o remetente deste convite.";

        public const string USUARIO_DESTINATARIO_NAO_ENCONTRADO = "Usuário destinatário não encontrado.";
        public const string USUARIO_REMETENTE_NAO_ENCONTRADO = "Usuário remetente não encontrado.";

        public const string CONVITE_EM_ANDAMENTO = "Já existe um convite pendente para este usuário.";
        public const string USUARIO_JA_PERTENCE_A_CONTA = "Usuário já está cadastrado nesta conta.";

        public static string TEMPO_MIN_EXPIRACAO(int tempoMinimo) => $"O tempo mínimo de expiração para convites é  de {tempoMinimo} minutos.";
        public static string CONVITE_VISUALIZADO(EStatusConvite status) => $"Este convite foi {(status == EStatusConvite.Aceito ? "aceito" : "rejeitado")}";
        public static string LIMITE_ACESSOS(int quantidadeAcessos) => $"O número máximo de usuários na conta é {quantidadeAcessos}.";

    }
}
