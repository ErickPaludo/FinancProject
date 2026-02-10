using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Leitura.Convite
{
    public record LeituraRetornoConvites
    {
        public int id { get; init; }
        public TiposAcessos Acesso { get; init; }
        public bool? Aceito { get; init; }
        public DateTime DataEnvio { get; init; }
        public DateTime Expiracao { get; init; }
        public int IdConta { get; init; }
        public string Titulo { get; init; }
        public TiposContas TipoConta { get; init; }
        public string IdUsuarioRemetente { get; init; }
        public string PrimeiroNomeRemetente { get; init; }
        public string SegundoNomeRemetente { get; init; }
        public string NomeCompletoRemetente { get; init; }
        public string IdUsuarioDestinatario { get; init; }
        public string PrimeiroNomeDestinatario { get; init; }
        public string SegundoNomeDestinatario { get; init; }
        public string NomeCompletoDestinatario { get; init; }
    }
}
