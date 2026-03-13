using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Autenticação;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_Usuarios.Handler
{
    public class RetornaUsuariosAssociadosHandler : IRequestHandler<RetornaUsuariosAssociadosQuery, Resultado<List<RetornaUsuariosAssociadosDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuarioServicos;

        public RetornaUsuariosAssociadosHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuarioServicos)
        {
            _unitOfWork = unitOfWork;
            _usuarioServicos = usuarioServicos;
        }

        public async Task<Resultado<List<RetornaUsuariosAssociadosDTO>>> Handle(RetornaUsuariosAssociadosQuery request, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.contasUsuariosRepositorio.BuscarObjetoUnico(x => x.IdConta == request.IdConta && x.IdUsuario == request.IdUsuarioSolicitante) != null)
            {

                var filtroIdUsuario = !string.IsNullOrEmpty(request.IdUsuario);
                var filtroStatus = request.Status.HasValue;
                var filtroAcesso = request.Acesso.HasValue;
                var filtroNome = !string.IsNullOrEmpty(request.NomeUsuario);
               

                var contaUsuarios = await _unitOfWork.contasUsuariosRepositorio.ObterContasDoUsuario(x => x.IdConta == request.IdConta //&& x.IdUsuario != request.IdUsuarioSolicitante
                && (!filtroIdUsuario || x.IdUsuario.Equals(request.IdUsuario))
                && (!filtroStatus || x.Status.Equals(request.Status))
                && (!filtroAcesso || x.Acesso.Equals(request.Acesso)));

                if (contaUsuarios.Count() > 0)
                {
                    List<RetornaUsuariosAssociadosDTO> listaUsuarios = new List<RetornaUsuariosAssociadosDTO>();
                    foreach (var conta in contaUsuarios)
                    {
                        var usuario = _usuarioServicos.ObtemUsuario(conta.IdUsuario).Result;
                        listaUsuarios.Add(ContasUsuariosMapper.ParaDTO(conta,usuario));
                    }

                    if (filtroNome) 
                        listaUsuarios = listaUsuarios.Where(x => x.Nome.Contains(request.NomeUsuario)).ToList();

                    return Resultado<List<RetornaUsuariosAssociadosDTO>>.GeraSucesso(listaUsuarios);
                }
                return Resultado<List<RetornaUsuariosAssociadosDTO>>.GeraFalha(Falha.NaoEncontrado("Somente você está associado a está conta!"));
            }
            return Resultado<List<RetornaUsuariosAssociadosDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada!"));
        }
    }
}
