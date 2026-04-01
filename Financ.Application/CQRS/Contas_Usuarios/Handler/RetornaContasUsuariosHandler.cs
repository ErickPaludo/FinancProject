using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;


namespace Financ.Application.CQRS.Contas_Usuarios.Handler
{
    public class RetornaContasUsuariosHandler : IRequestHandler<RetornaContaUsuariosQuery, Resultado<BaseGet<RetornaContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RetornaContasUsuariosHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGet<RetornaContasDTO>>> Handle(RetornaContaUsuariosQuery request, CancellationToken cancellationToken)
        {
           var contasUsuarios = await ContasUsuariosSelecionadas(request);

            if (contasUsuarios.Count() == 0)
                return Resultado<BaseGet<RetornaContasDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhuma conta foi encontrada!"));

            List<RetornaContasDTO> listaContas = new List<RetornaContasDTO>();

            return Resultado<BaseGet<RetornaContasDTO>>.GeraSucesso(ContasUsuariosMapper.ParaDTO(contasUsuarios, request.Filtros));
        }
        private async Task<IEnumerable<ContasUsuarios>> ContasUsuariosSelecionadas(RetornaContaUsuariosQuery filtros)
        {
            var filtroId = filtros.Filtros?.Id;
            var filtroTitulo = filtros.Filtros?.Titulo;
            var filtroStatus = filtros.Filtros?.Status;
            var possuiFiltros = filtros.Filtros != null;

            var contasUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContasDoUsuario(
                x => x.IdUsuario == filtros.IdUsuario 
                && (!possuiFiltros || (
                    (!filtroId.HasValue || x.IdConta == filtroId.Value) &&
                    (string.IsNullOrEmpty(filtroTitulo) || x.Conta!.Titulo!.Contains(filtroTitulo)) &&
                    (!filtroStatus.HasValue || x.Conta!.Status == filtroStatus.Value)))
            );
            return contasUsuario;
        }
    }
}
