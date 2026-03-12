using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Mensagens
{
    public static class MensagensConvite
    {
        public const string USUARIO_REMETENTE_INVALIDO = "O usuário remetente deve ser informado";
        public const string USUARIO_DESTINATARIO_INVALIDO = "O usuário destinatário deve ser informado";
        public const string USUARIO_SEM_PERMISSAO = "Você não pode convidar um usuário, pois não é um usuário mestre.";
        public const string USUARIO_CONTA_REMETENTE_INATIVO = "Seu usuário não está ativo nesta conta.";
        public const string CONTA_JA_POSSUI_UM_USUARIO_MESTRES= "A conta já possui um usuário com acesso mestre.";
       
        public const string CONVITE_JA_VIZUALIZADO = "O convite já foi ";
        public const string CONVITE_EXPIRADO = "Convite expirado!";
        public const string USUARIO_REMETENTE_NAO_AUTORIZADO = "Você não é o usuário remetente!";

        
    }
}
