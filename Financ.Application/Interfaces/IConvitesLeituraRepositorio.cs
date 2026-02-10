using Financ.Application.Leitura.Convite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Interfaces
{
    public interface IConvitesLeituraRepositorio
    {
        Task<IEnumerable<LeituraRetornoConvites>> RetornoConvites(string idUsuarioDestinatario,bool retornaConvitesRemetente);
    }
}
