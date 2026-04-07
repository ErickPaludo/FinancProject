using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;

namespace Financ.Application.CQRS.Contas_Usuarios.Handler
{
    public class RetornaUsuariosAssociadosHandler : IRequestHandler<RetornaUsuariosAssociadosQuery, Resultado<BaseGet<RetornaUsuariosAssociadosDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RetornaUsuariosAssociadosHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGet<RetornaUsuariosAssociadosDTO>>> Handle(RetornaUsuariosAssociadosQuery request, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.contasUsuariosRepositorio.BuscarObjetoUnico(x => x.IdConta == request.IdConta && x.IdUsuario == request.IdUsuario) != null)
            {

                var filtroIdUsuario = !string.IsNullOrEmpty(request.filtroConta.IdUsuario);
                var filtroStatus = request.filtroConta.Status.HasValue;
                var filtroAcesso = request.filtroConta.Acesso.HasValue;
                var filtroNome = !string.IsNullOrEmpty(request.filtroConta.NomeUsuario);


                var contaUsuarios = await _unitOfWork.contasUsuariosRepositorio.ObterContasDoUsuario(x => x.IdConta == request.IdConta
                && (!filtroIdUsuario || x.IdUsuario.Equals(request.filtroConta.IdUsuario))
                && (!filtroStatus || x.Status.Equals(request.filtroConta.Status))
                && (!filtroAcesso || x.Acesso.Equals(request.filtroConta.Acesso)));

                List<RetornaUsuariosAssociadosDTO> listaUsuarios = new List<RetornaUsuariosAssociadosDTO>();
                if (contaUsuarios.Count() > 0)
                {
                    foreach (var conta in contaUsuarios)
                    {
                        Usuario? usuario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.Id.Equals(conta.IdUsuario));
                        listaUsuarios.Add(ContasUsuariosMapper.ParaUsuariosAssociadosDTO(conta, usuario));
                    }

                    if (filtroNome)
                        listaUsuarios = listaUsuarios.Where(x => x.Nome.Contains(request.filtroConta.NomeUsuario)).ToList();

                }
                return Resultado<BaseGet<RetornaUsuariosAssociadosDTO>>.GeraSucesso(new BaseGet<RetornaUsuariosAssociadosDTO>(listaUsuarios, new Meta
                {
                    filtros = request.filtroConta != null && request.filtroConta.IdUsuario == null && request.filtroConta.NomeUsuario == null && request.filtroConta.Status == null ? null : request.filtroConta,
                }));

            }
            return Resultado<BaseGet<RetornaUsuariosAssociadosDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada!"));
        }
    }
}
