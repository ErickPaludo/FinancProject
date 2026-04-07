using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Base.Mensagens
{
    public static class MensagensBase
    {
        public const string ID_IGUAL_MENOR_ZERO = "Id não pode ser menor que zero";
        public const string DATA_REGISTRO_INVALIDA = "Deve ser registrada a data atual, esta não pode ser manipulada.";
        public const string USUARIO_NAO_INFORMADO = "Usuário não informado!";
        public const string STATUS_INVALIDO = "Status inválido.";
        public const string LIMITE_USUARIOS_MESTRES = "O limite de usuários mestres foi atingido. ";
        public const string USUARIO_INATIVO_NAO_PODE_SER_ATUALIZADO = "O usuário não está ativo!";

    }
}
