using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Domain.Enums;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Querys
{
    public class RetornaUsuariosAssociadosQuery : IRequest<Resultado<List<RetornaUsuariosAssociadosDTO>>>
    {
        public string IdUsuarioSolicitante { get; set; }
        public int IdConta { get; set; }
        public string? IdUsuario { get; set; }
        public string? NomeUsuario { get; set; }
        public TiposAcessos? Acesso { get; set; }
        public TiposStatusContas? Status { get; set; }

        public RetornaUsuariosAssociadosQuery(string idUsuarioSolicitante, int idConta, string? idUsuario, string? nomeUsuario, TiposAcessos? acesso, TiposStatusContas? status)
        {
            IdUsuarioSolicitante = idUsuarioSolicitante;
            IdConta = idConta;
            IdUsuario = idUsuario;
            NomeUsuario = nomeUsuario;
            Acesso = acesso;
            Status = status;
        }
    }
}
