using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Movimentações.Fixas.Mensagens
{
    public class MensagemMovimentacaoFixa
    {
        public const string DATAS_INVALIDAS = "Data de inicio deve ser superior a data final";
        public const string TIPO_INVALIDO = "Tipo de movimentação inválido.";
        public const string MOVIMENTACAO_NAO_ESTA_OCULTA = "Ação não permitida. A movimentação base precisa estar com o status Oculto para gerar uma movimentação fixa.";
        public const string MOVIMENTACAO_DIARIA_NAO_INFORMADA = "Informe os dias da semana em que a movimentação deverá ocorrer.";
        public const string TIPO_DIARIO_NAO_PODE_SER_CRIA_COM_DATA_OCORRENCIA = "Movimentações diárias devem ser criadas sem data de ocorrência.";
    }
}
